namespace VocaDb.Model.Domain.Artists {

	public class ArtistMetadataEntry : MetadataEntry {

		public virtual ArtistMetadataType MetadataType { get; set; }

		public virtual Artist Artist { get; set; }
	}
}

