using System;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Security {

	public class AuditLogEntry {

		private string action;
		private string agentName;

		public AuditLogEntry() {
			Time = DateTime.Now;
		}

		public AuditLogEntry(AgentLoginData agentLoginData, string action)
			: this() {
			
			ParamIs.NotNull(() => agentLoginData);

			Action = action;
			AgentName = agentLoginData.Name;
			User = agentLoginData.User;

		}

		public virtual string Action {
			get { return action; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				action = value; 
			}
		}

		public virtual string AgentName {
			get { return agentName; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				agentName = value; 
			}
		}

		public virtual long Id { get; set; }

		public virtual DateTime Time { get; set; }

		public virtual User User { get; set; }

	}

}
