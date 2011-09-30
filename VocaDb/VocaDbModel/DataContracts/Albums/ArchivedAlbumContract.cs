using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedAlbumContract {

		public ArchivedAlbumContract() { }

		public ArchivedAlbumContract(Album album) {

			ParamIs.NotNull(() => album);

			Artists = album.Artists.Select(a => new ObjectRefContract(a.Artist.Id, a.Artist.Name)).ToArray();
			CoverPicture = (album.CoverPicture != null ? new PictureDataContract(album.CoverPicture) : null);
			Description = album.Description;
			DiscType = album.DiscType;
			Id = album.Id;
			Names = album.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			ReleaseDate = album.ReleaseDate;
			Songs = album.Songs.Select(s => new SongInAlbumRefContract(s)).ToArray();
			TranslatedName = new TranslatedStringContract(album.TranslatedName);
			WebLinks = album.WebLinks.Select(l => new WebLinkContract(l)).ToArray();

		}

		[DataMember]
		public ObjectRefContract[] Artists { get; set; }

		[DataMember]
		public PictureDataContract CoverPicture { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DiscType DiscType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public DateTime? ReleaseDate { get; set; }

		[DataMember]
		public SongInAlbumRefContract[] Songs { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
