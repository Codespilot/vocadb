using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.TagFormatting {

	public class TagFormatter {

		public static readonly string[] TagFormatStrings = new[] {
			"%title% feat. %vocalists%;%producers%;%album%;%discnumber%;%track%",
			"%title%;%producers%;%vocalists%;%album%;%discnumber%;%track%",
			"%title%;%artists%;%album%;%discnumber%;%track%",
		};

		private string ApplyFormat(SongInAlbum track, string format, ContentLanguagePreference languagePreference) {

			var album = track.Album;
			var sb = new StringBuilder(format);

			sb.Replace("%album%", GetField(album.Names.SortNames[languagePreference]));
			sb.Replace("%artists%", GetField(track.Song.ArtistString[languagePreference]));
			sb.Replace("%discnumber%", track.DiscNumber.ToString());
			sb.Replace("%producers%", GetField(string.Join(", ", ArtistHelper.GetProducerNames(album.Artists, AlbumHelper.IsAnimation(album.DiscType), languagePreference))));
			sb.Replace("%title%", GetField(track.Song.Names.SortNames[languagePreference]));
			sb.Replace("%track%", track.TrackNumber.ToString());
			sb.Replace("%vocalists%", GetField(string.Join(", ", ArtistHelper.GetVocalistNames(album.Artists, languagePreference))));

			return sb.ToString();

		}

		private static string GetField(string val) {

			if (string.IsNullOrEmpty(val))
				return string.Empty;

			if (!val.Contains(";"))
				return val;
			else
				return string.Format("\"{0}\"", val);

		}

		public string ApplyFormat(Album album, string format, ContentLanguagePreference languagePreference) {

			var sb = new StringBuilder();

			foreach (var song in album.Songs)
				sb.AppendLine(ApplyFormat(song, format, languagePreference));

			return sb.ToString();

		}

	}
}
