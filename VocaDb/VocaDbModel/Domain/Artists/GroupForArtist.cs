using System;

namespace VocaDb.Model.Domain.Artists {

	public class GroupForArtist {

		private Artist group;
		private Artist member;

		public GroupForArtist() { }

		public GroupForArtist(Artist group, Artist member) {

			Group = group;
			Member = member;

		}

		//public DateTime? BeginDate { get; set; }

		//public DateTime? EndDate { get; set; }

		public virtual Artist Group {
			get { return group; }
			set {
				ParamIs.NotNull(() => value);
				group = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual Artist Member {
			get { return member; }
			set {
				ParamIs.NotNull(() => value);
				member = value;
			}
		}

		public virtual void Delete() {

			Member.AllGroups.Remove(this);

		}

	}
}
