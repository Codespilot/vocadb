﻿using System;
using System.Drawing;
using System.Linq;
using System.Web;
using NHibernate;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="Album"/>.
	/// </summary>
	public class AlbumQueries : QueriesBase<IAlbumRepository, Album> {

		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEntryThumbPersister imagePersister;
		private readonly IUserMessageMailer mailer;

		private IEntryLinkFactory EntryLinkFactory {
			get { return entryLinkFactory; }
		}

		private void ArchiveSong(IRepositoryContext<Song> ctx, Song song, SongDiff diff, SongArchiveReason reason, string notes = "") {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedSongVersion>().Save(archived);

		}

		private Artist[] GetArtists(IRepositoryContext<Album> ctx, ArtistContract[] artistContracts) {
			var ids = artistContracts.Select(a => a.Id).ToArray();
			return ctx.OfType<Artist>().Query().Where(a => ids.Contains(a.Id)).ToArray();			
		}

		private void UpdateSongArtists(IRepositoryContext<Album> ctx, Song song, ArtistContract[] artistContracts) {

			var artistDiff = song.SyncArtists(artistContracts, 
				addedArtistContracts => GetArtists(ctx, addedArtistContracts));

			ctx.OfType<ArtistForSong>().Sync(artistDiff);

			if (artistDiff.Changed) {

				var diff = new SongDiff(DoSnapshot(song.GetLatestVersion(), ctx.OfType<User>().GetLoggedUser(PermissionContext))) { Artists = true };

				song.UpdateArtistString();
				ArchiveSong(ctx.OfType<Song>(), song, diff, SongArchiveReason.PropertiesUpdated);
				ctx.Update(song);

				ctx.AuditLogger.AuditLog("updated artists for " + entryLinkFactory.CreateEntryLink(song));
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), song, EntryEditEvent.Updated);

			}
			
		}

		public AlbumQueries(IAlbumRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, 
			IEntryThumbPersister imagePersister, IUserMessageMailer mailer)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.mailer = mailer;

		}

		public void Archive(IRepositoryContext<Album> ctx, Album album, AlbumDiff diff, AlbumArchiveReason reason, string notes = "") {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedAlbumVersion.Create(album, diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedAlbumVersion>().Save(archived);

		}

		public void Archive(IRepositoryContext<Album> ctx, Album album, AlbumArchiveReason reason, string notes = "") {

			Archive(ctx, album, new AlbumDiff(), reason, notes);

		}

		public AlbumContract Create(CreateAlbumContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Album needs at least one name", "contract");

			VerifyManageDatabase();

			return repository.HandleTransaction(ctx => {

				ctx.AuditLogger.SysLog(string.Format("creating a new album with name '{0}'", contract.Names.First().Value));

				var album = new Album { DiscType = contract.DiscType };

				album.Names.Init(contract.Names, album);

				ctx.Save(album);

				foreach (var artistContract in contract.Artists) {
					var artist = ctx.OfType<Artist>().Load(artistContract.Id);
					if (!album.HasArtist(artist))
						ctx.OfType<ArtistForAlbum>().Save(ctx.OfType<Artist>().Load(artist.Id).AddAlbum(album));
				}

				album.UpdateArtistString();
				Archive(ctx, album, AlbumArchiveReason.Created);
				ctx.Update(album);

				ctx.AuditLogger.AuditLog(string.Format("created album {0} ({1})", entryLinkFactory.CreateEntryLink(album), album.DiscType));
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), album, EntryEditEvent.Created);

				new FollowedArtistNotifier().SendNotifications(ctx.OfType<UserMessage>(), album, album.ArtistList, PermissionContext.LoggedUser, 
					entryLinkFactory, mailer);

				return new AlbumContract(album, PermissionContext.LanguagePreference);

			});

		}

		public CommentContract CreateComment(int albumId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return repository.HandleTransaction(ctx => {

				var album = ctx.Load(albumId);
				var agent = ctx.CreateAgentLoginData(PermissionContext);

				ctx.AuditLogger.AuditLog(string.Format("creating comment for {0}: '{1}'",
					entryLinkFactory.CreateEntryLink(album),
					HttpUtility.HtmlEncode(message)), agent.User);

				var comment = album.CreateComment(message, agent);
				ctx.OfType<AlbumComment>().Save(comment);

				new UserCommentNotifier().CheckComment(comment, entryLinkFactory, ctx.OfType<User>());

				return new CommentContract(comment);

			});

		}

		public EntryForPictureDisplayContract GetCoverPictureThumb(int albumId) {
			
			var size = new Size(ImageHelper.DefaultThumbSize, ImageHelper.DefaultThumbSize);

			return repository.HandleQuery(ctx => {
				
				var album = ctx.Load(albumId);

				if (album.CoverPictureData == null || string.IsNullOrEmpty(album.CoverPictureMime) || album.CoverPictureData.HasThumb(size))
					return EntryForPictureDisplayContract.Create(album, PermissionContext.LanguagePreference, size);

				var data = new EntryThumb(album, album.CoverPictureMime);

				if (imagePersister.HasImage(data, ImageSize.Thumb)) {
					using (var stream = imagePersister.GetReadStream(data, ImageSize.Thumb)) {
						var bytes = StreamHelper.ReadStream(stream);
						return EntryForPictureDisplayContract.Create(album, data.Mime, bytes, PermissionContext.LanguagePreference);
					}
				}

				return EntryForPictureDisplayContract.Create(album, PermissionContext.LanguagePreference, size);

			});

		}

		public RelatedAlbumsContract GetRelatedAlbums(int albumId) {

			return repository.HandleQuery(ctx => {

				var album = ctx.Load(albumId);
				var q = new RelatedAlbumsQuery(ctx);
				var albums = q.GetRelatedAlbums(album);

				return new RelatedAlbumsContract { 
					ArtistMatches = 
						albums.ArtistMatches
						.Select(a => new AlbumContract(a, permissionContext.LanguagePreference))
						.OrderBy(a => a.Name)
						.ToArray(),
					LikeMatches = 
						albums.LikeMatches
						.Select(a => new AlbumContract(a, permissionContext.LanguagePreference))
						.OrderBy(a => a.Name)
						.ToArray(),
					TagMatches = 
						albums.TagMatches
						.Select(a => new AlbumContract(a, permissionContext.LanguagePreference))
						.OrderBy(a => a.Name)
						.ToArray()
					};

			});

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target albums can't be the same", "targetId");

			repository.HandleTransaction(session => {

				var source = session.Load(sourceId);
				var target = session.Load(targetId);

				session.AuditLogger.AuditLog(string.Format("Merging {0} to {1}", EntryLinkFactory.CreateEntryLink(source), EntryLinkFactory.CreateEntryLink(target)));

				NHibernateUtil.Initialize(source.CoverPictureData);
				NHibernateUtil.Initialize(target.CoverPictureData);

				foreach (var n in source.Names.Names.Where(n => !target.HasName(n))) {
					var name = target.CreateName(n.Value, n.Language);
					session.Save(name);
				}

				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url, w.Category);
					session.Save(link);
				}

				var artists = source.Artists.Where(a => !target.HasArtistForAlbum(a)).ToArray();
				foreach (var a in artists) {
					a.Move(target);
					session.Update(a);
				}

				var songs = source.Songs.Where(s => s.Song == null || !target.HasSong(s.Song)).ToArray();
				foreach (var s in songs) {
					s.Move(target);
					session.Update(s);
				}

				var pictures = source.Pictures.ToArray();
				foreach (var p in pictures) {
					p.Move(target);
					session.Update(p);
				}

				var userCollections = source.UserCollections.Where(a => !target.IsInUserCollection(a.User)).ToArray();
				foreach (var u in userCollections) {
					u.Move(target);
					session.Update(u);
				}

				if (target.Description == string.Empty)
					target.Description = source.Description;

				if (target.OriginalRelease == null)
					target.OriginalRelease = new AlbumRelease();

				if (string.IsNullOrEmpty(target.OriginalRelease.CatNum) && source.OriginalRelease != null)
					target.OriginalRelease.CatNum = source.OriginalRelease.CatNum;

				if (string.IsNullOrEmpty(target.OriginalRelease.EventName) && source.OriginalRelease != null)
					target.OriginalRelease.EventName = source.OriginalRelease.EventName;

				if (target.OriginalRelease.ReleaseDate == null)
					target.OriginalRelease.ReleaseDate = new OptionalDateTime();

				if (target.OriginalReleaseDate.Year == null && source.OriginalRelease != null)
					target.OriginalReleaseDate.Year = source.OriginalReleaseDate.Year;

				if (target.OriginalReleaseDate.Month == null && source.OriginalRelease != null)
					target.OriginalReleaseDate.Month = source.OriginalReleaseDate.Month;

				if (target.OriginalReleaseDate.Day == null && source.OriginalRelease != null)
					target.OriginalReleaseDate.Day = source.OriginalReleaseDate.Day;

				// Create merge record
				var mergeEntry = new AlbumMergeRecord(source, target);
				session.Save(mergeEntry);

				source.Deleted = true;

				target.UpdateArtistString();
				target.Names.UpdateSortNames();

				Archive(session, target, AlbumArchiveReason.Merged, string.Format("Merged from {0}", source));

				session.Update(source);
				session.Update(target);

			});

		}

		public AlbumForEditContract UpdateBasicProperties(AlbumForEditContract properties, EntryPictureFileContract pictureData) {

			ParamIs.NotNull(() => properties);

			return repository.HandleTransaction(session => {

				var album = session.Load(properties.Id);

				VerifyEntryEdit(album);

				var diff = new AlbumDiff(DoSnapshot(album.ArchivedVersionsManager.GetLatestVersion(), session.OfType<User>().GetLoggedUser(PermissionContext)));

				session.AuditLogger.SysLog(string.Format("updating properties for {0}", album));

				if (album.DiscType != properties.DiscType) {
					album.DiscType = properties.DiscType;
					album.UpdateArtistString();
					diff.DiscType = true;
				}

				if (album.Description != properties.Description) {
					album.Description = properties.Description;
					diff.Description = true;
				}

				var parsedBarcode = Album.ParseBarcode(properties.Barcode);
				if (album.Barcode != parsedBarcode) {
					album.Barcode = parsedBarcode;
					diff.Barcode = true;
				}

				if (album.TranslatedName.DefaultLanguage != properties.TranslatedName.DefaultLanguage) {
					album.TranslatedName.DefaultLanguage = properties.TranslatedName.DefaultLanguage;
					diff.OriginalName = true;
				}

				var validNames = properties.Names;
				var nameDiff = album.Names.Sync(validNames, album);
				session.OfType<AlbumName>().Sync(nameDiff);

				album.Names.UpdateSortNames();

				if (nameDiff.Changed)
					diff.Names = true;

				var validWebLinks = properties.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
				var webLinkDiff = WebLink.Sync(album.WebLinks, validWebLinks, album);
				session.OfType<AlbumWebLink>().Sync(webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks = true;

				var newOriginalRelease = (properties.OriginalRelease != null ? new AlbumRelease(properties.OriginalRelease) : new AlbumRelease());

				if (album.OriginalRelease == null)
					album.OriginalRelease = new AlbumRelease();

				if (!album.OriginalRelease.Equals(newOriginalRelease)) {
					album.OriginalRelease = newOriginalRelease;
					diff.OriginalRelease = true;
				}

				// Required because of a bug in NHibernate
				NHibernateUtil.Initialize(album.CoverPictureData);

				if (pictureData != null) {

					var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);
					album.CoverPictureData = new PictureData(parsed);
					album.CoverPictureMime = parsed.Mime;

					pictureData.Id = album.Id;
					pictureData.EntryType = EntryType.Album;
					var thumbGenerator = new ImageThumbGenerator(imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);

					diff.Cover = true;

				}

				if (album.Status != properties.Status) {
					album.Status = properties.Status;
					diff.Status = true;
				}

				var artistGetter = new Func<ArtistContract, Artist>(artist => 
					session.OfType<Artist>().Load(artist.Id));

				var artistsDiff = album.SyncArtists(properties.ArtistLinks, artistGetter);
				session.OfType<ArtistForAlbum>().Sync(artistsDiff);

				if (artistsDiff.Changed)
					diff.Artists = true;

				var songGetter = new Func<SongInAlbumEditContract, Song>(contract => {

					if (contract.SongId != 0)
						return session.Load<Album, Song>(contract.SongId);
					else {

						session.AuditLogger.SysLog(string.Format("creating a new song '{0}' to {1}", contract.SongName, album));

						var song = new Song(new LocalizedString(contract.SongName, ContentLanguageSelection.Unspecified));
						session.Save(song);

						var songDiff = new SongDiff { Names = true };
						var songArtistDiff = song.SyncArtists(contract.Artists, 
							addedArtistContracts => GetArtists(session, addedArtistContracts));

						if (songArtistDiff.Changed) {
							songDiff.Artists = true;
							session.Update(song);
						}

						session.OfType<ArtistForSong>().Sync(songArtistDiff);

						ArchiveSong(session.OfType<Song>(), song, songDiff, SongArchiveReason.Created,
							string.Format("Created for album '{0}'", album.DefaultName));

						session.AuditLogger.AuditLog(string.Format("created {0} for {1}",
							entryLinkFactory.CreateEntryLink(song), entryLinkFactory.CreateEntryLink(album)));
						AddEntryEditedEntry(session.OfType<ActivityEntry>(), song, EntryEditEvent.Created);

						return song;

					}

				});

				var tracksDiff = album.SyncSongs(properties.Songs, songGetter, 
					(song, artistContracts) => UpdateSongArtists(session, song, artistContracts));

				session.OfType<SongInAlbum>().Sync(tracksDiff);

				if (tracksDiff.Changed) {

					var add = string.Join(", ", tracksDiff.Added.Select(i => i.SongToStringOrName));
					var rem = string.Join(", ", tracksDiff.Removed.Select(i => i.SongToStringOrName));
					var edit = string.Join(", ", tracksDiff.Edited.Select(i => i.SongToStringOrName));

					var str = string.Format("edited tracks (added: {0}, removed: {1}, reordered: {2})", add, rem, edit)
						.Truncate(300);

					session.AuditLogger.AuditLog(str);

					diff.Tracks = true;

				}

				var picsDiff = album.Pictures.SyncPictures(properties.Pictures, session.OfType<User>().GetLoggedUser(PermissionContext), album.CreatePicture);
				session.OfType<AlbumPictureFile>().Sync(picsDiff);
				ImageHelper.GenerateThumbsAndMoveImages(picsDiff.Added);

				if (picsDiff.Changed)
					diff.Pictures = true;

				var pvDiff = album.SyncPVs(properties.PVs);
				session.OfType<PVForAlbum>().Sync(pvDiff);

				if (pvDiff.Changed)
					diff.PVs = true;

				var logStr = string.Format("updated properties for album {0} ({1})", 
					entryLinkFactory.CreateEntryLink(album), diff.ChangedFieldsString)
					+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
					.Truncate(400);

				session.AuditLogger.AuditLog(logStr);

				AddEntryEditedEntry(session.OfType<ActivityEntry>(), album, EntryEditEvent.Updated);

				Archive(session, album, diff, AlbumArchiveReason.PropertiesUpdated, properties.UpdateNotes);
				session.Update(album);

				var newSongCutoff = TimeSpan.FromHours(1);
				if (artistsDiff.Added.Any() && album.CreateDate >= DateTime.Now - newSongCutoff) {

					var addedArtists = artistsDiff.Added.Where(a => a.Artist != null).Select(a => a.Artist).Distinct().ToArray();

					if (addedArtists.Any()) {
						new FollowedArtistNotifier().SendNotifications(session.OfType<UserMessage>(), album, addedArtists, PermissionContext.LoggedUser, entryLinkFactory, mailer);											
					}

				}

				return new AlbumForEditContract(album, PermissionContext.LanguagePreference);

			});

		}

	}
}