using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Admin {

	public class ViewAuditLogModel {

		public ViewAuditLogModel() {

			GroupId = AuditLogUserGroupFilter.NoFilter;

			UserGroups = new[] {
				new KeyValuePair<AuditLogUserGroupFilter, string>(AuditLogUserGroupFilter.NoFilter, "No group filter")
				}.Concat(Translate.UserGroups.Values.Select(u => new KeyValuePair<AuditLogUserGroupFilter, string>(EnumVal<AuditLogUserGroupFilter>.Parse(u.ToString()), Translate.UserGroups[u])))
				.ToArray();

		}

		public string ExcludeUsers { get; set; }

		public string Filter { get; set; }

		public AuditLogUserGroupFilter GroupId { get; set; }

		public bool OnlyNewUsers { get; set; }

		public KeyValuePair<AuditLogUserGroupFilter, string>[] UserGroups { get; set; }

	}

}