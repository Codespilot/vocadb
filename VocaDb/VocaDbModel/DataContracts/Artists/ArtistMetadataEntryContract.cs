using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArtistMetadataEntryContract {

		public ArtistMetadataEntryContract(ArtistMetadataEntry entry) {

			ParamIs.NotNull(() => entry);

			MetadataType = entry.MetadataType;
			Value = entry.Value;

		}

		public ArtistMetadataType MetadataType { get; set; }

		public string Value { get; set; }

	}

}
