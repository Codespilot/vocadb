using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.EntryValidators;

namespace VocaDb.Model.Service {

	public class AdminService : ServiceBase {

		public AdminService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

		public void GeneratePictureThumbs() {
			
			PermissionContext.VerifyPermission(PermissionFlags.Admin);

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

		public void UpdateArtistStrings() {

			PermissionContext.VerifyPermission(PermissionFlags.Admin);

			HandleTransaction(session => {

				AuditLog("rebuilding artist strings", session);

				var albums = session.Query<Album>().Where(a => !a.Deleted).ToArray();

				foreach (var album in albums) {
					album.UpdateArtistString();
				}

				var songs = session.Query<Song>().Where(s => !s.Deleted).ToArray();

				foreach (var song in songs) {
					song.UpdateArtistString();
				}

				foreach (var album in albums)
					session.Update(album);

				foreach (var song in songs)
					session.Update(song);

			});

		}

		public int UpdateEntryStatuses() {

			PermissionContext.VerifyPermission(PermissionFlags.Admin);

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

		public void UpdateNicoIds() {

			PermissionContext.VerifyPermission(PermissionFlags.Admin);

			AuditLog("updating NicoIDs");

			HandleTransaction(session => {

				var songs = session.Query<Song>().Where(a => !a.Deleted).ToArray();

				foreach (var song in songs) {

					if (!string.IsNullOrEmpty(song.NicoId) && !song.PVs.Any())
						session.Save(song.CreatePV(PVService.NicoNicoDouga, song.NicoId, PVType.Original));

					song.UpdateNicoId();

					session.Update(song);
				}

			});

		}

		public void UpdatePVIcons() {

			PermissionContext.VerifyPermission(PermissionFlags.Admin);

			AuditLog("updating PVServices");

			HandleTransaction(session => {

				var songs = session.Query<Song>().Where(a => !a.Deleted).ToArray();

				foreach (var song in songs) {

					var old = song.PVServices;

					song.UpdatePVServices();

					if (song.PVServices != old)
						session.Update(song);

				}

			});

		}

	}
}
