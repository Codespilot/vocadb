using System.Collections.Generic;
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
using System;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.DataSharing;

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

		public void GeneratePictureThumbs() {

			VerifyAdmin();

			AuditLog("generating thumbnails");

			HandleTransaction(session => {

				var changed = new List<Album>(100);
				var albums = session.Query<Album>().Where(a => !a.Deleted).ToArray();

				foreach (var album in albums) {
					
					if (album.CoverPicture != null && album.CoverPicture.Bytes != null) {
						
						using (var stream = new MemoryStream(album.CoverPicture.Bytes)) {
							var thumbs = ImageHelper.GenerateThumbs(stream, new[] {250});
							if (thumbs.Any()) {
								album.CoverPicture.Thumb250 = new PictureThumb250(thumbs.First().Bytes);
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

		public AuditLogEntryContract[] GetAuditLog() {

			return HandleQuery(session => {

				var entries = session.Query<AuditLogEntry>()
					.OrderByDescending(e => e.Time)
					.Take(200)
					.ToArray()
					.Select(e => new AuditLogEntryContract(e))
					.ToArray();

				return entries;

			});

		}

		public UnifiedCommentContract[] GetRecentComments() {

			PermissionContext.VerifyPermission(PermissionToken.ReadRecentComments);
			const int maxComments = 50;

			return HandleQuery(session => {

				var albumComments = session.Query<AlbumComment>().Where(c => !c.Album.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();
				var artistComments = session.Query<ArtistComment>().Where(c => !c.Artist.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();

				var combined = albumComments.Select(c => new UnifiedCommentContract(c, PermissionContext.LanguagePreference)).Concat(
					artistComments.Select(c => new UnifiedCommentContract(c, PermissionContext.LanguagePreference))).OrderByDescending(c => c.Created).Take(maxComments);

				return combined.ToArray();

			});

		}

		public void UpdateAdditionalNames() {

			VerifyAdmin();

			AuditLog("updating additional names strings");

			HandleTransaction(session => {

				var artists = session.Query<Artist>().Where(a => !a.Deleted).ToArray();

				foreach (var artist in artists) {

					var old = artist.Names.AdditionalNamesString;
					artist.Names.UpdateSortNames();

					if (old != artist.Names.AdditionalNamesString)
						session.Update(artist);

				}

				var albums = session.Query<Album>().Where(a => !a.Deleted).ToArray();

				foreach (var album in albums) {

					var old = album.Names.AdditionalNamesString;
					album.Names.UpdateSortNames();

					if (old != album.Names.AdditionalNamesString)
						session.Update(album);

				}

				var songs = session.Query<Song>().Where(a => !a.Deleted).ToArray();

				foreach (var song in songs) {

					var old = song.Names.AdditionalNamesString;
					song.Names.UpdateSortNames();

					if (old != song.Names.AdditionalNamesString)
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

				var songs = session.Query<Song>().Where(a => !a.Deleted).ToArray();

				foreach (var song in songs) {

					var oldVal = song.FavoritedTimes;
					song.FavoritedTimes = session.Query<FavoriteSongForUser>().Count(s => s.Song.Id == song.Id);
					if (oldVal != song.FavoritedTimes)
						session.Update(song);

				}

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

	}
}
