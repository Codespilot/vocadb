using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedArtistContract {

		public ArchivedArtistContract() {}

		public ArchivedArtistContract(Artist artist) {
			
			ParamIs.NotNull(() => artist);

			Albums = artist.Albums.Select(a => new ObjectRefContract(a.Album.Id, a.Album.Name)).ToArray();
			ArtistType = artist.ArtistType;
			Id = artist.Id;
			Circle = (artist.Circle != null ? new ObjectRefContract(artist.Circle.Id, artist.Circle.Name) : null);
			Description = artist.Description;
			Names = artist.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			Songs = artist.Songs.Select(s => new ObjectRefContract(s.Song.Id, s.Song.DefaultName)).ToArray();
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			WebLinks = artist.WebLinks.Select(l => new WebLinkContract(l)).ToArray();

		}

		[DataMember]
		public ObjectRefContract[] Albums { get; set; }

		[DataMember]
		public ArtistType ArtistType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public ObjectRefContract Circle { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public ObjectRefContract[] Songs { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
