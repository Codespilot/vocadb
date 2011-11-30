using System.Linq;
using NHibernate.Linq;
using NHibernate;
using VocaDb.Model.DataContracts.Ranking;
using VocaDb.Model.Domain.Ranking;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service {

	public class RankingService : ServiceBase {

		public RankingService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory) {
		}

		public void CreateWVRPoll(RankingContract contract) {

			HandleTransaction(session => {

				var poll = new RankingList(contract);

				foreach (var songContract in contract.Songs) {

					var c = songContract;
					var song = session.Query<Song>().FirstOrDefault(s => s.NicoId == c.NicoId);

					if (song == null) {
						song = new Song(songContract);
						session.Save(song);
					}

					poll.AddSong(song, songContract.SortIndex);
					session.Save(poll);
					
				}
			});

		}

		public RankingContract GetPoll(int id) {

			return HandleQuery(session => new RankingContract(session.Load<RankingList>(id)));

		}

		public RankingContract[] GetPolls() {

			return HandleQuery(session => session.Query<RankingList>()
				.OrderByDescending(m => m.CreateDate)
				.ToArray()
				.Select(m => new RankingContract(m)).ToArray());

		}

		public SongInRankingContract GetSongInPoll(int id) {

			return HandleQuery(session => new SongInRankingContract(session.Load<SongInRanking>(id)));

		}

	}
}
