using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedSongContract {

		public ArchivedSongContract() { }

		public ArchivedSongContract(Song song) {

			ParamIs.NotNull(() => song);

			Id = song.Id;
			Lyrics = song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray();
			Names = song.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			TranslatedName = new TranslatedStringContract(song.TranslatedName);
			WebLinks = song.WebLinks.Select(l => new WebLinkContract(l)).ToArray();
			
		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public LyricsForSongContract[] Lyrics { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }


	}

}
