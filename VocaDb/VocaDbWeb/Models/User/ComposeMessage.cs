using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Web.Models.User {

	public class ComposeMessage {

		public ComposeMessage() { }

		[Display(Name = "Body")]
		[Required]
		[StringLength(1000)]
		public string Body { get; set; }

		public bool HighPriority { get; set; }

		[Display(Name = "Body")]
		[Required]
		[StringLength(100)]
		public string ReceiverName { get; set; }

		[Required]
		[StringLength(200)]
		public string Subject { get; set; }

		public UserMessageContract ToContract(int senderId) {

			return new UserMessageContract {
				Body = this.Body,
				HighPriority = this.HighPriority,
				Receiver = new UserContract { Name = ReceiverName },
				Sender = new UserContract { Id = senderId },
				Subject = this.Subject
			};

		}

	}

}