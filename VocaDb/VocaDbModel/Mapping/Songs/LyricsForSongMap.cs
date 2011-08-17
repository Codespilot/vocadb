using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class LyricsForSongMap : ClassMap<LyricsForSong> {

		public LyricsForSongMap() {

			Table("LyricsForSongs");

			Id(m => m.Id);
			Map(M => M.Language).Not.Nullable();
			Map(m => m.Notes).Not.Nullable();
			Map(m => m.Text).Not.Nullable();
			References(m => m.Song).Not.Nullable();


		}


	}

}
