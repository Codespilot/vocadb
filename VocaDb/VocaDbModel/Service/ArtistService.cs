using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Service {

	public class ArtistService : ServiceBase {

		private ArtistDetailsContract[] FindArtists(ISession session, string query, int maxResults) {

			var direct = session.Query<Artist>()
				.Where(s => string.IsNullOrEmpty(query)
					|| s.LocalizedName.English.Contains(query)
						|| s.LocalizedName.Romaji.Contains(query)
							|| s.LocalizedName.Japanese.Contains(query))
				.OrderBy(s => s.LocalizedName.Japanese)
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

		public ArtistContract Create(string name) {

			return HandleTransaction(session => {

				var artist = new Artist(new LocalizedString(name));

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

		public void UpdateBasicProperties(ArtistDetailsContract properties) {
			
			ParamIs.NotNull(() => properties);

			HandleTransaction(session => {

				var artist = session.Load<Artist>(properties.Id);
				artist.Description = properties.Description;
				artist.LocalizedName.CopyFrom(properties.LocalizedName);

			});

		}

	}

}
