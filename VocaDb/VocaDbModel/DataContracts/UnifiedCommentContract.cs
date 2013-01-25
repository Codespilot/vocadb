using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts {

	public class UnifiedCommentContract : CommentContract {

		public UnifiedCommentContract() { }

		public UnifiedCommentContract(Comment comment, ContentLanguagePreference languagePreference)
			: base(comment) {

			Entry = new EntryRefWithNameContract(comment.Entry, languagePreference);

		}

		public EntryRefWithNameContract Entry { get; set; }

	}

}
