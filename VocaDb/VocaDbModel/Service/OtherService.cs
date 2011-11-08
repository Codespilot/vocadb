using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service {

	public class OtherService : ServiceBase {

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

		public string[] FindTags(string query) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] {};

			return HandleQuery(session => {

				string[] tags;

				if (query.Length < 3)
					tags = session.Query<Tag>().Where(t => t.Name == query).Take(10).Select(t => t.Name).ToArray();
				else
					tags = session.Query<Tag>().Where(t => t.Name.Contains(query)).Take(10).Select(t => t.Name).ToArray();

				return tags;

			});

		}

		public FrontPageContract GetFrontPageContent() {

			return HandleQuery(session => new FrontPageContract(
				session.Query<Album>().Where(a => !a.Deleted)
					.OrderByDescending(a => a.CreateDate).Take(15),
				session.Query<Song>().Where(s => !s.Deleted)
					.OrderByDescending(s => s.CreateDate).Take(15), 
				PermissionContext.LanguagePreference));

		}

	}
}
