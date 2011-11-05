using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Web.Helpers {

	public static class TagsHelper {

		private static string GetField(string val) {

			if (!val.Contains(";"))
				return val;
			else
				return string.Format("\"{0}\"", val);

		}

		public static string GetAlbumTags(AlbumDetailsContract album) {

			var sb = new StringBuilder();

			foreach (var track in album.Songs) {
				sb.AppendLine(string.Format("{0};{1};{2};{3}", 
					GetField(album.Name), track.TrackNumber, GetField(track.Song.Name), GetField(track.Song.ArtistString)));
			}

			return sb.ToString();

		}

	}
}