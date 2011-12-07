using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using System.Drawing;
using VocaDb.Model.Helpers;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Model.DataContracts.UseCases {

	public class EntryForPictureDisplayContract {

		public static EntryForPictureDisplayContract Create(Album album, ContentLanguagePreference languagePreference, Size requestedSize) {

			ParamIs.NotNull(() => album);

			var name = album.TranslatedName[languagePreference];
			var pic = (album.CoverPicture != null ? new PictureContract(album.CoverPicture, requestedSize) : null);

			return new EntryForPictureDisplayContract(EntryType.Album, album.Id, name, album.Version, pic);

		}

		public static EntryForPictureDisplayContract Create(ArchivedAlbumVersion archivedVersion) {

			ParamIs.NotNull(() => archivedVersion);

			var data = XmlHelper.DeserializeFromXml<ArchivedAlbumContract>(archivedVersion.Data);
			var name = data.TranslatedName.Default;
			PictureContract pic = null;

			if (archivedVersion.CoverPicture != null) {
				pic = new PictureContract(archivedVersion.CoverPicture, Size.Empty);
			} else {

				var versionWithPic = archivedVersion.Album.GetLatestVersionWithField(AlbumEditableFields.Cover, archivedVersion.Version);

				if (versionWithPic != null && versionWithPic.CoverPicture != null)
					pic = new PictureContract(versionWithPic.CoverPicture, Size.Empty);

			}

			return new EntryForPictureDisplayContract(EntryType.Album, data.Id, name, archivedVersion.Version, pic);

		}

		public static EntryForPictureDisplayContract Create(Artist artist, ContentLanguagePreference languagePreference, Size requestedSize) {

			ParamIs.NotNull(() => artist);

			var name = artist.TranslatedName[languagePreference];
			var pic = (artist.Picture != null ? new PictureContract(artist.Picture, requestedSize) : null);

			return new EntryForPictureDisplayContract(EntryType.Artist, artist.Id, name, artist.Version, pic);

		}

		public static EntryForPictureDisplayContract Create(ArchivedArtistVersion archivedVersion) {

			ParamIs.NotNull(() => archivedVersion);

			var data = XmlHelper.DeserializeFromXml<ArchivedArtistContract>(archivedVersion.Data);
			var name = data.TranslatedName.Default;
			var pic = (archivedVersion.Picture != null ? new PictureContract(archivedVersion.Picture, Size.Empty) : null);

			return new EntryForPictureDisplayContract(EntryType.Artist, data.Id, name, archivedVersion.Version, pic);

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
