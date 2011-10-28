using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using System;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedArtistContract {

		public ArchivedArtistContract() {}

		public ArchivedArtistContract(Artist artist) {
			
			ParamIs.NotNull(() => artist);

			Albums = artist.Albums.Select(a => new ObjectRefContract(a.Album.Id, a.Album.Name)).ToArray();
			ArtistType = artist.ArtistType;
			Id = artist.Id;
			Description = artist.Description;
			Groups = artist.Groups.Select(g => new ObjectRefContract(g.Id, g.Group.Name)).ToArray();
			Members = artist.Members.Select(m => new ObjectRefContract(m.Id, m.Member.Name)).ToArray();
			Names = artist.Names.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			Picture = (artist.Picture != null ? new PictureDataContract(artist.Picture) : null);
			Songs = artist.Songs.Select(s => new ObjectRefContract(s.Song.Id, s.Song.DefaultName)).ToArray();
			StartDate = artist.StartDate;
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			WebLinks = artist.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray();

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
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public PictureDataContract Picture { get; set; }

		[DataMember]
		public ObjectRefContract[] Songs { get; set; }

		[DataMember]
		public DateTime? StartDate { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
