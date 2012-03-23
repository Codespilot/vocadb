using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistDetailsContract : ArtistWithAdditionalNamesContract {

		public ArtistDetailsContract() {}

		public ArtistDetailsContract(Artist artist, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference) {

			AllNames = string.Join(", ", artist.AllNames.Where(n => n != Name));
			Description = artist.Description;
			Draft = artist.Status == EntryStatus.Draft;
			Groups = artist.Groups.Select(g => new GroupForArtistContract(g, languagePreference)).OrderBy(g => g.Group.Name).ToArray();
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			LatestAlbums = new AlbumWithAdditionalNamesContract[] {};
			LatestSongs = new SongWithAdditionalNamesContract[] { };
			Members = artist.Members.Select(m => new GroupForArtistContract(m, languagePreference)).OrderBy(a => a.Member.Name).ToArray();
			Tags = artist.Tags.Usages.Select(u => new TagUsageContract(u)).OrderByDescending(t => t.Count).Take(Tag.MaxDisplayedTags).ToArray();
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public string AllNames { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool Draft { get; set; }

		[DataMember]
		public GroupForArtistContract[] Groups { get; set; }

		[DataMember]
		public bool IsAdded { get; set; }

		[DataMember]
		public CommentContract[] LatestComments { get; set; }

		[DataMember]
		public GroupForArtistContract[] Members { get; set; }

		[DataMember]
		public AlbumWithAdditionalNamesContract[] LatestAlbums { get; set; }

		[DataMember]
		public SongWithAdditionalNamesContract[] LatestSongs { get; set; }

		[DataMember]
		public TagUsageContract[] Tags { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
