using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Ranking;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service {

	public class SongService : ServiceBase {

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

		public SongService(ISessionFactory sessionFactory)
			: base(sessionFactory) {

		}

		public SongContract CreateSong(CreateSongContract contract) {

			return HandleTransaction(session => {

				if (!string.IsNullOrEmpty(contract.BasicData.NicoId)) {

					var existing = session.Query<Song>().FirstOrDefault(s => s.NicoId == contract.BasicData.NicoId);

					if (existing != null) {
						throw new ServiceException("Song with NicoId '" + contract.BasicData.NicoId + "' has already been added");
					}			
		
				}

				var song = new Song(new LocalizedString(contract.BasicData.Name), contract.BasicData.NicoId);

				if (contract.AlbumId != null)
					song.AddAlbum(session.Load<Album>(contract.AlbumId.Value), 0);

				if (contract.PerformerId != null)
					song.AddArtist(session.Load<Artist>(contract.PerformerId.Value));

				if (contract.ProducerId != null)
					song.AddArtist(session.Load<Artist>(contract.ProducerId.Value));

				song.UpdateArtistString();
				session.Save(song);

				return new SongContract(song);

			});

		}

		public ArtistDetailsContract[] FindArtists(string query, int maxResults) {

			return HandleQuery(session => FindArtists(session, query, maxResults));

		}

		public AlbumContract[] GetAlbums() {

			return HandleQuery(session => session.Query<Album>()
				.ToArray()
				.OrderBy(a => a.Name)
				.Select(a => new AlbumContract(a))
				.ToArray());

		}

		public ArtistContract[] GetArtists() {

			return GetArtists(a => new ArtistContract(a));

		}

		public ArtistWithAdditionalNamesContract[] GetArtistsWithAdditionalNames() {

			return GetArtists(a => new ArtistWithAdditionalNamesContract(a));

		}

		public int GetSongCount(string filter) {

			return HandleQuery(session => session.Query<Song>()
				.Where(s => string.IsNullOrEmpty(filter) || s.LocalizedName.Japanese.Contains(filter))
				.Count());

		}

		public SongDetailsContract GetSongDetails(int songId) {

			return HandleQuery(session => {

				var song = session.Load<Song>(songId);
				var songInPolls = session.Query<SongInRanking>().Where(s => s.Song == song).ToArray();

				return new SongDetailsContract(song, songInPolls);

			});

		}

		public SongContract[] GetSongs(string filter, int start, int count) {

			return HandleQuery(session => session.Query<Song>()
				.Where(s => string.IsNullOrEmpty(filter) 
					|| s.LocalizedName.Japanese.Contains(filter) 
					|| s.NicoId == filter)
				.OrderBy(s => s.LocalizedName.Japanese)
				.Skip(start)
				.Take(count)
				.ToArray()
				.Select(s => new SongContract(s))
				.ToArray());

		}

	}

}
