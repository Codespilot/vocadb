using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	public class UnifiedCommentContract : CommentContract {

		public UnifiedCommentContract() { }

		public UnifiedCommentContract(Comment comment)
			: base(comment) {

			Entry = new EntryBaseContract(comment.Entry);

		}

		public EntryBaseContract Entry { get; set; }

		/*public UnifiedCommentContract(AlbumComment comment, ContentLanguagePreference languagePreference)
			: base(comment) {

			Album = new AlbumContract(comment.Album, languagePreference);

		}

		public UnifiedCommentContract(ArtistComment comment, ContentLanguagePreference languagePreference)
			: base(comment) {

			Artist = new ArtistContract(comment.Artist, languagePreference);

		}

		public UnifiedCommentContract(SongComment comment, ContentLanguagePreference languagePreference)
			: base(comment) {

			Artist = new ArtistContract(comment.Song, languagePreference);

		}

		public AlbumContract Album { get; set; }

		public ArtistContract Artist { get; set; }

		public ArtistContract Artist { get; set; }*/

	}

}
