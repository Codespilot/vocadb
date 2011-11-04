using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using log4net;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Service.Helpers {

	public class UserMessageMailer {

		private static readonly ILog log = LogManager.GetLogger(typeof(UserMessageMailer));

		public void Send(string messagesUrl, UserMessage message) {

			ParamIs.NotNull(() => message);

			if (string.IsNullOrEmpty(message.Receiver.Email))
				return;

			MailAddress to;

			try {
				to = new MailAddress(message.Receiver.Email);
			} catch (FormatException x) {
				log.Warn("Unable to validate receiver email", x);
				return;
			}

			var mailMessage = new MailMessage();
			mailMessage.To.Add(to);
			mailMessage.Subject = "New private message from " + message.Sender.Name;
			mailMessage.Body =
				"Hi " + message.Receiver.Name + ",\n\n" +
				"You have received a message from " + message.Sender.Name + ".\n" +
				"You can view your messages at " + messagesUrl + " and decline from receiving any future messages by changing settings in your settings.\n\n" +
				"- VocaDB mailer";

			var client = new SmtpClient();

			try {
				client.Send(mailMessage);
			} catch (SmtpException x) {
				log.Error("Unable to send mail", x);
			}

		}

	}

}
