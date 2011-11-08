using System;
using System.Collections.Generic;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumTagUsage : TagUsage {

		private Album album;
		private IList<AlbumTagVote> votes = new List<AlbumTagVote>();

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public override IEntryBase Entry {
			get { return Album; }
		}

		public virtual IList<AlbumTagVote> Votes {
			get { return votes; }
			set {
				ParamIs.NotNull(() => value);
				votes = value;
			}
		}

		public override IEnumerable<TagVote> VotesBase {
			get { return Votes; }
		}
	}

}
