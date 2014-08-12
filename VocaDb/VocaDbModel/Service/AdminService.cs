﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.DataSharing;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service {

	public class AdminService : ServiceBase {

		private void VerifyAdmin() {
			PermissionContext.VerifyPermission(PermissionToken.Admin);
		}

		public AdminService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public int CleanupOldLogEntries() {

			VerifyAdmin();

			SysLog("cleaning up old log");

			return HandleTransaction(session => {

				var oldEntries = session.Query<ActivityEntry>().OrderByDescending(e => e.CreateDate).Skip(200).ToArray();

				foreach (var entry in oldEntries)
					session.Delete(entry);

				return oldEntries.Length;

			});

		}

		public void CreateMissingThumbs() {

			VerifyAdmin();

			var imagePersister = new ServerEntryThumbPersister();
			var thumbGenerator = new ImageThumbGenerator(imagePersister);
			var artistIds = new int[0];

			HandleQuery(session => {
				artistIds = session.Query<Artist>().Where(a => !a.Deleted && a.PictureMime != null && a.PictureMime != "").Select(a => a.Id).ToArray();
			});

			for (int i = 0; i < artistIds.Length; i += 100) {
				
				var ids = artistIds.Skip(i).Take(100).ToArray();

				HandleQuery(session => {

					var artists = session.Query<Artist>().Where(a => ids.Contains(a.Id)).ToArray();

					foreach (var artist in artists) {

						var data = new EntryThumb(artist, artist.PictureMime);

						if (artist.Picture.Bytes == null || imagePersister.HasImage(data, ImageSize.Thumb))
							continue;

						using (var stream = new MemoryStream(artist.Picture.Bytes)) {
							thumbGenerator.GenerateThumbsAndMoveImage(stream, data, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);						
						}

					}

				});

			}

			/*var albumIds = new int[0];

			HandleQuery(session => {
				albumIds = session.Query<Album>().Where(a => !a.Deleted && a.CoverPictureData.Mime != null && a.CoverPictureData.Mime != "").Select(a => a.Id).ToArray();
			});

			for (int i = 0; i < albumIds.Length; i += 100) {
				
				var ids = albumIds.Skip(i).Take(100).ToArray();

				HandleQuery(session => {

					var albums = session.Query<Album>().Where(a => ids.Contains(a.Id)).ToArray();

					foreach (var album in albums) {

						var data = new EntryThumb(album, album.CoverPictureData.Mime);

						if (album.CoverPictureData.Bytes == null || imagePersister.HasImage(data, ImageSize.Thumb))
							continue;

						using (var stream = new MemoryStream(album.CoverPictureData.Bytes)) {
							thumbGenerator.GenerateThumbsAndMoveImage(stream, data, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);						
						}

					}

				});

			}*/

			HandleQuery(session => {

				/*var artistPic = session.Query<ArtistPictureFile>().ToArray();

				foreach (var pic in artistPic) {

					var thumbFile = ImageHelper.GetImagePathSmallThumb(pic);
					var origPath = ImageHelper.GetImagePath(pic);

					if (File.Exists(origPath) && !File.Exists(thumbFile)) {

						using (var original = Image.FromFile(ImageHelper.GetImagePath(pic))) {

							if (original.Width > ImageHelper.DefaultSmallThumbSize || original.Height > ImageHelper.DefaultSmallThumbSize) {
								using (var thumb = ImageHelper.ResizeToFixedSize(original, ImageHelper.DefaultSmallThumbSize, ImageHelper.DefaultSmallThumbSize)) {
									thumb.Save(thumbFile);
								}
							} else {
								File.Copy(origPath, thumbFile);
							}

						}

					}

				}

				var albumPic = session.Query<AlbumPictureFile>().ToArray();

				foreach (var pic in albumPic) {

					var thumbFile = ImageHelper.GetImagePathSmallThumb(pic);
					var origPath = ImageHelper.GetImagePath(pic);

					if (File.Exists(origPath) && !File.Exists(thumbFile)) {

						using (var original = Image.FromFile(ImageHelper.GetImagePath(pic))) {

							if (original.Width > ImageHelper.DefaultSmallThumbSize || original.Height > ImageHelper.DefaultSmallThumbSize) {
								using (var thumb = ImageHelper.ResizeToFixedSize(original, ImageHelper.DefaultSmallThumbSize, ImageHelper.DefaultSmallThumbSize)) {
									thumb.Save(thumbFile);									
								}
							} else {
								File.Copy(origPath, thumbFile);
							}

						}

					}

				}*/

			});

		}

		public void CreateXmlDump() {

			VerifyAdmin();

			SysLog("creating XML dump");

			HandleQuery(session => {

				var dumper = new XmlDumper();
				var path = Path.Combine(AppConfig.DbDumpFolder, "dump.zip");
				dumper.Create(path, session);

			});

		}

		public void DeleteEntryReports(int[] reportIds) {

			ParamIs.NotNull(() => reportIds);

			PermissionContext.VerifyPermission(PermissionToken.ManageEntryReports);

			HandleTransaction(session => {

				var reports = session.Query<EntryReport>().Where(r => reportIds.Contains(r.Id)).ToArray();

				foreach (var report in reports)
					session.Delete(report);

				AuditLog(string.Format("deleted reports: {0}", string.Join(", ", reports.Select(r => r.ToString()))), session);

			});

		}

		public int DeletePVsByAuthor(string author, PVService service) {

			PermissionContext.VerifyPermission(PermissionToken.BulkDeletePVs);

			if (string.IsNullOrEmpty(author))
				return 0;

			return HandleTransaction(session => {

				AuditLog(string.Format("deleting PVs by '{0}' for service {1}.", author, service), session);

				var pvs = session.Query<PVForSong>().Where(p => p.Service == service && p.Author == author).ToArray();

				foreach (var pv in pvs) {
					pv.OnDelete();
					session.Delete(pv);
				}

				return pvs.Length;

			});

		}

		public string[] FindPVAuthorNames(string term) {

			if (string.IsNullOrEmpty(term))
				return new string[] {};

			return HandleQuery(session => {

				return session.Query<PVForSong>()
					.Where(p => p.Author.Contains(term))
					.OrderBy(p => p.Author)
					.Select(p => p.Author)
					.Distinct()
					.ToArray();

			});

		}

		public EntryReportContract[] GetEntryReports() {

			PermissionContext.VerifyPermission(PermissionToken.ManageEntryReports);

			return HandleQuery(session => {

				var reports = session.Query<EntryReport>().OrderByDescending(r => r.Created).Take(200).ToArray();
				var fac = new EntryReportContractFactory();

				return reports.Select(r => fac.Create(r, PermissionContext.LanguagePreference)).ToArray();

			});

		}

		public int GeneratePictureThumbs() {

			VerifyAdmin();
			int count = 0;

			HandleTransaction(session => {

				AuditLog("generating thumbnails", session);

				var changed = new List<Album>(100);
				var albums = session.Query<Album>().Where(a => !a.Deleted).ToArray();

				foreach (var album in albums) {
					
					if (album.CoverPictureData != null && album.CoverPictureData.Bytes != null) {
						
						using (var stream = new MemoryStream(album.CoverPictureData.Bytes)) {
							var thumbs = ImageHelper.GenerateThumbs(stream, new[] {250});
							if (thumbs.Any()) {
								album.CoverPictureData.Thumb250 = new PictureThumb250(thumbs.First().Bytes);
								changed.Add(album);
							}
						}

					}

				}

				foreach (var album in changed)
					session.Update(album);

				count += changed.Count;

			});

			HandleTransaction(session => {

				var changed = new List<Artist>(100);
				var artists = session.Query<Artist>().Where(s => !s.Deleted).ToArray();

				foreach (var artist in artists) {

					if (artist.Picture != null && artist.Picture.Bytes != null) {

						using (var stream = new MemoryStream(artist.Picture.Bytes)) {
							var thumbs = ImageHelper.GenerateThumbs(stream, new[] { 250 });
							if (thumbs.Any()) {
								artist.Picture.Thumb250 = new PictureThumb250(thumbs.First().Bytes);
								changed.Add(artist);
							}
						}

					}

				}

				foreach (var artist in changed)
					session.Update(artist);

				count += changed.Count;

			});

			return count;

		}

		public AuditLogEntryContract[] GetAuditLog(string filter, int start, int maxEntries, int timeCutoffDays,
 			string userName, string[] excludeUsers, bool onlyNewUsers,
			AuditLogUserGroupFilter filterByGroup = AuditLogUserGroupFilter.Nothing) {

			return HandleTransaction(session => {

				var q = session.Query<AuditLogEntry>();

				if (timeCutoffDays > 0) {
					var cutoff = DateTime.Now - TimeSpan.FromDays(timeCutoffDays);
					q = q.Where(e => e.Time > cutoff);					
				}

				if (!string.IsNullOrWhiteSpace(filter)) {
					q = q.Where(e => e.Action.Contains(filter));
				}

				if (!string.IsNullOrWhiteSpace(userName)) {
					q = q.Where(e => e.AgentName == userName || e.User.Options.LastLoginAddress == userName);
				}

				if (excludeUsers.Any()) {

					var usr = session.Query<User>().Where(u => excludeUsers.Contains(u.Name)).Select(u => u.Id).Distinct().ToArray();

					q = q.Where(e => !usr.Contains(e.User.Id));

				}

				if (onlyNewUsers) {

					var newUserDate = DateTime.Now - TimeSpan.FromDays(7);
					q = q.Where(e => e.User.CreateDate >= newUserDate);

				}

				if (filterByGroup != AuditLogUserGroupFilter.Nothing && filterByGroup != AuditLogUserGroupFilter.NoFilter) {
					var userGroup = EnumVal<UserGroupId>.Parse(filterByGroup.ToString());
					q = q.Where(e => e.User != null && e.User.GroupId == userGroup);					
				}

				if (filterByGroup == AuditLogUserGroupFilter.Nothing)
					q = q.Where(e => e.User == null);

				var entries = q
					.OrderByDescending(e => e.Time)
					.Skip(start)
					.Take(maxEntries)
					.ToArray()
					.Select(e => new AuditLogEntryContract(e))
					.ToArray();

				return entries;

			}, IsolationLevel.ReadUncommitted);

		}

		public PVForSongContract[] GetSongPVsByAuthor(string author) {

			if (string.IsNullOrEmpty(author))
				return new PVForSongContract[] {};

			return HandleQuery(session => {

				return session.Query<PVForSong>().Where(p => p.Author == author)
					.Take(50)
					.ToArray()
					.Select(p => new PVForSongContract(p, LanguagePreference))
					.ToArray();

			});

		}

		public void UpdateAdditionalNames() {

			VerifyAdmin();

			SysLog("updating sort names");

			HandleTransaction(session => {

				var artists = session.Query<Artist>().Where(a => !a.Deleted).ToArray();

				foreach (var artist in artists) {

					artist.Names.UpdateSortNames();
					session.Update(artist);

				}

				var albums = session.Query<Album>().Where(a => !a.Deleted).ToArray();

				foreach (var album in albums) {

					album.Names.UpdateSortNames();
					session.Update(album);

				}

				var songs = session.Query<Song>().Where(a => !a.Deleted).ToArray();

				foreach (var song in songs) {

					song.Names.UpdateSortNames();
					session.Update(song);

				}

			});

		}

		public void UpdateAlbumRatingTotals() {

			VerifyAdmin();

			SysLog("updating album rating totals");

			HandleTransaction(session => {

				var albums = session.Query<Album>().Where(a => !a.Deleted).ToArray();

				foreach (var album in albums) {

					var oldCount = album.RatingCount;
					var oldTotal = album.RatingTotal;
					album.UpdateRatingTotals();
					if (oldCount != album.RatingCount || oldTotal != album.RatingTotal)
						session.Update(album);

				}

			});


		}

		public void UpdateArtistStrings() {

			VerifyAdmin();

			HandleTransaction(session => {

				AuditLog("rebuilding artist strings", session);

				var albums = session.Query<Album>().Where(a => !a.Deleted).ToArray();

				foreach (var album in albums) {

					var old = album.ArtistString;

					album.UpdateArtistString();

					if (album.ArtistString != old)
						session.Update(album);

				}

				var songs = session.Query<Song>().Where(s => !s.Deleted).ToArray();

				foreach (var song in songs) {

					var old = song.ArtistString;

					song.UpdateArtistString();

					if (song.ArtistString != old)
						session.Update(song);

				}

			});

		}

		public void UpdateSongFavoritedTimes() {

			VerifyAdmin();

			SysLog("updating song favorites");

			HandleTransaction(session => {

				var ratings = session.Query<FavoriteSongForUser>().Where(a => !a.Song.Deleted).GroupBy(s => s.Song.Id);

				foreach (var songRating in ratings) {

					var song = session.Load<Song>(songRating.Key);
					song.FavoritedTimes = songRating.Count(r => r.Rating == SongVoteRating.Favorite);
					song.RatingScore = songRating.Sum(r => FavoriteSongForUser.GetRatingScore(r.Rating));

					session.Update(song);

				}

			});
			

		}

		private void UpdateVoteCounts<T>(ISession session, IEnumerable<T> usages, ref int count) where T : TagUsage {

			foreach (var usage in usages) {

				var realCount = usage.VotesBase.Count();

				if (usage.Count != realCount) {
					usage.Count = realCount;
					session.Update(usage);
					count++;
				}

			}

		}

		public int UpdateTagVoteCounts() {

			VerifyAdmin();

			SysLog("updating tag vote counts");

			int count = 0;

			HandleTransaction(session => {

				var artistUsages = session.Query<ArtistTagUsage>().Where(a => !a.Artist.Deleted).ToArray();
				UpdateVoteCounts(session, artistUsages, ref count);

				var albumUsages = session.Query<AlbumTagUsage>().Where(a => !a.Album.Deleted).ToArray();
				UpdateVoteCounts(session, albumUsages, ref count);

				var songUsages = session.Query<SongTagUsage>().Where(a => !a.Song.Deleted).ToArray();
				UpdateVoteCounts(session, songUsages, ref count);

			});

			return count;

		}

		public void UpdateIPRules(IPRule[] rules) {

			PermissionContext.VerifyPermission(PermissionToken.ManageIPRules);

			HandleTransaction(session => {

				AuditLog("updating IP rules", session);

				var ipRules = session.Query<IPRule>().ToArray();
				var diff = CollectionHelper.Diff(ipRules, rules, (r1, r2) => r1.Id == r2.Id);

				foreach (var entry in diff.Unchanged)
					entry.Notes = rules.First(r => r.Id == entry.Id).Notes;

				SessionHelper.Sync(session, diff);

			});

		}

		public void UpdateNicoIds() {

			UpdatePVIcons();

		}

		public void UpdatePVIcons() {

			VerifyAdmin();

			SysLog("updating PVServices");

			HandleTransaction(session => {

				var songs = session.Query<Song>().Where(a => !a.Deleted).ToArray();

				foreach (var song in songs) {

					var nicoId = song.NicoId;
					var old = song.PVServices;

					song.UpdateNicoId();
					song.UpdatePVServices();

					if (song.NicoId != nicoId || song.PVServices != old)
						session.Update(song);

				}

			});

		}

		private void UpdateWebLinkCategories<T>() where T : WebLink {

			/*HandleTransaction(session => {

				var catHelper = new WebLinkCategoryHelper();
				var webLinks = session.Query<T>().Where(l => l.Category == WebLinkCategory.Other).ToArray();

				foreach (var link in webLinks) {

					var oldCat = link.Category;
					link.Category = catHelper.GetCategory(link.Url);
					if (link.Category != oldCat)
						session.Update(link);

				}

			});*/

		}

		public void UpdateWebLinkCategories() {

			VerifyAdmin();

			SysLog("Updating web link categories");

			UpdateWebLinkCategories<AlbumWebLink>();
			UpdateWebLinkCategories<ArtistWebLink>();
			UpdateWebLinkCategories<SongWebLink>();

		}

	}

	public enum AuditLogUserGroupFilter {

		NoFilter,

		Nothing,

		Limited,

		Regular,

		Trusted,

		Moderator,

		Admin,

	}

}
