﻿using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Resources;
using VocaDb.Model.Domain.Globalization;
using Resources;

namespace VocaDb.Web.Helpers {

	public static class Translate {

		public static string ArtistTypeName(ArtistType artistType) {

			return ArtistTypeNames.ResourceManager.GetString(artistType.ToString());

		}

		public static string ContentLanguagePreferenceName(ContentLanguagePreference languagePreference) {

			return ContentLanguageSelectionNames.ResourceManager.GetString(languagePreference.ToString());

		}

		public static string ContentLanguageSelectionName(ContentLanguageSelection languageSelection) {

			return ContentLanguageSelectionNames.ResourceManager.GetString(languageSelection.ToString());

		}

		public static string MediaTypeName(MediaType mediaType) {

			switch (mediaType) {
				case MediaType.DigitalDownload:
					return "Digital download";

				case MediaType.PhysicalDisc:
					return "Physical disc";

				default:
					return "Other";
			}

		}

	}

}