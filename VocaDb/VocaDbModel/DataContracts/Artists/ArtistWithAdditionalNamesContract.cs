using System.Linq;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArtistWithAdditionalNamesContract : ArtistContract {

		public ArtistWithAdditionalNamesContract(Artist artist)
			: base(artist) {

			AdditionalNames = string.Join(", ", artist.AllNames.Where(n => n != artist.Name));

		}

		public string AdditionalNames { get; private set; }

	}

}
