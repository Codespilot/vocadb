using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using System.Drawing;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.UseCases {

	public class EntryForPictureDisplayContract {

		public static EntryForPictureDisplayContract Create(Album album, ContentLanguagePreference languagePreference, Size requestedSize) {

			ParamIs.NotNull(() => album);

			var name = album.TranslatedName[languagePreference];
			var pic = (album.CoverPicture != null ? new PictureContract(album.CoverPicture, requestedSize) : null);

			return new EntryForPictureDisplayContract(EntryType.Album, album.Id, name, album.Version, pic);

		}

		public static EntryForPictureDisplayContract Create(ArchivedAlbumVersion archivedVersion, 
			ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => archivedVersion);

			var name = archivedVersion.Album.TranslatedName[languagePreference];
			var versionWithPic = archivedVersion.GetLatestVersionWithField(AlbumEditableFields.Cover);
			PictureContract pic = null;

			if (versionWithPic != null && versionWithPic.CoverPicture != null)
				pic = new PictureContract(versionWithPic.CoverPicture, Size.Empty);

			return new EntryForPictureDisplayContract(
				EntryType.Album, archivedVersion.Album.Id, name, archivedVersion.Version, pic);

		}

		public static EntryForPictureDisplayContract Create(Artist artist, ContentLanguagePreference languagePreference, Size requestedSize) {

			ParamIs.NotNull(() => artist);

			var name = artist.TranslatedName[languagePreference];
			var pic = (artist.Picture != null ? new PictureContract(artist.Picture, requestedSize) : null);

			return new EntryForPictureDisplayContract(EntryType.Artist, artist.Id, name, artist.Version, pic);

		}

		public static EntryForPictureDisplayContract Create(ArchivedArtistVersion archivedVersion, 
			ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => archivedVersion);

			var name = archivedVersion.Artist.TranslatedName[languagePreference];
			var versionWithPic = archivedVersion.GetLatestVersionWithField(ArtistEditableFields.Picture);
			PictureContract pic = null;

			if (versionWithPic != null && versionWithPic.Picture != null)
				pic = new PictureContract(versionWithPic.Picture, Size.Empty);

			return new EntryForPictureDisplayContract(EntryType.Artist, archivedVersion.Artist.Id, name, archivedVersion.Version, pic);

		}

		public EntryForPictureDisplayContract(EntryType entryType, int entryId, string name, int version, PictureContract pictureContract) {

			EntryType = entryType;
			EntryId = entryId;
			Name = name;
			Version = version;
			Picture = pictureContract;

		}

		public int EntryId { get; set; }

		public EntryType EntryType { get; set; }

		public string Name { get; set; }

		public PictureContract Picture { get; set; }

		public int Version { get; set; }

	}
}
