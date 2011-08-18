using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArtistContract {

		public ArtistContract() {}

		public ArtistContract(Artist artist) {

			ParamIs.NotNull(() => artist);

			ArtistType = artist.ArtistType;
			Id = artist.Id;
			Name = artist.Name;

		}

		public ArtistType ArtistType { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

	}

}
