﻿using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;
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
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Domain.Caching;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="Artist"/>.
	/// </summary>
	public class ArtistQueries : QueriesBase<IArtistRepository, Artist> {

		private readonly ObjectCache cache;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEntryThumbPersister imagePersister;

		private PersonalArtistStatsContract GetPersonalArtistStats(IRepositoryContext<Artist> ctx, Artist artist) {
			
			if (!PermissionContext.IsLoggedIn)
				return null;

			var key = string.Format("PersonalArtistStatsContract.{0}.{1}", artist.Id, PermissionContext.LoggedUserId);
			return cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(1), () => {
				
				return new PersonalArtistStatsContract {
					SongRatingCount = ctx.OfType<FavoriteSongForUser>()
						.Query()
						.Count(f => f.User.Id == PermissionContext.LoggedUserId && f.Song.AllArtists.Any(a => a.Artist.Id == artist.Id))
				};

			});
			
		}

		private SharedArtistStatsContract GetSharedArtistStats(IRepositoryContext<Artist> ctx, Artist artist) {
			
			var key = string.Format("SharedArtistStatsContract.{0}", artist.Id);
			return cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(1), () => {
				
				var stats = ctx.Query()
					.Where(a => a.Id == artist.Id)
					.Select(a => new {
						AlbumCount = a.AllAlbums.Count,
						RatedAlbumCount = a.AllAlbums.Count(l => l.Album.RatingCount > 0),
						SongCount = a.AllSongs.Count,
						RatedSongCount = a.AllSongs.Count(s => s.Song.RatingScore > 0),
						AlbumRatingsTotalCount = a.AllAlbums.Any() ? a.AllAlbums.Sum(l => l.Album.RatingCount) : 0,
						AlbumRatingsTotalSum = a.AllAlbums.Any() ? a.AllAlbums.Sum(l => l.Album.RatingTotal) : 0
					})
					.FirstOrDefault();

				return new SharedArtistStatsContract {
					AlbumCount = stats.AlbumCount,
					RatedAlbumCount = stats.RatedAlbumCount,
					SongCount = stats.SongCount,
					RatedSongCount = stats.RatedSongCount,
					AlbumRatingAverage = (stats.AlbumRatingsTotalCount > 0 ? Math.Round(stats.AlbumRatingsTotalSum / (double)stats.AlbumRatingsTotalCount, 2) : 0)
				};

			});
			
		}

		private ArtistMergeRecord GetMergeRecord(IRepositoryContext<Artist> session, int sourceId) {
			return session.OfType<ArtistMergeRecord>().Query().FirstOrDefault(s => s.Source == sourceId);
		}

		public ArtistQueries(IArtistRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, IEntryThumbPersister imagePersister,
			ObjectCache cache)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.cache = cache;

		}

		public void Archive(IRepositoryContext<Artist> ctx, Artist artist, ArtistDiff diff, ArtistArchiveReason reason, string notes = "") {

			ctx.AuditLogger.SysLog("Archiving " + artist);

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedArtistVersion.Create(artist, diff, agentLoginData, reason, notes);
			ctx.Save(archived);

		}

		public void Archive(IRepositoryContext<Artist> ctx, Artist artist, ArtistArchiveReason reason, string notes = "") {

			Archive(ctx, artist, new ArtistDiff(), reason, notes);

		}

		public ArtistContract Create(CreateArtistContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Artist needs at least one name", "contract");

			VerifyManageDatabase();

			return repository.HandleTransaction(ctx => {

				ctx.AuditLogger.SysLog(string.Format("creating a new artist with name '{0}'", contract.Names.First().Value));

				var artist = new Artist { 
					ArtistType = contract.ArtistType, 
					Description = contract.Description.Trim(), 
					Status = contract.Draft ? EntryStatus.Draft : EntryStatus.Finished 
				};

				artist.Names.Init(contract.Names, artist);

				if (contract.WebLink != null) {
					artist.CreateWebLink(contract.WebLink.Description, contract.WebLink.Url, contract.WebLink.Category);
				}

				ctx.Save(artist);

				if (contract.PictureData != null) {

					var pictureData = contract.PictureData;
					var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);
					artist.Picture = new PictureData(parsed);
					artist.PictureMime = parsed.Mime;

					pictureData.Id = artist.Id;
					pictureData.EntryType = EntryType.Artist;
					var thumbGenerator = new ImageThumbGenerator(imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);

				}

				Archive(ctx, artist, ArtistArchiveReason.Created);
				ctx.Update(artist);

				ctx.AuditLogger.AuditLog(string.Format("created artist {0} ({1})", entryLinkFactory.CreateEntryLink(artist), artist.ArtistType));
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), artist, EntryEditEvent.Created);

				return new ArtistContract(artist, PermissionContext.LanguagePreference);

			});

		}

		public CommentContract CreateComment(int artistId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return repository.HandleTransaction(ctx => {

				var artist = ctx.Load(artistId);
				var author = ctx.OfType<User>().GetLoggedUser(PermissionContext);

				ctx.AuditLogger.AuditLog(string.Format("creating comment for {0}: '{1}'",
					entryLinkFactory.CreateEntryLink(artist),
					HttpUtility.HtmlEncode(message.Truncate(60))), author);

				var comment = artist.CreateComment(message, author);
				ctx.OfType<ArtistComment>().Save(comment);

				new UserCommentNotifier().CheckComment(comment, entryLinkFactory, ctx.OfType<User>());

				return new CommentContract(comment);

			});

		}

		public ArtistDetailsContract GetDetails(int id) {

			return HandleQuery(session => {

				var artist = session.Load(id);

				var stats = session.Query()
					.Where(a => a.Id == id)
					.Select(a => new {
						CommentCount = a.Comments.Count,
						FollowCount = a.Users.Count
					})
					.FirstOrDefault();

				if (stats == null)
					EntityNotFoundException.Throw<Artist>(id);

				var contract = new ArtistDetailsContract(artist, LanguagePreference) {
					CommentCount = stats.CommentCount,
					FollowCount = stats.FollowCount,
					SharedStats = GetSharedArtistStats(session, artist),
					PersonalStats = GetPersonalArtistStats(session, artist)
				};

				if (PermissionContext.LoggedUser != null) {

					var subscription = session.OfType<ArtistForUser>().Query().FirstOrDefault(s => s.Artist.Id == id && s.User.Id == PermissionContext.LoggedUser.Id);

					if (subscription != null) {
						contract.IsAdded = true;
						contract.EmailNotifications = subscription.EmailNotifications;
					}

				}

				var relations = (new ArtistRelationsQuery(session, LanguagePreference)).GetRelations(artist, ArtistRelationsFields.All);
				contract.LatestAlbums = relations.LatestAlbums;
				contract.TopAlbums = relations.PopularAlbums;
				contract.LatestSongs = relations.LatestSongs;
				contract.TopSongs = relations.PopularSongs;

				contract.LatestComments = session.OfType<ArtistComment>().Query()
					.Where(c => c.Artist.Id == id)
					.OrderByDescending(c => c.Created)
					.Take(3)
					.ToArray()
					.Select(c => new CommentContract(c)).ToArray();

				if (artist.Deleted) {
					var mergeEntry = GetMergeRecord(session, id);
					contract.MergedTo = (mergeEntry != null ? new ArtistContract(mergeEntry.Target, LanguagePreference) : null);
				}

				return contract;

			});

		}

		public T GetWithMergeRecord<T>(int id, Func<Artist, ArtistMergeRecord, IRepositoryContext<Artist>, T> fac) {

			return HandleQuery(session => {
				var artist = session.Load(id);
				return fac(artist, (artist.Deleted ? GetMergeRecord(session, id) : null), session);
			});

		}

		public EntryForPictureDisplayContract GetPictureThumb(int artistId) {
			
			var size = new Size(ImageHelper.DefaultThumbSize, ImageHelper.DefaultThumbSize);

			return repository.HandleQuery(ctx => {
				
				var artist = ctx.Load(artistId);

				if (artist.Picture == null || string.IsNullOrEmpty(artist.PictureMime) || artist.Picture.HasThumb(size))
					return EntryForPictureDisplayContract.Create(artist, PermissionContext.LanguagePreference, size);

				var data = new EntryThumb(artist, artist.PictureMime);

				if (imagePersister.HasImage(data, ImageSize.Thumb)) {
					using (var stream = imagePersister.GetReadStream(data, ImageSize.Thumb)) {
						var bytes = StreamHelper.ReadStream(stream);
						return EntryForPictureDisplayContract.Create(artist, data.Mime, bytes, PermissionContext.LanguagePreference);
					}
				}

				return EntryForPictureDisplayContract.Create(artist, PermissionContext.LanguagePreference, size);

			});

		}
		public int Update(ArtistForEditContract properties, EntryPictureFileContract pictureData, IUserPermissionContext permissionContext) {
			
			ParamIs.NotNull(() => properties);
			ParamIs.NotNull(() => permissionContext);

			return repository.HandleTransaction(ctx => {

				var artist = ctx.Load(properties.Id);

				VerifyEntryEdit(artist);

				var diff = new ArtistDiff(DoSnapshot(artist.GetLatestVersion(), ctx.OfType<User>().GetLoggedUser(permissionContext)));

				ctx.AuditLogger.SysLog(string.Format("updating properties for {0}", artist));

				if (artist.ArtistType != properties.ArtistType) {
					artist.ArtistType = properties.ArtistType;
					diff.ArtistType = true;
				}

				if (artist.Description != properties.Description) {
					artist.Description = properties.Description;
					diff.Description = true;
				}

				if (artist.TranslatedName.DefaultLanguage != properties.TranslatedName.DefaultLanguage) {
					artist.TranslatedName.DefaultLanguage = properties.TranslatedName.DefaultLanguage;
					diff.OriginalName = true;
				}

				// Required because of a bug in NHibernate
				NHibernateUtil.Initialize(artist.Picture);

				if (pictureData != null) {

					var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);
					artist.Picture = new PictureData(parsed);
					artist.PictureMime = parsed.Mime;

					pictureData.Id = artist.Id;
					pictureData.EntryType = EntryType.Artist;
					var thumbGenerator = new ImageThumbGenerator(imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);

					diff.Picture = true;

				}

				if (artist.Status != properties.Status) {
					artist.Status = properties.Status;
					diff.Status = true;
				}

				var nameDiff = artist.Names.Sync(properties.Names, artist);
				ctx.OfType<ArtistName>().Sync(nameDiff);

				if (nameDiff.Changed)
					diff.Names = true;

				if (!artist.BaseVoicebank.NullSafeIdEquals(properties.BaseVoicebank)) {
					
					var newBase = ctx.NullSafeLoad(properties.BaseVoicebank);

					if (artist.IsValidBaseVoicebank(newBase)) {
						diff.BaseVoicebank = true;
						artist.SetBaseVoicebank(ctx.NullSafeLoad(properties.BaseVoicebank));
					}

				}

				var validWebLinks = properties.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
				var webLinkDiff = WebLink.Sync(artist.WebLinks, validWebLinks, artist);
				ctx.OfType<ArtistWebLink>().Sync(webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks = true;

				if (diff.ArtistType || diff.Names) {

					foreach (var song in artist.Songs) {
						song.Song.UpdateArtistString();
						ctx.Update(song);
					}

				}

				var groupsDiff = CollectionHelper.Diff(artist.Groups, properties.Groups, (i, i2) => (i.Id == i2.Id));

				foreach (var grp in groupsDiff.Removed) {
					grp.Delete();
					ctx.Delete(grp);
				}

				foreach (var grp in groupsDiff.Added) {
					var link = artist.AddGroup(ctx.Load(grp.Group.Id));
					ctx.Save(link);
				}

				if (groupsDiff.Changed)
					diff.Groups = true;

				var picsDiff = artist.Pictures.SyncPictures(properties.Pictures, ctx.OfType<User>().GetLoggedUser(permissionContext), artist.CreatePicture);
				ctx.OfType<ArtistPictureFile>().Sync(picsDiff);
				ImageHelper.GenerateThumbsAndMoveImages(picsDiff.Added);

				if (picsDiff.Changed)
					diff.Pictures = true;

				var logStr = string.Format("updated properties for artist {0} ({1})", entryLinkFactory.CreateEntryLink(artist), diff.ChangedFieldsString)
					+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
					.Truncate(400);

				ctx.AuditLogger.AuditLog(logStr);
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), artist, EntryEditEvent.Updated);

				Archive(ctx, artist, diff, ArtistArchiveReason.PropertiesUpdated, properties.UpdateNotes);
				ctx.Update(artist);

				return artist.Id;

			});

		}

	}
}