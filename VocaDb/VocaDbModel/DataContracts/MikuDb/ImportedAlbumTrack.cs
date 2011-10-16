using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocaDb.Model.DataContracts.MikuDb {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ImportedAlbumTrack {

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public int TrackNum { get; set; }

	}

}
