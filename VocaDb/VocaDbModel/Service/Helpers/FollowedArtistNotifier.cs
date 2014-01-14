﻿using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Helpers {

	/// <summary>
	/// Sends notifications to users based on artists they're following.
	/// 
	/// Notifications will not be sent to users with too many unread messages in their inbox.
	/// This is to prevent flooding users with too many notifications.
	/// </summary>
	public class FollowedArtistNotifier {

		private string CreateMessageBody(Artist[] followedArtists, User user, IEntryWithNames entry, IEntryLinkFactory entryLinkFactory, bool markdown) {
			
			var entryTypeName = entry.EntryType.ToString().ToLowerInvariant();
			var entryName = entry.Names.SortNames[user.DefaultLanguageSelection];
			var url = entryLinkFactory.GetFullEntryUrl(entry);

			string entryLink;
			if (markdown) {
				entryLink = MarkdownHelper.CreateMarkdownLink(url, entryName);
			} else {
				entryLink = string.Format("{0} ( {1} )", entryName, url);
			}

			string msg;

			if (followedArtists.Length == 1) {

				var artistName = followedArtists.First().TranslatedName[user.DefaultLanguageSelection];
				msg = string.Format("A new {0}, '{1}', by {2} was just added.",
					entryTypeName, entryLink, artistName);

			} else {

				msg = string.Format("A new {0}, '{1}', by multiple artists you're following was just added.",
					entryTypeName, entryLink);

			}

			msg += "\nYou're receiving this notification because you're following the artist(s).";
			return msg;

		}

		/// <summary>
		/// Sends notifications
		/// </summary>
		/// <param name="ctx">Repository context. Cannot be null.</param>
		/// <param name="entry">Entry that was created. Cannot be null.</param>
		/// <param name="artists">List of artists for the entry. Cannot be null.</param>
		/// <param name="creator">User who created the entry. The creator will be excluded from all notifications. Cannot be null.</param>
		/// <param name="entryLinkFactory">Factory for creating links to entries. Cannot be null.</param>
		/// <param name="mailer">Mailer for user email messages. Cannot be null.</param>
		public void SendNotifications(IRepositoryContext<UserMessage> ctx, IEntryWithNames entry, 
			IEnumerable<Artist> artists, IUser creator, IEntryLinkFactory entryLinkFactory,
			IUserMessageMailer mailer) {

			ParamIs.NotNull(() => ctx);
			ParamIs.NotNull(() => entry);
			ParamIs.NotNull(() => artists);
			ParamIs.NotNull(() => creator);
			ParamIs.NotNull(() => entryLinkFactory);
			ParamIs.NotNull(() => mailer);

			var coll = artists.ToArray();
			var users = coll
				.SelectMany(a => a.Users.Where(u => !u.User.Equals(creator)).Select(u => u.User))
				.Distinct()
				.ToArray();

			foreach (var user in users) {

				var unreadMessagesCount = ctx.Query().Count(m => m.Receiver.Id == user.Id && !m.Read); // Indexed search

				if (unreadMessagesCount > 3)
					continue;

				var followedArtists = coll.Where(a => a.Users.Any(u => u.User.Equals(user))).ToArray();

				if (followedArtists.Length == 0)
					continue;

				string title;

				var entryTypeName = entry.EntryType.ToString().ToLowerInvariant();
				var msg = CreateMessageBody(followedArtists, user, entry, entryLinkFactory, true);

				if (followedArtists.Length == 1) {

					var artistName = followedArtists.First().TranslatedName[user.DefaultLanguageSelection];
					title = string.Format("New {0} by {1}", entryTypeName, artistName);

				} else {

					title = string.Format("New {0}", entryTypeName);

				}

				var notification = new UserMessage(user, title, msg, false);
				ctx.Save(notification);

				if (user.EmailOptions != UserEmailOptions.NoEmail && !string.IsNullOrEmpty(user.Email) 
					&& followedArtists.Any(a => a.Users.Any(u => u.User.Equals(user) && u.EmailNotifications))) {
					
					mailer.SendEmail(user.Email, user.Name, title, CreateMessageBody(followedArtists, user, entry, entryLinkFactory, false));

				}

			}

		}

	}
}
