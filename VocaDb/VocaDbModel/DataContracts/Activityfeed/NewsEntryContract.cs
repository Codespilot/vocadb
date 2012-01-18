using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.DataContracts.Activityfeed {

	public class NewsEntryContract {

		public NewsEntryContract(NewsEntry newsEntry) {

			ParamIs.NotNull(() => newsEntry);

			Anonymous = newsEntry.Anonymous;
			Author = new UserContract(newsEntry.Author);
			CreateDate = newsEntry.CreateDate;
			Important = newsEntry.Important;
			Text = newsEntry.Text;

		}

		public bool Anonymous { get; set; }

		public UserContract Author { get; set; }

		public DateTime CreateDate { get; set; }

		public bool Important { get; set; }

		public string Text { get; set; }

	}

}
