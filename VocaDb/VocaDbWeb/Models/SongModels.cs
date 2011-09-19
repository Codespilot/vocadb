using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;

namespace VocaDb.Web.Models {

	public class SongEdit {

		public SongEdit(SongForEditContract song) {

			Artists = song.Artists;
			Name = song.Name;

		}

		public ArtistForSongContract[] Artists { get; protected set; }

		public string Name { get; protected set; }

	}

}