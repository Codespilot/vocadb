using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Songs {

	public class SongDiff {

		public SongDiff() {
			IncludeLyrics = true;
		}

		public bool IncludeLyrics { get; set; }

	}

}
