namespace VocaDb.Model.Domain.Artists {

	public enum ArtistType {

		Unknown,

		Circle,

		/// <summary>
		/// Producer is the maker or the song (for example doriko)
		/// </summary>
		Producer,

		/// <summary>
		/// Performer is an entity performing the song (for example the Vocaloid characters)
		/// </summary>
		Performer,

	}
}
