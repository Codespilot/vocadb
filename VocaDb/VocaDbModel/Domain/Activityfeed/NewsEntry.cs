using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Activityfeed {

	public class NewsEntry {

		private string text;
		private User author;

		public NewsEntry() {
			Anonymous = true;
			CreateDate = DateTime.Now;
		}

		public NewsEntry(string text, User author)
			: this() {

			Text = text;
			Author = author;

		}

		public virtual bool Anonymous { get; set; }

		public virtual User Author {
			get { return author; }
			set {
				ParamIs.NotNull(() => value);
				author = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual int Id { get; set; }

		public virtual bool Important { get; set; }

		public virtual string Text {
			get { return text; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				text = value;
			}
		}

	}
}
