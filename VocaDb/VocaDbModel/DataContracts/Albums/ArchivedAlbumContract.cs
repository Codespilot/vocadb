using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedAlbumContract {

		public ArchivedAlbumContract() { }

		public ArchivedAlbumContract(Album album, AlbumDiff diff) {

			ParamIs.NotNull(() => album);
			ParamIs.NotNull(() => diff);

			Artists = album.Artists.Select(a => new ObjectRefContract(a.Artist)).ToArray();
			CoverPicture = (diff.IncludeCover && album.CoverPicture != null ? new PictureDataContract(album.CoverPicture) : null);
			CreateDate = album.CreateDate;
			Description = (diff.IncludeDescription ? album.Description : null);
			Diff = new AlbumDiffContract(diff);
			DiscType = album.DiscType;
			Id = album.Id;
			OriginalRelease = (album.OriginalRelease != null ? new ArchivedAlbumReleaseContract(album.OriginalRelease) : null);
			PVs = album.PVs.Select(p => new ArchivedPVContract(p)).ToArray();
			Names = (diff.IncludeNames ? album.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null);
			Songs = album.Songs.Select(s => new SongInAlbumRefContract(s)).ToArray();
			TranslatedName = new TranslatedStringContract(album.TranslatedName);
			WebLinks = (diff.IncludeWebLinks ? album.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null);

		}

		[DataMember]
		public ObjectRefContract[] Artists { get; set; }

		[DataMember]
		public PictureDataContract CoverPicture { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public AlbumDiffContract Diff { get; set; }

		[DataMember]
		public DiscType DiscType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public ArchivedAlbumReleaseContract OriginalRelease { get; set; }

		[DataMember]
		public ArchivedPVContract[] PVs { get; set; }

		[DataMember]
		public SongInAlbumRefContract[] Songs { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
