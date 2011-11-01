using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumDiff {

		public AlbumDiff() {
			IncludeCover = true;
		}

		public bool IncludeCover { get; set; }

	}
}
