using System;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.DataContracts.Activityfeed {

	public class ActivityEntryContract {

		public ActivityEntryContract(ActivityEntry entry) {

			ParamIs.NotNull(() => entry);

			Author = new UserContract(entry.Author);
			CreateDate = entry.CreateDate;
			EntryType = entry.EntryType;
			Sticky = entry.Sticky;

		}

		public ActivityEntryContract(EntryEditedEntry entry, EntryName name)
			: this(entry) {

			EntryEditedProperties = new EntryEditedPropertiesContract(entry, name);

		}

		public ActivityEntryContract(PlaintextEntry entry)
			: this((ActivityEntry)entry) {

			PlaintextProperties = new PaintextEntryPropertiesContract(entry);

		}

		public UserContract Author { get; set; }

		public DateTime CreateDate { get; set; }

		public EntryEditedPropertiesContract EntryEditedProperties { get; set; }

		public ActivityEntryType EntryType { get; set; }

		public PaintextEntryPropertiesContract PlaintextProperties { get; set; }

		public bool Sticky { get; set; }

	}

	public class EntryEditedPropertiesContract {

		public EntryEditedPropertiesContract(EntryEditedEntry entry, EntryName name) {

			ParamIs.NotNull(() => entry);
			ParamIs.NotNull(() => name);

			EditEvent = entry.EditEvent;
			EntryRef = new EntryRefContract(entry.EntryRef);
			DisplayName = name.DisplayName;
			AdditionalNames = name.DisplayName;

		}

		public string AdditionalNames { get; set; }

		public string DisplayName { get; set; }

		public EntryEditEvent EditEvent { get; set; }

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
