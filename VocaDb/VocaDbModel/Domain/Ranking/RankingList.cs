using System;
using System.Collections.Generic;
using VocaDb.Model.DataContracts.Ranking;
using VocaDb.Model.Domain.Songs;
using VocaVoter.Model;

namespace VocaDb.Model.Domain.Ranking {

	public class RankingList {

		private string name;
		private IList<SongInRanking> songs = new List<SongInRanking>();

		public RankingList() {
			Description = string.Empty;
			NicoId = string.Empty;
		}

		public RankingList(RankingContract contract) {

			ParamIs.NotNull(() => contract);

			CreateDate = DateTime.Now;
			Description = contract.Description;
			Name = contract.Name;
			NicoId = contract.NicoId;
			WVRId = contract.WVRId;

		}

		public virtual DateTime CreateDate { get; set; }

		public virtual string Description { get; set; }

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {

				ParamIs.NotNullOrEmpty(value, "value");
				name = value;

			}
		}

		public virtual string NicoId { get; set; }

		public virtual int WVRId { get; set; }

		public virtual IList<SongInRanking> Songs {
			get { return songs; }
			set { songs = value; }
		}

		public virtual SongInRanking AddSong(Song song, int sortIndex) {

			ParamIs.NotNull(song, "song");

			var songInPoll = new SongInRanking(this, song, sortIndex);
			Songs.Add(songInPoll);

			return songInPoll;

		}

	}

}
