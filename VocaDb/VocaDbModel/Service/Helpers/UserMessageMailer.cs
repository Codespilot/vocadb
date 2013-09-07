using System;
using System.Net.Mail;
using NLog;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Service.Helpers {

	public class UserMessageMailer {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public bool SendEmail(string mySettingsUrl, string toEmail, string receiverName, string subject, string body) {

			if (string.IsNullOrEmpty(toEmail))
				return false;

			MailAddress to;

			try {
				to = new MailAddress(toEmail);
			} catch (FormatException x) {
				log.WarnException("Unable to validate receiver email", x);
				return false;
			}

			var mailMessage = new MailMessage();
			mailMessage.To.Add(to);
			mailMessage.Subject = subject;
			mailMessage.Body =
				string.Format(
					"Hi {0},\n\n" +
					"{1}" +
					"\n\n" +
					"If you do not wish to receive more email notifications such as this, you can adjust your settings at {2}.\n\n" +
					"- VocaDB mailer",
				receiverName, body, mySettingsUrl);

			var client = new SmtpClient();

			try {
				client.Send(mailMessage);
			} catch (SmtpException x) {
				log.ErrorException("Unable to send mail", x);
				return false;
			}

			return true;

		}

		public void SendPrivateMessageNotification(string mySettingsUrl, string messagesUrl, UserMessage message) {

			ParamIs.NotNull(() => message);

			var subject = string.Format("New private message from {0}", message.Sender.Name);
			var body = string.Format(
				"You have received a message from {0}. " +
				"You can view your messages at {1}.", 
				message.Sender.Name, messagesUrl);

			SendEmail(mySettingsUrl, message.Receiver.Email, message.Receiver.Name, subject, body);

		}

	}

}
