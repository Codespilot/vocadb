using System;
using System.Linq;
using log4net;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.DataContracts.Albums;
using System.Drawing;

namespace VocaDb.Model.Service {

	public class ArtistService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(ArtistService));

		private void ArchiveArtist(ISession session, IUserPermissionContext permissionContext, Artist artist) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, permissionContext);
			var archived = ArchivedArtistVersion.Create(artist, agentLoginData);
			session.Save(archived);

		}

		private PartialFindResult<ArtistWithAdditionalNamesContract> FindArtists(ISession session, string query, int start, int maxResults, bool getTotalCount) {

			if (string.IsNullOrWhiteSpace(query)) {

				var artists = session.Query<Artist>()
					.Where(s => !s.Deleted)
					.OrderBy(s => s.TranslatedName.Romaji)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				var contracts = artists.Select(s => new ArtistWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
					.ToArray();

				var count = (getTotalCount ? GetArtistCount(session, query) : 0);

				return new PartialFindResult<ArtistWithAdditionalNamesContract>(contracts, count);

			} else {

				var direct = session.Query<Artist>()
					.Where(s =>
						!s.Deleted &&
						(string.IsNullOrEmpty(query)
							|| s.TranslatedName.English.Contains(query)
							|| s.TranslatedName.Romaji.Contains(query)
							|| s.TranslatedName.Japanese.Contains(query)))
					.OrderBy(s => s.TranslatedName.Romaji)
					.Take(maxResults)
					.ToArray();

				var additionalNames = session.Query<ArtistName>()
					.Where(m => m.Value.Contains(query) && !m.Artist.Deleted)
					.Select(m => m.Artist)
					.OrderBy(s => s.TranslatedName.Romaji)
					.Distinct()
					.Take(maxResults)
					.ToArray()
					.Where(a => !direct.Contains(a));

				var contracts = direct.Concat(additionalNames)
					.Skip(start)
					.Take(maxResults)
					.Select(a => new ArtistWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
					.ToArray();

				var count = (getTotalCount ? GetArtistCount(session, query) : 0);

				return new PartialFindResult<ArtistWithAdditionalNamesContract>(contracts, count);

			}

		}

		private int GetArtistCount(ISession session, string query) {

			if (string.IsNullOrWhiteSpace(query)) {

				return session.Query<Artist>()
					.Where(s => !s.Deleted)
					.Count();

			}

			var direct = session.Query<Artist>()
				.Where(s =>
					!s.Deleted &&
					(string.IsNullOrEmpty(query)
						|| s.TranslatedName.English.Contains(query)
						|| s.TranslatedName.Romaji.Contains(query)
						|| s.TranslatedName.Japanese.Contains(query)))
				.ToArray();

			var additionalNames = session.Query<ArtistName>()
				.Where(m => m.Value.Contains(query) && !m.Artist.Deleted)
				.Select(m => m.Artist)
				.Distinct()
				.ToArray()
				.Where(a => !direct.Contains(a))
				.ToArray();

			return direct.Count() + additionalNames.Count();

		}

		private T[] GetArtists<T>(Func<Artist, T> func) {

			return HandleQuery(session => session
				.Query<Artist>()
				.Where(a => !a.Deleted)
				.ToArray()
				.OrderBy(a => a.Name)
				.Select(func)
				.ToArray());

		}

		public ArtistService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) {}

		public ArtistForAlbumContract AddAlbum(int artistId, int albumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {
				
				var artist = session.Load<Artist>(artistId);
				var album = session.Load<Album>(albumId);

				AuditLog("adding " + album + " for " + artist);

				var artistForAlbum = artist.AddAlbum(album);

				album.UpdateArtistString();
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

		public ArtistForAlbumContract AddAlbum(int artistId, string newAlbumName) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				AuditLog("creating a new album '" + newAlbumName + "' for " + artist);

				var album = new Album(new TranslatedString(newAlbumName));

				session.Save(album);
				var artistForAlbum = artist.AddAlbum(album);
				session.Update(artist);

				album.UpdateArtistString();
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

		public ArtistContract Create(string name, IUserPermissionContext permissionContext) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNull(() => permissionContext);

			AuditLog("creating a new artist with name '" + name + "'");

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var artist = new Artist(new TranslatedString(name));

				session.Save(artist);

				return new ArtistContract(artist, PermissionContext.LanguagePreference);

			});

		}

		public LocalizedStringWithIdContract CreateName(int artistId, string nameVal, ContentLanguageSelection language, IUserPermissionContext permissionContext) {

			ParamIs.NotNullOrEmpty(() => nameVal);
			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				AuditLog("creating a new name '" + nameVal + "' for " + artist);

				var name = artist.CreateName(nameVal, language);
				session.Save(name);
				return new LocalizedStringWithIdContract(name);

			});

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<Artist>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", a));

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();
			                         
			});

		}

		public void DeleteName(int nameId, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageDatabase);
	
			DeleteEntity<ArtistName>(nameId);

		}

		public WebLinkContract CreateWebLink(int artistId, string description, string url, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);
			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				AuditLog("creating web link for " + artist);

				var link = artist.CreateWebLink(description, url );
				session.Save(link);

				return new WebLinkContract(link);

			});

		}

		public void DeleteWebLink(int linkId, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			DeleteEntity<ArtistWebLink>(linkId);

		}

		public PartialFindResult<ArtistWithAdditionalNamesContract> FindArtists(string query, int start, int maxResults, bool getTotalCount = false) {

			return HandleQuery(session => FindArtists(session, query, start, maxResults, getTotalCount));

		}

		public ArtistDetailsContract GetArtistDetails(int id) {

			return HandleQuery(session => {
			                   	
				var contract = new ArtistDetailsContract(session.Load<Artist>(id), PermissionContext.LanguagePreference);
				contract.LatestSongs = session.Query<ArtistForSong>().Where(s => s.Artist.Id == id).Select(s => s.Song)
					.OrderByDescending(s => s.CreateDate).Take(20).ToArray()
					.Select(s => new SongContract(s, PermissionContext.LanguagePreference)).ToArray();

				return contract;

			});

		}

		public ArtistForEditContract GetArtistForEdit(int id) {

			return
				HandleQuery(session =>
					new ArtistForEditContract(session.Load<Artist>(id), PermissionContext.LanguagePreference,
						session.Query<Artist>().Where(a => a.ArtistType == ArtistType.Circle)));

		}

		/// <summary>
		/// Gets the picture for a <see cref="Artist"/>.
		/// </summary>
		/// <param name="id">Artist Id.</param>
		/// <param name="requestedSize">Requested size. If Empty, original size will be returned.</param>
		/// <returns>Data contract for the picture. Can be null if there is no picture.</returns>
		public PictureContract GetArtistPicture(int id, Size requestedSize) {

			return HandleQuery(session => {

				var artist = session.Load<Artist>(id);

				if (artist.Picture != null)
					return new PictureContract(artist.Picture, requestedSize);
				else
					return null;

			});

		}

		public ArtistContract[] GetArtists() {

			return GetArtists(a => new ArtistContract(a, PermissionContext.LanguagePreference));

		}

		public ArtistWithAdditionalNamesContract[] GetArtistsWithAdditionalNames() {

			return GetArtists(a => new ArtistWithAdditionalNamesContract(a, PermissionContext.LanguagePreference));

		}

		public ArtistContract[] GetCircles() {

			return HandleQuery(session => session.Query<Artist>()
				.Where(a => !a.Deleted && a.ArtistType == ArtistType.Circle)
				.ToArray()
				.Select(a => new ArtistContract(a, PermissionContext.LanguagePreference))
				.ToArray());

		}

		public void UpdateBasicProperties(ArtistDetailsContract properties, PictureDataContract pictureData, IUserPermissionContext permissionContext) {
			
			ParamIs.NotNull(() => properties);
			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<Artist>(properties.Id, (session, artist) => {

				AuditLog(string.Format("updating properties for {0}", artist));
				ArchiveArtist(session, permissionContext, artist);

				artist.ArtistType = properties.ArtistType;
				artist.Circle = (properties.Circle != null ? session.Load<Artist>(properties.Circle.Id) : null);
				artist.Description = properties.Description;
				artist.TranslatedName.CopyFrom(properties.TranslatedName);

				if (pictureData != null) {
					artist.Picture = new PictureData(pictureData);
				}

			});

		}

		public void UpdateArtistNameLanguage(int nameId, ContentLanguageSelection lang, IUserPermissionContext permissionContext) {

			permissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<ArtistName>(nameId, name => name.Language = lang);

		}

		public void UpdateArtistNameValue(int nameId, string val, IUserPermissionContext permissionContext) {
			
			ParamIs.NotNullOrEmpty(() => val);

			permissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<ArtistName>(nameId, name => name.Value = val);

		}

		public void UpdateWebLinkDescription(int linkId, string description, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => description);

			permissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<ArtistWebLink>(linkId, link => link.Description = description);

		}

		public void UpdateWebLinkUrl(int nameId, string url, IUserPermissionContext permissionContext) {

			ParamIs.NotNullOrEmpty(() => url);

			permissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<ArtistWebLink>(nameId, link => link.Url = url);

		}

	}

}
