using System;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Activityfeed {

	public abstract class ActivityEntry {

		private User author;

		protected ActivityEntry() {
			CreateDate = DateTime.Now;
		}

		protected ActivityEntry(User author, bool sticky)	
			: this() {

			Author = author;
			Sticky = sticky;

		}

		public virtual User Author {
			get { return author; }
			set {
				ParamIs.NotNull(() => value);
				author = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public abstract ActivityEntryType EntryType { get; }

		public virtual int Id { get; set; }

		public virtual bool Sticky { get; set; }

		public abstract void Accept(IActivityEntryVisitor visitor);

		public virtual bool IsDuplicate(ActivityEntry entry) {
			return false;
		}

	}

}
