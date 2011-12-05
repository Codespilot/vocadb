using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Web.Models.Admin {

	public class CommentViewModel {

		public CommentViewModel(CommentContract comment, string targetName, string targetUrl) {
			Comment = comment;
			TargetName = targetName;
			TargetUrl = targetUrl;
		}

		public CommentContract Comment { get; set; }

		public string TargetName { get; set; }

		public string TargetUrl { get; set; }

	}
}