using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedAlbumContract {

		public ArchivedAlbumContract() { }

		public ArchivedAlbumContract(Album album) {

			ParamIs.NotNull(() => album);

			CoverPicture = new PictureDataContract(album.CoverPicture);
			Description = album.Description;
			DiscType = album.DiscType;
			Id = album.Id;
			Names = album.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			ReleaseDate = album.ReleaseDate;
			TranslatedName = new TranslatedStringContract(album.TranslatedName);
			WebLinks = album.WebLinks.Select(l => new WebLinkContract(l)).ToArray();

		}

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
		public DateTime ReleaseDate { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
