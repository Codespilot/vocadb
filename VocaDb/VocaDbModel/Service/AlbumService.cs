using System.Collections.Generic;
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
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Artists;
using System.Drawing;

namespace VocaDb.Model.Service {

	public class AlbumService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(AlbumService));

		private void Archive(ISession session, Album album) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedAlbumVersion.Create(album, agentLoginData);
			session.Save(archived);

		}

		private int GetAlbumCount(ISession session, string query) {

			if (string.IsNullOrWhiteSpace(query)) {

				return session.Query<Album>()
					.Where(s => !s.Deleted)
					.Count();

			}

			var direct = session.Query<Album>()
				.Where(s =>
					!s.Deleted &&
					(string.IsNullOrEmpty(query)
						|| s.TranslatedName.English.Contains(query)
						|| s.TranslatedName.Romaji.Contains(query)
						|| s.TranslatedName.Japanese.Contains(query)
					|| (s.ArtistString.Contains(query))))
				.ToArray();

			var additionalNames = session.Query<AlbumName>()
				.Where(m => m.Value.Contains(query) && !m.Album.Deleted)
				.Select(m => m.Album)
				.Distinct()
				.ToArray()
				.Where(a => !direct.Contains(a))
				.ToArray();

			return direct.Count() + additionalNames.Count();

		}

		private IEnumerable<Artist> GetAllLabels(ISession session) {

			return session.Query<Artist>().Where(a => a.ArtistType == ArtistType.Label);

		}

		public AlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

		public ArtistForAlbumContract AddArtist(int albumId, string newArtistName) {

			ParamIs.NotNullOrEmpty(() => newArtistName);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var artist = new Artist(new TranslatedString(newArtistName));

				AuditLog(string.Format("creating a new artist '{0}' to {1}", newArtistName, album));

				var artistForAlbum = artist.AddAlbum(album);
				session.Save(artist);

				album.UpdateArtistString();
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

		public SongInAlbumContract AddSong(int albumId, string newSongName) {

			ParamIs.NotNullOrEmpty(() => newSongName);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("creating a new song '{0}' to {1}", newSongName, album));

				var song = new Song(new TranslatedString(newSongName), null);

				session.Save(song);
				var songInAlbum = album.AddSong(song);
				session.Save(songInAlbum);

				return new SongInAlbumContract(songInAlbum, PermissionContext.LanguagePreference);

			});

		}

		public SongInAlbumContract AddSong(int albumId, int songId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var song = session.Load<Song>(songId);

				AuditLog(string.Format("adding {0} to {1}", song, album));

				var songInAlbum = album.AddSong(song);
				session.Save(songInAlbum);

				return new SongInAlbumContract(songInAlbum, PermissionContext.LanguagePreference);

			});

		}

		public AlbumContract Create(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			AuditLog("creating a new artist with name '" + name + "'");

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

				AuditLog("creating name '" + nameVal + "' for " + album);

				var name = album.CreateName(nameVal, language);
				session.Save(name);
				return new LocalizedStringWithIdContract(name);

			});

		}

		public WebLinkContract CreateWebLink(int albumId, string description, string url) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				AuditLog("creating web link for " + album);

				var link = album.CreateWebLink(description, url);
				session.Save(link);

				return new WebLinkContract(link);

			});

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<Album>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", a));

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();

			});

		}

		public void DeleteArtistForAlbum(int artistForAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var artistForAlbum = session.Load<ArtistForAlbum>(artistForAlbumId);

				AuditLog("deleting " + artistForAlbum);

				artistForAlbum.Album.DeleteArtistForAlbum(artistForAlbum);
				session.Delete(artistForAlbum);
				session.Update(artistForAlbum.Album);

			});

		}

		public void DeleteName(int nameId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			DeleteEntity<AlbumName>(nameId);

		}

		public SongInAlbumContract[] DeleteSongInAlbum(int songInAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var songInAlbum = session.Load<SongInAlbum>(songInAlbumId);

				AuditLog("removing " + songInAlbum);

				songInAlbum.OnDeleting();
				session.Delete(songInAlbum);
				session.Update(songInAlbum.Album);

				return songInAlbum.Album.Songs.Select(s => new SongInAlbumContract(s, PermissionContext.LanguagePreference)).ToArray();

			});

		}

		public void DeleteWebLink(int linkId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			DeleteEntity<AlbumWebLink>(linkId);

		}

		public PartialFindResult<AlbumWithAdditionalNamesContract> Find(string query, int start, int maxResults, bool getTotalCount = false) {

			return HandleQuery(session => {

				if (string.IsNullOrWhiteSpace(query)) {

					var albums = session.Query<Album>()
						.Where(s => !s.Deleted)
						.OrderBy(s => s.TranslatedName.Romaji)
						.Skip(start)
						.Take(maxResults)
						.ToArray();

					var contracts = albums.Select(s => new AlbumWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
						.ToArray();

					var count = (getTotalCount ? GetAlbumCount(session, query) : 0);

					return new PartialFindResult<AlbumWithAdditionalNamesContract>(contracts, count);

				} else {

					var direct = session.Query<Album>()
						.Where(s =>
							(!s.Deleted
							&& (string.IsNullOrEmpty(query)
								|| s.TranslatedName.English.Contains(query)
								|| s.TranslatedName.Romaji.Contains(query)
								|| s.TranslatedName.Japanese.Contains(query)))
							|| (s.ArtistString.Contains(query)))
						.OrderBy(s => s.TranslatedName.Romaji)
						.Take(maxResults)
						.ToArray();

					var additionalNames = session.Query<AlbumName>()
						.Where(m => m.Value.Contains(query) && !m.Album.Deleted)
						.Select(m => m.Album)
						.OrderBy(s => s.TranslatedName.Romaji)
						.Distinct()
						.Take(maxResults)
						.ToArray()
						.Where(a => !direct.Contains(a));

					var contracts = direct.Concat(additionalNames)
						.Skip(start)
						.Take(maxResults)
						.Select(a => new AlbumWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
						.ToArray();

					var count = (getTotalCount ? GetAlbumCount(session, query) : 0);

					return new PartialFindResult<AlbumWithAdditionalNamesContract>(contracts, count);

				}

			});

		}

		public AlbumDetailsContract GetAlbumDetails(int id) {

			return HandleQuery(session => {

				var contract = new AlbumDetailsContract(session.Load<Album>(id), PermissionContext.LanguagePreference);
				if (PermissionContext.LoggedUser != null)
					contract.UserHasAlbum = session.Query<AlbumForUser>()
						.Any(a => a.Album.Id == id && a.User.Id == PermissionContext.LoggedUser.Id);

				return contract;

			});

		}

		public AlbumForEditContract GetAlbumForEdit(int id) {

			return
				HandleQuery(session =>
					new AlbumForEditContract(session.Load<Album>(id), GetAllLabels(session), PermissionContext.LanguagePreference));

		}

		public AlbumContract[] GetAlbums() {

			return HandleQuery(session => session.Query<Album>()
				.Where(a => !a.Deleted)
				.ToArray()
				.OrderBy(a => a.Name)
				.Select(a => new AlbumContract(a, PermissionContext.LanguagePreference))
				.ToArray());

		}

		/// <summary>
		/// Gets the cover picture for a <see cref="Album"/>.
		/// </summary>
		/// <param name="id">Album Id.</param>
		/// <param name="requestedSize">Requested size. If Empty, original size will be returned.</param>
		/// <returns>Data contract for the picture. Can be null if there is no picture.</returns>
		public PictureContract GetCoverPicture(int id, Size requestedSize) {

			return HandleQuery(session => {

				var album = session.Load<Album>(id);

				if (album.CoverPicture != null)
					return new PictureContract(album.CoverPicture, requestedSize);
				else
					return null;

			});

		}

		public SongInAlbumContract[] MoveSongDown(int songInAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var songInAlbum = session.Load<SongInAlbum>(songInAlbumId);

				AuditLog("moving down " + songInAlbum);

				songInAlbum.Album.MoveSongDown(songInAlbum);
				session.Update(songInAlbum.Album);

				return songInAlbum.Album.Songs.OrderBy(s => s.TrackNumber).Select(s => new SongInAlbumContract(s, PermissionContext.LanguagePreference)).ToArray();

			});

		}

		public SongInAlbumContract[] MoveSongUp(int songInAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var songInAlbum = session.Load<SongInAlbum>(songInAlbumId);

				AuditLog("moving up " + songInAlbum);

				songInAlbum.Album.MoveSongUp(songInAlbum);
				session.Update(songInAlbum.Album);

				return songInAlbum.Album.Songs.OrderBy(s => s.TrackNumber).Select(s => new SongInAlbumContract(s, PermissionContext.LanguagePreference)).ToArray();

			});
			
		}

		public AlbumForEditContract UpdateBasicProperties(AlbumForEditContract properties, PictureDataContract pictureData) {

			ParamIs.NotNull(() => properties);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(properties.Id);

				AuditLog(string.Format("updating properties for {0}", album));
				Archive(session, album);

				album.DiscType = properties.DiscType;
				album.Description = properties.Description;
				album.TranslatedName.CopyFrom(properties.TranslatedName);
				
				if (properties.OriginalRelease != null) {
					album.OriginalRelease = new AlbumRelease(properties.OriginalRelease, (properties.OriginalRelease.Label != null ? session.Load<Artist>(properties.OriginalRelease.Label.Id) : null));
				} else {
					album.OriginalRelease = null;
				}

				if (pictureData != null) {
					album.CoverPicture = new PictureData(pictureData);
				}

				session.Update(album);
				return new AlbumForEditContract(album, GetAllLabels(session), PermissionContext.LanguagePreference);

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
