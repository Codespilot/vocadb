using System.Drawing;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {
	public class AlbumWithCoverPictureContract : AlbumContract {

		public AlbumWithCoverPictureContract(Album album, ContentLanguagePreference languagePreference, Size requestedSize)
			: base(album, languagePreference) {

			CoverPicture = (album.CoverPicture != null ? new PictureContract(album.CoverPicture, requestedSize) : null);

		}

		public PictureContract CoverPicture { get; set; }

	}
}
