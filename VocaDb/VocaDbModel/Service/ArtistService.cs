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
using VocaDb.Model.Service.Security;

namespace VocaDb.Model.Service {

	public class ArtistService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(ArtistService));

		private ArtistDetailsContract[] FindArtists(ISession session, string query, int maxResults) {

			var direct = session.Query<Artist>()
				.Where(s => string.IsNullOrEmpty(query)
					|| s.TranslatedName.English.Contains(query)
						|| s.TranslatedName.Romaji.Contains(query)
							|| s.TranslatedName.Japanese.Contains(query))
				.OrderBy(s => s.TranslatedName.Japanese)
				.Take(maxResults)
				.ToArray();

			var additionalNames = session.Query<ArtistMetadataEntry>()
				.Where(m => m.MetadataType == ArtistMetadataType.AlternateName
					&& m.Value.Contains(query))
				.Select(m => m.Artist)
				.Distinct()
				.Take(maxResults)
				.ToArray()
				.Where(a => !direct.Contains(a));

			return direct.Concat(additionalNames)
				.Take(maxResults)
				.Select(a => new ArtistDetailsContract(a))
				.ToArray();

		}

		private T[] GetArtists<T>(Func<Artist, T> func) {

			return HandleQuery(session => session.Query<Artist>()
				.ToArray()
				.OrderBy(a => a.Name)
				.Select(func)
				.ToArray());

		}

		public ArtistService(ISessionFactory sessionFactory)
			: base(sessionFactory) {}

		public ArtistContract Create(string name, IUserPermissionContext permissionContext) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			return HandleTransaction(session => {

				var artist = new Artist(new TranslatedString(name));

				session.Save(artist);

				return new ArtistContract(artist);

			});

		}

		public ArtistDetailsContract[] FindArtists(string query, int maxResults) {

			return HandleQuery(session => FindArtists(session, query, maxResults));

		}

		public ArtistDetailsContract GetArtistDetails(int id) {

			return HandleQuery(session => new ArtistDetailsContract(session.Load<Artist>(id)));

		}

		public ArtistForEditContract GetArtistForEdit(int id) {

			return
				HandleQuery(session =>
					new ArtistForEditContract(session.Load<Artist>(id),
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

			return GetArtists(a => new ArtistContract(a));

		}

		public ArtistWithAdditionalNamesContract[] GetArtistsWithAdditionalNames() {

			return GetArtists(a => new ArtistWithAdditionalNamesContract(a));

		}

		public ArtistContract[] GetCircles() {

			return HandleQuery(session => session.Query<Artist>()
				.Where(a => a.ArtistType == ArtistType.Circle)
				.ToArray()
				.OrderBy(a => a.Name)
				.Select(a => new ArtistContract(a))
				.ToArray());

		}

		public void UpdateBasicProperties(ArtistDetailsContract properties, PictureDataContract pictureData, IUserPermissionContext permissionContext) {
			
			ParamIs.NotNull(() => properties);
			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageArtists);

			UpdateEntity<Artist>(properties.Id, (session, artist) => {

				artist.ArtistType = properties.ArtistType;
				artist.Circle = (properties.Circle != null ? session.Load<Artist>(properties.Circle.Id) : null);
				artist.Description = properties.Description;
				artist.TranslatedName.CopyFrom(properties.TranslatedName);

				if (pictureData != null) {
					artist.Picture = new PictureData(pictureData);
				}

			});

		}

	}

}
