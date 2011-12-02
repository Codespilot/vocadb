using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using System;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedArtistContract {

		public ArchivedArtistContract() {}

		public ArchivedArtistContract(Artist artist, ArtistDiff diff) {
			
			ParamIs.NotNull(() => artist);

			Albums = (diff.IncludeAlbums ? artist.Albums.Select(a => new ObjectRefContract(a.Album)).ToArray() : null);
			ArtistType = artist.ArtistType;
			Id = artist.Id;
			Description = (diff.IncludeDescription ? artist.Description : null);
			Groups = artist.Groups.Select(g => new ObjectRefContract(g.Group)).ToArray();
			Members = artist.Members.Select(m => new ObjectRefContract(m.Member)).ToArray();
			Names = (diff.IncludeNames ? artist.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null);
			//Picture = (diff.IncludePicture && artist.Picture != null ? new PictureDataContract(artist.Picture) : null);
			//Songs = artist.Songs.Select(s => new ObjectRefContract(s.Song)).ToArray();
			//StartDate = artist.StartDate;
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			WebLinks = (diff.IncludeWebLinks ? artist.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null);

		}

		[DataMember]
		public ObjectRefContract[] Albums { get; set; }

		[DataMember]
		public ArtistType ArtistType { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public ObjectRefContract[] Groups { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public ObjectRefContract[] Members { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		//[DataMember]
		//public PictureDataContract Picture { get; set; }

		//[DataMember]
		//public ObjectRefContract[] Songs { get; set; }

		//[DataMember]
		//public DateTime? StartDate { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
