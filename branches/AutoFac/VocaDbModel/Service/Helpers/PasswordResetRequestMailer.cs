using System;
using System.Net.Mail;
using NLog;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Service.Helpers {

	public class PasswordResetRequestMailer {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public void Send(string resetUrl, PasswordResetRequest request) {

			ParamIs.NotNull(() => request);

			MailAddress to;

			try {
				to = new MailAddress(request.User.Email);
			} catch (FormatException x) {
				log.WarnException("Unable to validate receiver email", x);
				return;
			}

			var mailMessage = new MailMessage();
			mailMessage.To.Add(to);
			mailMessage.Subject = "Password reset requested.";
			mailMessage.Body =
				"Hi " + request.User.Name + ",\n\n" +
				"You (or someone who knows your email address) has requested to reset your password on VocaDB.\n" +
				"You can perform this action at " + resetUrl + "/" + request.Id + ". If you did not request this action, you can ignore this message.\n\n" +
				"- VocaDB mailer";

			var client = new SmtpClient();

			try {
				client.Send(mailMessage);
			} catch (SmtpException x) {
				log.ErrorException("Unable to send mail", x);
			}

		}


	}
}
