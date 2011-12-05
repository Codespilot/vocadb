using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts {

	public class UnifiedCommentContract : CommentContract {

		public UnifiedCommentContract() { }

		public UnifiedCommentContract(AlbumComment comment, ContentLanguagePreference languagePreference)
			: base(comment) {

			Album = new AlbumContract(comment.Album, languagePreference);

		}

		public UnifiedCommentContract(ArtistComment comment, ContentLanguagePreference languagePreference)
			: base(comment) {

			Artist = new ArtistContract(comment.Artist, languagePreference);

		}

		public AlbumContract Album { get; set; }

		public ArtistContract Artist { get; set; }

	}

}
