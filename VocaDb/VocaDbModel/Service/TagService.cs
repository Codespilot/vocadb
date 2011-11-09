using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.DataContracts.Tags;

namespace VocaDb.Model.Service {

	public class TagService : ServiceBase {

		public TagService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

		public string[] FindTags(string query) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] { };

			return HandleQuery(session => {

				string[] tags;

				if (query.Length < 3)
					tags = session.Query<Tag>().Where(t => t.Name == query).Take(10).ToArray().Select(t => t.Name).ToArray();
				else
					tags = session.Query<Tag>().Where(t => t.Name.Contains(query)).Take(10).ToArray().Select(t => t.Name).ToArray();

				return tags;

			});

		}

		public TagDetailsContract GetTagDetails(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => new TagDetailsContract(session.Load<Tag>(tagName), PermissionContext.LanguagePreference));

		}

		public string[] GetTagNames() {

			return HandleQuery(session => session.Query<Tag>().OrderBy(t => t.Name).Select(t => t.Name).ToArray());

		}

	}

}
