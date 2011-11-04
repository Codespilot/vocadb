using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Users {

	public class UserMessage {

		private string message;
		private User receiver;
		private User sender;
		private string subject;

		public UserMessage() {
			Created = DateTime.Now;
		}

		public UserMessage(User from, User to, string subject, string body, bool highPriority)
			: this() {

			Sender = from;
			Receiver = to;
			Subject = subject;
			Message = message;
			HighPriority = highPriority;

		}

		public virtual DateTime Created { get; set; }

		public virtual bool HighPriority { get; set; }

		public virtual int Id { get; set; }

		public virtual string Message {
			get { return message; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				message = value;
			}
		}

		public virtual bool Read { get; set; }

		public virtual User Receiver {
			get { return receiver; }
			set {
				ParamIs.NotNull(() => value);
				receiver = value;
			}
		}

		public virtual User Sender {
			get { return sender; }
			set {
				ParamIs.NotNull(() => value);
				sender = value;
			}
		}

		public virtual string Subject {
			get { return subject; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				subject = value;
			}
		}

	}

}
