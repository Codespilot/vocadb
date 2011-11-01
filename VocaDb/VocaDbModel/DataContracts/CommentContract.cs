using System;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	public class CommentContract {

		public CommentContract() { }

		public CommentContract(Comment comment) {

			ParamIs.NotNull(() => comment);

			Author = (comment.Author != null ? new UserContract(comment.Author) : null);
			AuthorName = comment.AuthorName;
			Created = comment.Created;
			Id = comment.Id;
			Message = comment.Message;

		}

		public UserContract Author { get; set; }

		public string AuthorName { get; set; }

		public DateTime Created { get; set; }

		public int Id { get; set; }

		public string Message { get; set; }

	}

}
