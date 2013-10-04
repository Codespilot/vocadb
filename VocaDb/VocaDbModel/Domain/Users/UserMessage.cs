using System;

namespace VocaDb.Model.Domain.Users {

	public class UserMessage : IEntryWithIntId {

		private string message;
		private User receiver;
		private User sender;
		private string subject;

		public UserMessage() {
			Created = DateTime.Now;
		}

		public UserMessage(User to, string subject, string body, bool highPriority)
			: this() {

			Receiver = to;
			Subject = subject;
			Message = body;
			HighPriority = highPriority;

		}

		public UserMessage(User from, User to, string subject, string body, bool highPriority)
			: this() {

			Sender = from;
			Receiver = to;
			Subject = subject;
			Message = body;
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

		/// <summary>
		/// Receiver of this message. Cannot be null.
		/// </summary>
		public virtual User Receiver {
			get { return receiver; }
			set {
				ParamIs.NotNull(() => value);
				receiver = value;
			}
		}

		/// <summary>
		/// Sender of this message. Can be null, in which case it's a notification.
		/// </summary>
		public virtual User Sender {
			get { return sender; }
			set { sender = value; }
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
