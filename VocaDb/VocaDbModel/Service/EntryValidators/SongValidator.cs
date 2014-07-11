using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Helpers;
using VocaDb.Model.Resources;

namespace VocaDb.Model.Service.EntryValidators {

	[Obsolete("Moved to client side")]
	public static class SongValidator {

		public static ValidationResult Validate(Song song) {

			ParamIs.NotNull(() => song);

			var errors = new List<string>();

			if (song.SongType == SongType.Unspecified)
				errors.Add(SongValidationErrors.NeedType);

			if (song.Artists.All(a => a.Artist == null))
				errors.Add(SongValidationErrors.NeedArtist);

			if (song.Names.Names.All(n => n.Language == ContentLanguageSelection.Unspecified))
				errors.Add(SongValidationErrors.UnspecifiedNames);

			if (song.SongType != SongType.Instrumental 
				&& song.SongType != SongType.DramaPV 
				&& !song.Tags.HasTag(Tag.CommonTag_Instrumental) 
				&& !ArtistHelper.GetVocalists(song.Artists.ToArray()).Any())
				errors.Add(SongValidationErrors.NonInstrumentalSongNeedsVocalists);

			if (!song.Artists.Any(a => a.Artist != null && ArtistHelper.IsProducerRole(a, SongHelper.IsAnimation(song.SongType))))
				errors.Add(SongValidationErrors.NeedProducer);

			return new ValidationResult(errors);

		}
	
	}

}
