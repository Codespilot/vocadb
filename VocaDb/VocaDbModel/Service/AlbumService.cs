using System.Linq;
using log4net;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service {

	public class AlbumService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(AlbumService));

		private void Archive(ISession session, Album artist) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			//var archived = ArchivedArtistVersion.Create(artist, agentLoginData);
			//session.Save(archived);

		}

		public AlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) : base(sessionFactory, permissionContext) {}

		public AlbumContract[] Find(string query, int maxResults) {

			return HandleQuery(session => {

				var direct = session.Query<Album>()
					.Where(s => string.IsNullOrEmpty(query)
						|| s.TranslatedName.English.Contains(query)
							|| s.TranslatedName.Romaji.Contains(query)
								|| s.TranslatedName.Japanese.Contains(query))
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
				.ToArray()
				.OrderBy(a => a.Name)
				.Select(a => new AlbumContract(a, PermissionContext.LanguagePreference))
				.ToArray());

		}

		public AlbumForEditContract UpdateBasicProperties(AlbumForEditContract properties, PictureDataContract pictureData) {

			ParamIs.NotNull(() => properties);

			log.Info(string.Format("'{0}' updating properties for album '{1}'", PermissionContext.Name, properties.Name));

			PermissionContext.VerifyPermission(PermissionFlags.ManageAlbums);

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

	}

}
