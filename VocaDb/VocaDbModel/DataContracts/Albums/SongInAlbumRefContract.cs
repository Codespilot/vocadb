﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInAlbumRefContract : ObjectRefContract {

		public SongInAlbumRefContract() {}

		public SongInAlbumRefContract(SongInAlbum songInAlbum)
			: base(songInAlbum.Song.Id, songInAlbum.Song.DefaultName) {

			TrackNumber = songInAlbum.TrackNumber;

		}

		[DataMember]
		public int TrackNumber { get; set; }

	}

}
