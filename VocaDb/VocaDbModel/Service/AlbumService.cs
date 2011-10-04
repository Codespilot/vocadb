using System.Linq;
using log4net;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Service {

	public class AlbumService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(AlbumService));

		private void Archive(ISession session, Album album) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedAlbumVersion.Create(album, agentLoginData);
			session.Save(archived);

		}

		public AlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

		public ArtistForAlbumContract AddArtist(int albumId, string newArtistName) {

			ParamIs.NotNullOrEmpty(() => newArtistName);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			AuditLog("creating artist '" + newArtistName + "' to album '" + albumId + "'");

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var artist = new Artist(new TranslatedString(newArtistName));

				var artistForAlbum = artist.AddAlbum(album);
				session.Save(artist);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

		public SongInAlbumContract AddSong(int albumId, string newSongName) {

			ParamIs.NotNullOrEmpty(() => newSongName);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			AuditLog("creating song '" + newSongName + "' to album '" + albumId + "'");

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var song = new Song(new TranslatedString(newSongName), null);

				session.Save(song);
				var songInAlbum = album.AddSong(song);
				session.Save(songInAlbum);

				return new SongInAlbumContract(songInAlbum, PermissionContext.LanguagePreference);

			});

		}

		public SongInAlbumContract AddSong(int albumId, int songId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			AuditLog("adding song '" + songId + "' to album '" + albumId + "'");

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var song = session.Load<Song>(songId);

				var songInAlbum = album.AddSong(song);
				session.Save(songInAlbum);

				return new SongInAlbumContract(songInAlbum, PermissionContext.LanguagePreference);

			});

		}

		public AlbumContract Create(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			log.Info("'" + PermissionContext.Name + "' creating an artist");

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var artist = new Album(new TranslatedString(name));

				session.Save(artist);

				return new AlbumContract(artist, PermissionContext.LanguagePreference);

			});

		}

		public LocalizedStringWithIdContract CreateName(int albumId, string nameVal, ContentLanguageSelection language) {

			ParamIs.NotNullOrEmpty(() => nameVal);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				var name = album.CreateName(nameVal, language);
				session.Save(name);
				return new LocalizedStringWithIdContract(name);

			});

		}

		public WebLinkContract CreateWebLink(int artistId, string description, string url) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var artist = session.Load<Album>(artistId);

				var link = artist.CreateWebLink(description, url);
				session.Save(link);

				return new WebLinkContract(link);

			});

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<Album>(id, (session, a) => {

				AuditLog(string.Format("deleting album '{0}'", a.Name));

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();

			});

		}

		public void DeleteArtistForAlbum(int artistForAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			DeleteEntity<ArtistForAlbum>(artistForAlbumId);

		}

		public void DeleteName(int nameId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			DeleteEntity<AlbumName>(nameId);

		}

		public void DeleteSongInAlbum(int songInAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var songInAlbum = session.Load<SongInAlbum>(songInAlbumId);

				AuditLog("removing " + songInAlbum);

				songInAlbum.OnDeleting();

				session.Delete(songInAlbum);

			});

		}

		public void DeleteWebLink(int linkId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			DeleteEntity<AlbumWebLink>(linkId);

		}

		public AlbumContract[] Find(string query, int maxResults) {

			return HandleQuery(session => {

				var direct = session.Query<Album>()
					.Where(s => 
						!s.Deleted &&
						(string.IsNullOrEmpty(query)
							|| s.TranslatedName.English.Contains(query)
							|| s.TranslatedName.Romaji.Contains(query)
							|| s.TranslatedName.Japanese.Contains(query)))
					.OrderBy(s => s.TranslatedName.Japanese)
					.Take(maxResults)
					.ToArray();

				return direct
					.Select(a => new AlbumContract(a, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public AlbumDetailsContract GetAlbumDetails(int id) {

			return HandleQuery(session => new AlbumDetailsContract(session.Load<Album>(id), PermissionContext.LanguagePreference));

		}

		public AlbumForEditContract GetAlbumForEdit(int id) {

			return
				HandleQuery(session =>
					new AlbumForEditContract(session.Load<Album>(id), PermissionContext.LanguagePreference));

		}

		public AlbumContract[] GetAlbums() {

			return HandleQuery(session => session.Query<Album>()
				.Where(a => !a.Deleted)
				.ToArray()
				.OrderBy(a => a.Name)
				.Select(a => new AlbumContract(a, PermissionContext.LanguagePreference))
				.ToArray());

		}

		public PictureDataContract GetCoverPicture(int id) {

			return HandleQuery(session => {

				var album = session.Load<Album>(id);

				if (album.CoverPicture != null)
					return new PictureDataContract(album.CoverPicture);
				else
					return null;

			});

		}

		public AlbumForEditContract UpdateBasicProperties(AlbumForEditContract properties, PictureDataContract pictureData) {

			ParamIs.NotNull(() => properties);

			log.Info(string.Format("'{0}' updating properties for album '{1}'", PermissionContext.Name, properties.Name));

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(properties.Id);

				Archive(session, album);

				album.DiscType = properties.DiscType;
				album.Description = properties.Description;
				album.TranslatedName.CopyFrom(properties.TranslatedName);

				if (pictureData != null) {
					album.CoverPicture = new PictureData(pictureData);
				}

				session.Update(album);
				return new AlbumForEditContract(album, PermissionContext.LanguagePreference);

			});

		}

		public void UpdateNameLanguage(int nameId, ContentLanguageSelection lang) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<AlbumName>(nameId, name => name.Language = lang);

		}

		public void UpdateNameValue(int nameId, string val) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<AlbumName>(nameId, name => name.Value = val);

		}

		public void UpdateWebLinkDescription(int linkId, string description) {

			ParamIs.NotNull(() => description);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<AlbumWebLink>(linkId, link => link.Description = description);

		}

		public void UpdateWebLinkUrl(int nameId, string url) {

			ParamIs.NotNullOrEmpty(() => url);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<AlbumWebLink>(nameId, link => link.Url = url);

		}

	}

}
