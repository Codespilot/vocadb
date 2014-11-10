﻿using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using NHibernate;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Helpers {

	public class TagFactory : ITagFactory {

		private readonly AgentLoginData loginData;
		private readonly ISession session;

		public TagFactory(ISession session, AgentLoginData loginData) {
			this.session = session;
			this.loginData = loginData;
		}

		public Tag CreateTag(string name) {

			var tag = new Tag(name);
			session.Save(tag);

			var archived = tag.CreateArchivedVersion(new TagDiff(), loginData, EntryEditEvent.Created);
			session.Save(archived);

			return tag;
		}

	}

	public class TagFactoryRepository : ITagFactory {

		private readonly AgentLoginData loginData;
		private readonly IRepositoryContext<Tag> session;

		public TagFactoryRepository(IRepositoryContext<Tag> session, AgentLoginData loginData) {
			this.session = session;
			this.loginData = loginData;
		}

		public Tag CreateTag(string name) {

			var tag = new Tag(name);
			session.Save(tag);

			var archived = tag.CreateArchivedVersion(new TagDiff(), loginData, EntryEditEvent.Created);
			session.Save(archived);

			return tag;
		}

	}

}
