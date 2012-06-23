using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Resources;

namespace VocaDb.Model.Service.EntryValidators {

	public static class AlbumValidator {

		public static ValidationResult Validate(Album album) {

			ParamIs.NotNull(() => album);

			var errors = new List<string>();

			if (album.DiscType == DiscType.Unknown)
				errors.Add(AlbumValidationErrors.NeedType);

			if (album.Artists.All(a => a.Artist == null))
				errors.Add(AlbumValidationErrors.NeedArtist);

			if (album.Names.Names.All(n => n.Language == ContentLanguageSelection.Unspecified))
				errors.Add(AlbumValidationErrors.UnspecifiedNames);

			if (album.OriginalReleaseDate.IsEmpty)
				errors.Add(AlbumValidationErrors.NeedReleaseYear);

			if (!album.Songs.Any())
				errors.Add(AlbumValidationErrors.NeedTracks);

			return new ValidationResult(errors);

		}

	}

}
