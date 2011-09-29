using System;
using System.Linq;
using log4net;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Model.Service {

	public class ArtistService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(ArtistService));

		private void ArchiveArtist(ISession session, IUserPermissionContext permissionContext, Artist artist) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, permissionContext);
			var archived = ArchivedArtistVersion.Create(artist, agentLoginData);
			session.Save(archived);

		}

		private ArtistDetailsContract[] FindArtists(ISession session, string query, int maxResults) {

			var direct = session.Query<Artist>()
				.Where(s => 
					!s.Deleted &&
					(string.IsNullOrEmpty(query)
						|| s.TranslatedName.English.Contains(query)
						|| s.TranslatedName.Romaji.Contains(query)
						|| s.TranslatedName.Japanese.Contains(query)))
				.Take(maxResults)
				.ToArray();

			var additionalNames = session.Query<ArtistName>()
				.Where(m => m.Value.Contains(query) && !m.Artist.Deleted)
				.Select(m => m.Artist)
				.Distinct()
				.Take(maxResults)
				.ToArray()
				.Where(a => !direct.Contains(a));

			return direct.Concat(additionalNames)
				.Take(maxResults)
				.Select(a => new ArtistDetailsContract(a, PermissionContext.LanguagePreference))
				.ToArray();

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

		public AlbumContract AddAlbum(int artistId, int albumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			return HandleTransaction(session => {
				
				var artist = session.Load<Artist>(artistId);
				var album = session.Load<Album>(albumId);

				artist.AddAlbum(session.Load<Album>(albumId));
				return new AlbumContract(album, PermissionContext.LanguagePreference);

			});

		}

		public AlbumContract AddAlbum(int artistId, string newAlbumName) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				var album = new Album(new TranslatedString(newAlbumName));

				session.Save(album);
				artist.AddAlbum(album);
				session.Update(artist);

				return new AlbumContract(album, PermissionContext.LanguagePreference);

			});

		}

		public ArtistContract Create(string name, IUserPermissionContext permissionContext) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNull(() => permissionContext);

			log.Info("'" + permissionContext.Name + "' creating an artist");

			PermissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			return HandleTransaction(session => {

				var artist = new Artist(new TranslatedString(name));

				session.Save(artist);

				return new ArtistContract(artist, PermissionContext.LanguagePreference);

			});

		}

		public LocalizedStringWithIdContract CreateName(int artistId, string nameVal, ContentLanguageSelection language, IUserPermissionContext permissionContext) {

			ParamIs.NotNullOrEmpty(() => nameVal);
			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);

				var name = artist.CreateName(nameVal, language);
				session.Save(name);
				return new LocalizedStringWithIdContract(name);

			});

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			UpdateEntity<Artist>(id, (session, a) => {

				log.Info(string.Format("'{0}' deleting artist '{1}'", PermissionContext.Name, a.Name));

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();
			                         
			});

		}

		public void DeleteName(int nameId, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);
	
			DeleteEntity<ArtistName>(nameId);

		}

		public WebLinkContract CreateWebLink(int artistId, string description, string url, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);
			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);

				var link = artist.CreateWebLink(description, url );
				session.Save(link);

				return new WebLinkContract(link);

			});

		}

		public void DeleteWebLink(int linkId, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			DeleteEntity<ArtistWebLink>(linkId);

		}

		public ArtistDetailsContract[] FindArtists(string query, int maxResults) {

			return HandleQuery(session => FindArtists(session, query, maxResults));

		}

		public ArtistDetailsContract GetArtistDetails(int id) {

			return HandleQuery(session => new ArtistDetailsContract(session.Load<Artist>(id), PermissionContext.LanguagePreference));

		}

		public ArtistForEditContract GetArtistForEdit(int id) {

			return
				HandleQuery(session =>
					new ArtistForEditContract(session.Load<Artist>(id), PermissionContext.LanguagePreference,
						session.Query<Artist>().Where(a => a.ArtistType == ArtistType.Circle)));

		}

		public PictureDataContract GetArtistPicture(int id) {

			return HandleQuery(session => {

				var artist = session.Load<Artist>(id);

				if (artist.Picture != null)
					return new PictureDataContract(artist.Picture);
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

			log.Info(string.Format("'{0}' updating properties for artist '{1}'", permissionContext.Name, properties.Name));

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			UpdateEntity<Artist>(properties.Id, (session, artist) => {

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

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			UpdateEntity<ArtistName>(nameId, name => name.Language = lang);

		}

		public void UpdateArtistNameValue(int nameId, string val, IUserPermissionContext permissionContext) {
			
			ParamIs.NotNullOrEmpty(() => val);

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			UpdateEntity<ArtistName>(nameId, name => name.Value = val);

		}

		public void UpdateWebLinkDescription(int linkId, string description, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => description);

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			UpdateEntity<ArtistWebLink>(linkId, link => link.Description = description);

		}

		public void UpdateWebLinkUrl(int nameId, string url, IUserPermissionContext permissionContext) {

			ParamIs.NotNullOrEmpty(() => url);

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			UpdateEntity<ArtistWebLink>(nameId, link => link.Url = url);

		}

	}

}
