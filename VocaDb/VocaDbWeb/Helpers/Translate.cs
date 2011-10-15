using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Resources;

namespace VocaDb.Web.Helpers {

	public static class Translate {

		public static string ArtistTypeName(ArtistType artistType) {

			return ArtistTypeNames.ResourceManager.GetString(artistType.ToString());

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