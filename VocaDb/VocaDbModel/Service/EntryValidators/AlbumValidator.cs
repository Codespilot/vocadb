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

			if (!album.Artists.Any())
				errors.Add(AlbumValidationErrors.NeedArtist);

			if (album.Names.Names.Any(n => n.Language == ContentLanguageSelection.Unspecified))
				errors.Add(AlbumValidationErrors.UnspecifiedNames);

			if (!album.Songs.Any())
				errors.Add(AlbumValidationErrors.NeedTracks);

			return new ValidationResult(errors);

		}

	}

}
