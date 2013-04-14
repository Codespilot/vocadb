using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Helpers {

	public static class SongHelper {

		public static bool IsAnimation(SongType songType) {
			return (songType == SongType.DramaPV || songType == SongType.MusicPV);
		}

	}
}
