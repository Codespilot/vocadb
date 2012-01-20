using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using NHibernate;
using VocaDb.Model.DataContracts.Ranking;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Ranking;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Rankings;

namespace VocaDb.Model.Service {

	public class RankingService : ServiceBase {

		public RankingService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory) {
		}

		public int CreateSongListFromWVR(string url) {

			PermissionContext.VerifyPermission(PermissionFlags.EditProfile);

			var parsed = new NNDWVRParser().GetSongs(url);

			return HandleTransaction(session => {

				var user = GetLoggedUser(session);
				var list = new SongList("Weekly Vocaloid ranking #" + parsed.WVRId, user) { Description = parsed.Name };
				session.Save(list);

				foreach (var entry in parsed.Songs) {

					var song = session.Query<PVForSong>()
						.Where(p => p.Service == PVService.NicoNicoDouga && p.PVId == entry.NicoId)
						.First().Song;

					session.Save(list.AddSong(song));

				}

				AuditLog(string.Format("created {0}", EntryLinkFactory.CreateEntryLink(list)), session, user);
				return list.Id;

			});

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

		public WVRListResult ParseWVRList(string url) {

			var parsed = new NNDWVRParser().GetSongs(url);

			return HandleQuery(session => {

				var songs = new List<WVRListEntryResult>();

				foreach (var entry in parsed.Songs) {

					var pv = session.Query<PVForSong>()
						.Where(p => p.Service == PVService.NicoNicoDouga && p.PVId == entry.NicoId)
						.FirstOrDefault();

					var song = pv != null ? new SongWithAdditionalNamesContract(pv.Song, PermissionContext.LanguagePreference) : null;

					songs.Add(new WVRListEntryResult(entry.NicoId, entry.SortIndex, entry.Name, entry.Url, song));

				}

				return new WVRListResult(parsed.Name, parsed.WVRId, songs);

			});

		}

	}

	public class WVRListResult {

		public WVRListResult(string name, int wvrNumber, IEnumerable<WVRListEntryResult> entries) {

			Name = name;
			WVRNumber = wvrNumber;
			Entries = entries.ToArray();

		}

		public WVRListEntryResult[] Entries { get; set; }

		public string Name { get; set; }

		public int WVRNumber { get; set; }

		public bool IsIncomplete {
			get {
				return Entries.Any(e => e.Song == null);
			}
		}

	}

	public class WVRListEntryResult {

		public WVRListEntryResult(string nicoId, int order, string name, string url, SongWithAdditionalNamesContract song) {

			NicoId = nicoId;
			Order = order;
			Name = name;
			Url = url;
			Song = song;

		}

		public string Name { get; set; }

		public string NicoId { get; set; }

		public int Order { get; set; }

		public SongWithAdditionalNamesContract Song { get; set; }

		public string Url { get; set; }

	}

}
