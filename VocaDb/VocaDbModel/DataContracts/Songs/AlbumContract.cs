using System;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class AlbumContract {

		public AlbumContract() { }

		public AlbumContract(Album album) {

			ParamIs.NotNull(() => album);

			Id = album.Id;
			Name = album.Name;
			ReleaseDate = album.ReleaseDate;

		}

		public int Id { get; set; }

		public string Name { get; set; }

		public DateTime ReleaseDate { get; set; }

	}

}
