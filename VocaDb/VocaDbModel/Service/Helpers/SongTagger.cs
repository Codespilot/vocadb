using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.Helpers {

	public class SongTagger {

		private readonly Artist[] artists;

		public SongTagger(Artist[] artists) {
			this.artists = artists;
		}

		private void AddArtist(Song song, Artist artist) {

			if (!song.HasArtist(artist))
				song.AddArtist(artist);

		}

		private void Handle(Song song) {

			foreach (var artist in artists.Where(a => MatchArtist(song, a)))
				AddArtist(song, artist);

			// TODO: replace song name.

		}

		private bool MatchArtist(Song song, Artist artist) {

			foreach (var lang in EnumVal<ContentLanguageSelection>.Values)
				if (song.LocalizedName[lang].ToLowerInvariant().Contains(artist.LocalizedName[lang].ToLowerInvariant()))
					return true;

			return false;

		}

		public void TagSongs(IEnumerable<Song> songs) {
			
			foreach (var song in songs)
				Handle(song);

		}

	}
}
