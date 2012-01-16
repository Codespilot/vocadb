using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Activityfeed {

	public class ActivityEntry {

		private User author;

		public virtual DateTime CreateDate { get; set; }

		public virtual ActivityEntryType EntryType { get; set; }

		public virtual bool Sticky { get; set; }

	}
}
