using System;
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
			CreateDate = artist.CreateDate;
			Description = artist.Description;
			Draft = artist.Status == EntryStatus.Draft;
			FollowCount = artist.Users.Count;
			Groups = artist.Groups.Select(g => new GroupForArtistContract(g, languagePreference)).OrderBy(g => g.Group.Name).ToArray();
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			LatestAlbums = new AlbumContract[] {};
			LatestSongs = new SongContract[] {};
			Members = artist.Members.Select(m => new GroupForArtistContract(m, languagePreference)).OrderBy(a => a.Member.Name).ToArray();
			Pictures = artist.Pictures.Select(p => new EntryPictureFileContract(p)).ToArray();
			Tags = artist.Tags.Usages.Select(u => new TagUsageContract(u)).OrderByDescending(t => t.Count).ToArray();
			TopAlbums = new AlbumContract[] {};
			TopSongs = new SongContract[] {};
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public string AllNames { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool Draft { get; set; }

		[DataMember]
		public int FollowCount { get; set; }

		[DataMember]
		public GroupForArtistContract[] Groups { get; set; }

		[DataMember]
		public bool IsAdded { get; set; }

		[DataMember]
		public CommentContract[] LatestComments { get; set; }

		[DataMember]
		public GroupForArtistContract[] Members { get; set; }

		[DataMember]
		public AlbumContract[] LatestAlbums { get; set; }

		[DataMember]
		public SongContract[] LatestSongs { get; set; }

		[DataMember]
		public EntryPictureFileContract[] Pictures { get; set; }

		[DataMember]
		public TagUsageContract[] Tags { get; set; }

		[DataMember]
		public AlbumContract[] TopAlbums { get; set; }

		[DataMember]
		public SongContract[] TopSongs { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
