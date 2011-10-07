using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Helpers {

	public static class Translate {

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