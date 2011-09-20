using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class LyricsForSongMap : ClassMap<LyricsForSong> {

		public LyricsForSongMap() {

			Table("LyricsForSongs");

			Id(m => m.Id);
			Map(m => m.Language).Not.Nullable();
			//Map(m => m.Notes).Not.Nullable();
			Map(m => m.Source).Not.Nullable();
			Map(m => m.Value).Column("Text").Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}


	}

}
