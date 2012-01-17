using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.DataContracts.Activityfeed {

	public class ActivityEntryContract {

		public ActivityEntryContract(ActivityEntry entry) {

			ParamIs.NotNull(() => entry);

			Author = new UserContract(entry.Author);
			CreateDate = entry.CreateDate;
			Sticky = entry.Sticky;

		}

		public ActivityEntryContract(EntryEditedEntry entry, string displayName, string additionalNames)
			: this((ActivityEntry)entry) {

			EntryEditedProperties = new EntryEditedPropertiesContract(entry, displayName, additionalNames);

		}

		public ActivityEntryContract(PlaintextEntry entry)
			: this((ActivityEntry)entry) {

			PlaintextProperties = new PaintextEntryPropertiesContract(entry);

		}

		public UserContract Author { get; set; }

		public DateTime CreateDate { get; set; }

		public EntryEditedPropertiesContract EntryEditedProperties { get; set; }

		public PaintextEntryPropertiesContract PlaintextProperties { get; set; }

		public bool Sticky { get; set; }

	}

	public class EntryEditedPropertiesContract {

		public EntryEditedPropertiesContract(EntryEditedEntry entry, string displayName, string additionalNames) {

			ParamIs.NotNull(() => entry);

			EntryRef = new EntryRefContract(entry.EntryRef);
			DisplayName = displayName;
			AdditionalNames = additionalNames;

		}

		public string AdditionalNames { get; set; }

		public string DisplayName { get; set; }

		public EntryRefContract EntryRef { get; set; }

	}

	public class PaintextEntryPropertiesContract {

		public PaintextEntryPropertiesContract(PlaintextEntry entry) {

			ParamIs.NotNull(() => entry);

			Text = entry.Text;

		}

		public string Text { get; set; }

	}

}
