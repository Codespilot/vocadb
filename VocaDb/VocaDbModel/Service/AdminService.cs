using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.EntryValidators;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.DataSharing;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service {

	public class AdminService : ServiceBase {

		private void VerifyAdmin() {
			PermissionContext.VerifyPermission(PermissionToken.Admin);
		}

		public AdminService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public int CleanupOldLogEntries() {

			VerifyAdmin();

			AuditLog("cleaning up old log");

			return HandleTransaction(session => {

				var oldEntries = session.Query<ActivityEntry>().OrderByDescending(e => e.CreateDate).Skip(200).ToArray();

				foreach (var entry in oldEntries)
					session.Delete(entry);

				return oldEntries.Length;

			});

		}

		public void CreateMissingThumbs() {

			VerifyAdmin();

			HandleQuery(session => {

				var artistPic = session.Query<ArtistPictureFile>().ToArray();

				foreach (var pic in artistPic) {

					var thumbFile = ImageHelper.GetImagePathSmallThumb(pic);
					var origPath = ImageHelper.GetImagePath(pic);

					if (File.Exists(origPath) && !File.Exists(thumbFile)) {

						using (var original = Image.FromFile(ImageHelper.GetImagePath(pic))) {

							if (original.Width > ImageHelper.DefaultSmallThumbSize || original.Height > ImageHelper.DefaultSmallThumbSize) {
								var thumb = ImageHelper.ResizeToFixedSize(original, ImageHelper.DefaultSmallThumbSize, ImageHelper.DefaultSmallThumbSize);
								thumb.Save(thumbFile);
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
								var thumb = ImageHelper.ResizeToFixedSize(original, ImageHelper.DefaultSmallThumbSize, ImageHelper.DefaultSmallThumbSize);
								thumb.Save(thumbFile);
							} else {
								File.Copy(origPath, thumbFile);
							}

						}

					}

				}

			});

		}

		public void CreateXmlDump() {

			VerifyAdmin();

			AuditLog("creating XML dump");

			HandleQuery(session => {

				var dumper = new XmlDumper();
				dumper.Create(@"C:\inetpub\wwwroot\vocadb\dump.zip", session);

				/*var artists = session.Query<Artist>().Where(a => !a.Deleted).ToArray();
				var albums = session.Query<Album>().Where(a => !a.Deleted).ToArray();
				var songs = session.Query<Song>().Where(a => !a.Deleted).ToArray();

				dumper.Create(@"C:\inetpub\wwwroot\vocadb\dump.zip", artists, albums, songs);*/

			});

		}

		public void DeleteEntryReports(int[] reportIds) {

			ParamIs.NotNull(() => reportIds);

			PermissionContext.VerifyPermission(PermissionToken.ManageEntryReports);

			HandleTransaction(session => {

				var reports = session.Query<EntryReport>().Where(r => reportIds.Contains(r.Id)).ToArray();

				foreach (var report in reports)
					session.Delete(report);

				AuditLog(string.Format("deleted entry reports: {0}", string.Join(", ", reportIds)), session);

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

		public void GeneratePictureThumbs() {

			VerifyAdmin();

			AuditLog("generating thumbnails");

			HandleTransaction(session => {

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

			});

		}

		public AuditLogEntryContract[] GetAuditLog(string filter, int start, int maxEntries) {

			return HandleTransaction(session => {

				var entries = session.Query<AuditLogEntry>()
					.Where(e => string.IsNullOrEmpty(filter) || e.Action.Contains(filter) || e.AgentName == filter)
					.OrderByDescending(e => e.Time)
					.Skip(start)
					.Take(maxEntries)
					.ToArray()
					.Select(e => new AuditLogEntryContract(e))
					.ToArray();

				return entries;

			}, IsolationLevel.ReadUncommitted);

		}

		public UnifiedCommentContract[] GetRecentComments() {

			PermissionContext.VerifyPermission(PermissionToken.ReadRecentComments);
			const int maxComments = 50;

			return HandleQuery(session => {

				var albumComments = session.Query<AlbumComment>().Where(c => !c.Album.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();
				var artistComments = session.Query<ArtistComment>().Where(c => !c.Artist.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();
				var songComments = session.Query<SongComment>().Where(c => !c.Song.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();

				var combined = albumComments.Cast<Comment>().Concat(artistComments).Concat(songComments)
					.OrderByDescending(c => c.Created)
					.Take(maxComments)
					.Select(c => new UnifiedCommentContract(c));

				return combined.ToArray();

			});

		}

		public void UpdateAdditionalNames() {

			VerifyAdmin();

			AuditLog("updating sort names");

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

			AuditLog("updating album rating totals");

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

		public int UpdateEntryStatuses() {

			VerifyAdmin();

			AuditLog("updating entry statuses");

			return HandleTransaction(session => {

				int count = 0;

				var artists = session.Query<Artist>().Where(a => !a.Deleted).ToArray();

				foreach (var artist in artists) {

					var result = ArtistValidator.Validate(artist);

					if (result.Passed && artist.Status == EntryStatus.Draft) {
						artist.Status = EntryStatus.Finished;
						session.Update(artist);
						count++;
					} else if (!result.Passed && artist.Status == EntryStatus.Finished) {
						artist.Status = EntryStatus.Draft;
						session.Update(artist);
						count++;
					}

				}

				var albums = session.Query<Album>().Where(a => !a.Deleted).ToArray();

				foreach (var album in albums) {

					var result = AlbumValidator.Validate(album);

					if (result.Passed && album.Status == EntryStatus.Draft) {
						album.Status = EntryStatus.Finished;
						session.Update(album);
						count++;
					} else if (!result.Passed && album.Status == EntryStatus.Finished) {
						album.Status = EntryStatus.Draft;
						session.Update(album);
						count++;
					}

				}

				var songs = session.Query<Song>().Where(a => !a.Deleted).ToArray();

				foreach (var song in songs) {

					var result = SongValidator.Validate(song);

					if (result.Passed && song.Status == EntryStatus.Draft) {
						song.Status = EntryStatus.Finished;
						session.Update(song);
						count++;
					} else if (!result.Passed && song.Status == EntryStatus.Finished) {
						song.Status = EntryStatus.Draft;
						session.Update(song);
						count++;
					}

				}

				return count;

			});

		}

		public void UpdateSongFavoritedTimes() {

			VerifyAdmin();

			AuditLog("updating song favorites");

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

		public void UpdateIPRules(IPRule[] rules) {

			PermissionContext.VerifyPermission(PermissionToken.ManageIPRules);

			HandleTransaction(session => {

				AuditLog("updating IP rules", session);

				var ipRules = session.Query<IPRule>().ToArray();
				var diff = CollectionHelper.Diff(ipRules, rules, (r1, r2) => r1.Id == r2.Id);

				SessionHelper.Sync(session, diff);

			});

		}

		public void UpdateNicoIds() {

			UpdatePVIcons();

		}

		public void UpdatePVIcons() {

			VerifyAdmin();

			AuditLog("updating PVServices");

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

			HandleTransaction(session => {

				var catHelper = new WebLinkCategoryHelper();
				var webLinks = session.Query<T>().Where(l => l.Category == WebLinkCategory.Other).ToArray();

				foreach (var link in webLinks) {

					var oldCat = link.Category;
					link.Category = catHelper.GetCategory(link.Url);
					if (link.Category != oldCat)
						session.Update(link);

				}

			});

		}

		public void UpdateWebLinkCategories() {

			VerifyAdmin();

			AuditLog("Updating web link categories");

			UpdateWebLinkCategories<AlbumWebLink>();
			UpdateWebLinkCategories<ArtistWebLink>();
			UpdateWebLinkCategories<SongWebLink>();

		}

	}
}
