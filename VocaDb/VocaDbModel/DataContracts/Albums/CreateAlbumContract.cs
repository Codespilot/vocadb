using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums {

	public class CreateAlbumContract {

		public ArtistContract[] Artists { get; set; }

		public DiscType DiscType { get; set; }

		public string NameEnglish { get; set; }

		public string NameOriginal { get; set; }

		public string NameRomaji { get; set; }

	}

}
