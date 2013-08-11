using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeAuditLogger : IAuditLogger {

		public void AuditLog(string doingWhat, AgentLoginData who, AuditLogCategory category = AuditLogCategory.Unspecified) {
		}

		public void AuditLog(string doingWhat, string who, AuditLogCategory category = AuditLogCategory.Unspecified) {
		}

		public void AuditLog(string doingWhat, User user = null, AuditLogCategory category = AuditLogCategory.Unspecified) {
		}

		public void SysLog(string doingWhat) {
		}

		public void SysLog(string doingWhat, string who) {
		}
	}
}
