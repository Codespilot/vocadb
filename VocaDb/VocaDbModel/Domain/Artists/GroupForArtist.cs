using System;

namespace VocaDb.Model.Domain.Artists {

	public class GroupForArtist {

		private Artist group;
		private Artist member;

		//public DateTime? BeginDate { get; set; }

		//public DateTime? EndDate { get; set; }

		public Artist Group {
			get { return group; }
			set {
				ParamIs.NotNull(() => value);
				group = value;
			}
		}

		public Artist Member {
			get { return member; }
			set {
				ParamIs.NotNull(() => value);
				member = value;
			}
		}
	}
}
