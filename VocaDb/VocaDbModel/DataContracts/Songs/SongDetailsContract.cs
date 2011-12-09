using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongDetailsContract {

		public SongDetailsContract() {}

		public SongDetailsContract(Song song, ContentLanguagePreference languagePreference) {

			Song = new SongContract(song, languagePreference);

			AdditionalNames = string.Join(", ", song.AllNames.Where(n => n != Song.Name).Distinct());
			Albums = song.Albums.Select(a => new AlbumWithAdditionalNamesContract(a.Album, languagePreference)).OrderBy(a => a.Name).ToArray();
			AlternateVersions = song.AlternateVersions.Select(s => new SongWithAdditionalNamesContract(s, languagePreference)).ToArray();
			Artists = song.AllArtists.Select(a => new ArtistForSongContract(a, languagePreference)).OrderBy(a => a.Artist.Name).ToArray();
			Deleted = song.Deleted;
			Lyrics = song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray();
			Notes = song.Notes;
			OriginalVersion = (song.OriginalVersion != null ? new SongWithAdditionalNamesContract(song.OriginalVersion, languagePreference) : null);
			PVs = song.PVs.Select(p => new PVContract(p)).ToArray();
			Tags = song.Tags.Usages.Select(u => new TagUsageContract(u)).OrderByDescending(t => t.Count).Take(3).ToArray();
			TranslatedName = new TranslatedStringContract(song.TranslatedName);
			WebLinks = song.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public AlbumWithAdditionalNamesContract[] Albums { get; set; }

		[DataMember]
		public SongWithAdditionalNamesContract[] AlternateVersions { get; set; }

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		public ArtistForSongContract[] Artists { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public bool IsFavorited { get; set; }

		[DataMember]
		public LyricsForSongContract[] Lyrics { get; set; }

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public SongWithAdditionalNamesContract OriginalVersion { get; set; }

		[DataMember]
		public PVContract[] PVs { get; set; }

		[DataMember]
		public SongContract Song { get; set; }

		[DataMember]
		public TagUsageContract[] Tags { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
