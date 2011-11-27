using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Resources;

namespace VocaDb.Model.Service.EntryValidators {

	public static class SongValidator {

		public static ValidationResult Validate(Song song) {

			ParamIs.NotNull(() => song);

			var errors = new List<string>();

			if (song.SongType == SongType.Unspecified)
				errors.Add(SongValidationErrors.NeedType);

			if (!song.Artists.Any())
				errors.Add(SongValidationErrors.NeedArtist);

			if (!song.Names.Names.Any(n => n.Language != ContentLanguageSelection.Unspecified))
				errors.Add(SongValidationErrors.UnspecifiedNames);

			return new ValidationResult(errors);

		}
	
	}

}
