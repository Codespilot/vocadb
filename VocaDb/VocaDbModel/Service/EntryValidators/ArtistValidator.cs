using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Resources;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Service.EntryValidators {

	public static class ArtistValidator {

		public static ValidationResult Validate(Artist artist) {

			ParamIs.NotNull(() => artist);

			var errors = new List<string>();

			if (artist.ArtistType == ArtistType.Unknown)
				errors.Add(ArtistValidationErrors.NeedType);

			if (!artist.Names.Names.Any(n => n.Language != ContentLanguageSelection.Unspecified))
				errors.Add(ArtistValidationErrors.UnspecifiedNames);

			if (string.IsNullOrWhiteSpace(artist.Description) && !artist.WebLinks.Any())
				errors.Add(ArtistValidationErrors.NeedReferences);

			return new ValidationResult(errors);

		}


	}

}
