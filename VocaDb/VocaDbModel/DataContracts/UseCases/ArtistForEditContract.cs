using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.UseCases {

	public class ArtistForEditContract : ArtistDetailsContract {

		public ArtistForEditContract(Artist artist, IEnumerable<Artist> allCircles)
			: base(artist) {

			AllCircles = allCircles.Select(a => new ArtistContract(a)).ToArray();

		}

		public ArtistContract[] AllCircles { get; private set; }

	}

}
