﻿using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// Data contract for <see cref="UserMessage"/>.
	/// Email address is not included, just the URL to profile icon.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserMessageContract {

		public UserMessageContract() { }

		public UserMessageContract(UserMessage message, IUserIconFactory iconFactory, bool includeBody = false) {

			ParamIs.NotNull(() => message);

			Body = (includeBody ? message.Message : string.Empty);
			Created = message.Created;
			CreatedFormatted = Created.ToUniversalTime().ToString("g");
			HighPriority = message.HighPriority;
			Id = message.Id;
			Read = message.Read;
			Receiver = new UserWithIconContract(message.Receiver, iconFactory);
			Sender = (message.Sender != null ? new UserWithIconContract(message.Sender, iconFactory) : null);
			Subject = message.Subject;

		}

		[DataMember]
		public string Body { get; set; }

		// Currently unable to parse raw datetime on client side, therefore only sending formatted datetime instead.
		public DateTime Created { get; set; }

		[DataMember]
		public string CreatedFormatted { get; set; }

		[DataMember]
		public bool HighPriority { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public bool Read { get; set; }

		[DataMember]
		public UserWithIconContract Receiver { get; set; }

		[DataMember]
		public UserWithIconContract Sender { get; set; }

		[DataMember]
		public string Subject { get; set; }

		public override string ToString() {
			return string.Format("Message '{0}' to {1} [{2}]", Subject, Receiver, Id);
		}

	}

}
