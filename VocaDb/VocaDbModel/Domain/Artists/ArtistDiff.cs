using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistDiff {

		public ArtistDiff() {
			IncludePicture = true;
		}

		public bool IncludePicture { get; set; }

	}

}
