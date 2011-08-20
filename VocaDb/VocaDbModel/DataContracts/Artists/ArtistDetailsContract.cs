using System.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArtistDetailsContract : ArtistWithAdditionalNamesContract {

		public ArtistDetailsContract() {}

		public ArtistDetailsContract(Artist artist)
			: base(artist) {

			Albums = artist.Albums.Select(a => new AlbumContract(a)).ToArray();
			AllNames = string.Join(", ", artist.AllNames.Where(n => n != artist.Name));
			Circle = (artist.Circle != null ? new ArtistContract(artist.Circle) : null);
			Description = artist.Description;
			LocalizedName = new LocalizedStringContract(artist.LocalizedName);
			Members = artist.Members.Select(m => new ArtistContract(m)).ToArray();
			Metadata = artist.Metadata.Select(m => new ArtistMetadataEntryContract(m)).ToArray();
			Songs = artist.Songs.Select(s => new SongContract(s.Song)).ToArray();

		}

		public AlbumContract[] Albums { get; set; }

		public string AllNames { get; set; }

		public ArtistContract Circle { get; set; }

		public string Description { get; set; }

		public LocalizedStringContract LocalizedName { get; set; }

		public ArtistContract[] Members { get; set; }

		public ArtistMetadataEntryContract[] Metadata { get; set; }

		public SongContract[] Songs { get; set; }

	}

}
