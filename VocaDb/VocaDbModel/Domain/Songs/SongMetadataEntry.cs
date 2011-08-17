namespace VocaDb.Model.Domain.Songs {

	public class SongMetadataEntry : MetadataEntry {

		public virtual SongMetadataType MetadataType { get; set; }

		public virtual Song Song { get; set; }

	}

}
