using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Queries for <see cref="UserMessage"/>.
	/// </summary>
	public class UserMessageQueries : QueriesBase<IUserMessageRepository> {

		public UserMessageQueries(IUserMessageRepository repository, IUserPermissionContext permissionContext) 
			: base(repository, permissionContext) {}

		/// <summary>
		/// Permanently deletes a message by Id.
		/// Currently only the receiver can delete the message. 
		/// This is meant for notifications. Personal messages need different handling.
		/// </summary>
		/// <param name="messageId">Id of the message to be deleted.</param>
		public void Delete(int messageId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			repository.HandleTransaction(ctx => {

				var msg = ctx.Load(messageId);

				VerifyResourceAccess(msg.Receiver);

				if (msg.Sender != null)
					msg.Sender.SentMessages.Remove(msg);

				msg.Receiver.ReceivedMessages.Remove(msg);
				ctx.Delete(msg);

			});

		}

		public UserMessageContract Get(int messageId, IUserIconFactory iconFactory) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return repository.HandleTransaction(ctx => {

				var msg = ctx.Load(messageId);

				if (msg.Sender != null)
					VerifyResourceAccess(msg.Sender, msg.Receiver);
				else
					VerifyResourceAccess(msg.Receiver);

				if (!msg.Read && PermissionContext.LoggedUser.Id == msg.Receiver.Id) {
					msg.Read = true;
					ctx.Update(msg);
				}

				return new UserMessageContract(msg, iconFactory, includeBody: true);

			});

		}

	}

}