using System;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.DataContracts.Activityfeed {

	public class ActivityEntryContract {

		private string GetArtistString(IEntryBase entry, ContentLanguagePreference languagePreference) {

			if (entry is Album)
				return ((Album)entry).ArtistString[languagePreference];
			else if (entry is Song)
				return ((Song)entry).ArtistString[languagePreference];
			else
				return null;

		}

		public ActivityEntryContract(ActivityEntry entry, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => entry);

			var name = entry.EntryNames.GetEntryName(languagePreference);

			AdditionalNames = name.AdditionalNames;
			ArtistString = GetArtistString(entry.EntryBase, languagePreference);
			Author = new UserContract(entry.Author);
			CreateDate = entry.CreateDate;
			DisplayName = name.DisplayName;
			EditEvent = entry.EditEvent;
			EntryRef = new EntryRefContract(entry.EntryBase);

		}

		public ActivityEntryContract(ArchivedObjectVersion entry, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => entry);

			var name = entry.EntryBase.Names.GetEntryName(languagePreference);

			AdditionalNames = name.AdditionalNames;
			ArtistString = GetArtistString(entry.EntryBase, languagePreference);
			Author = new UserContract(entry.Author);
			CreateDate = entry.Created;
			DisplayName = name.DisplayName;
			EditEvent = entry.EditEvent;
			EntryRef = new EntryRefContract(entry.EntryBase);

		}

		public string AdditionalNames { get; set; }

		public string ArtistString { get; set; }

		public UserContract Author { get; set; }

		public DateTime CreateDate { get; set; }

		public string DisplayName { get; set; }

		public EntryEditEvent EditEvent { get; set; }

		public EntryRefContract EntryRef { get; set; }

	}

}
