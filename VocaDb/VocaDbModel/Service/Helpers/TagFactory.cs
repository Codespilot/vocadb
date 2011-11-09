using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Tags;
using NHibernate;

namespace VocaDb.Model.Service.Helpers {

	public class TagFactory : ITagFactory {

		private ISession session;

		public TagFactory(ISession session) {
			this.session = session;
		}

		public Tag CreateTag(string name) {
			var tag = new Tag(name);
			return tag;
		}

	}

}
