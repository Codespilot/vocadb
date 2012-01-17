using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Activityfeed {

	public abstract class ActivityEntry {

		private User author;

		public ActivityEntry() {
			CreateDate = DateTime.Now;
		}

		public ActivityEntry(User author, bool sticky)	
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

	}

}
