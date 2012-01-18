using System;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.Activityfeed {

	public class ActivityEntryContract {

		public ActivityEntryContract(ActivityEntry entry, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => entry);

			var name = entry.EntryNames.GetEntryName(languagePreference);

			AdditionalNames = name.AdditionalNames;
			Author = new UserContract(entry.Author);
			CreateDate = entry.CreateDate;
			DisplayName = name.DisplayName;
			EditEvent = entry.EditEvent;
			EntryRef = new EntryRefContract(entry.EntryBase);

		}

		public string AdditionalNames { get; set; }

		public UserContract Author { get; set; }

		public DateTime CreateDate { get; set; }

		public string DisplayName { get; set; }

		public EntryEditEvent EditEvent { get; set; }

		public EntryRefContract EntryRef { get; set; }

	}

	/*public class EntryEditedPropertiesContract {

		public EntryEditedPropertiesContract(EntryEditedEntry entry, EntryName name) {

			ParamIs.NotNull(() => entry);
			ParamIs.NotNull(() => name);

			EditEvent = entry.EditEvent;
			EntryRef = new EntryRefContract(entry.EntryRef);
			DisplayName = name.DisplayName;
			AdditionalNames = name.DisplayName;

		}


	}

	public class PaintextEntryPropertiesContract {

		public PaintextEntryPropertiesContract(PlaintextEntry entry) {

			ParamIs.NotNull(() => entry);

			Text = entry.Text;

		}

		public string Text { get; set; }

	}*/

}
