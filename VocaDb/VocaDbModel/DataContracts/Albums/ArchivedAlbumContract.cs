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

			Artists = (diff.IncludeArtists ? album.Artists.Select(a => new ObjectRefContract(a.Artist)).ToArray() : null);
			Description = (diff.IncludeDescription ? album.Description : null);
			DiscType = album.DiscType;
			Id = album.Id;
			OriginalRelease = (album.OriginalRelease != null && !album.OriginalRelease.IsEmpty ? new ArchivedAlbumReleaseContract(album.OriginalRelease) : null);
			PVs = album.PVs.Select(p => new ArchivedPVContract(p)).ToArray();
			Names = (diff.IncludeNames ? album.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null);
			Songs = (diff.IncludeTracks ? album.Songs.Select(s => new SongInAlbumRefContract(s)).ToArray() : null);
			TranslatedName = new TranslatedStringContract(album.TranslatedName);
			WebLinks = (diff.IncludeWebLinks ? album.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null);

		}

		[DataMember]
		public ObjectRefContract[] Artists { get; set; }

		[DataMember]
		public string Description { get; set; }

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
