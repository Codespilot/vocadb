using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedSongContract {

		public ArchivedSongContract() { }

		public ArchivedSongContract(Song song, SongDiff diff) {

			ParamIs.NotNull(() => song);

			Artists = song.Artists.Select(a => new ObjectRefContract(a.Artist.Id, a.Artist.Name)).ToArray();
			Id = song.Id;
			Lyrics = (diff.IncludeLyrics ? song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray() : null);
			Names = song.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray();
			NicoId = song.NicoId;
			Notes = song.Notes;
			OriginalVersion = (song.OriginalVersion != null ? new ObjectRefContract(song.OriginalVersion.Id, song.OriginalVersion.DefaultName) : null);
			PVs = song.PVs.Select(p => new ArchivedPVContract(p)).ToArray();
			SongType = song.SongType;
			Status = song.Status;
			TranslatedName = new ArchivedTranslatedStringContract(song.TranslatedName);
			WebLinks = song.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray();
			
		}

		[DataMember]
		public ObjectRefContract[] Artists { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public LyricsForSongContract[] Lyrics { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public string NicoId { get; set; }

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public ObjectRefContract OriginalVersion { get; set; }

		[DataMember]
		public ArchivedPVContract[] PVs { get; set; }

		[DataMember]
		public SongType SongType { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public ArchivedTranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }


	}

}
