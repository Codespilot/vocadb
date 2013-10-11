using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Helpers {

	/// <summary>
	/// Sends notifications to users based on artists they're following.
	/// </summary>
	public class FollowedArtistNotifier {

		/// <summary>
		/// Sends notifications
		/// </summary>
		/// <param name="ctx">Repository context. Cannot be null.</param>
		/// <param name="entry">Entry that was created. Cannot be null.</param>
		/// <param name="artists">List of artists for the entry. Cannot be null.</param>
		/// <param name="creator">User who created the entry. The creator will be excluded from all notifications. Cannot be null.</param>
		/// <param name="entryLinkFactory">Factory for creating links to entries. Cannot be null.</param>
		public void SendNotifications(IRepositoryContext<UserMessage> ctx, IEntryWithNames entry, IEnumerable<Artist> artists, IUser creator, IEntryLinkFactory entryLinkFactory) {

			ParamIs.NotNull(() => ctx);
			ParamIs.NotNull(() => entry);
			ParamIs.NotNull(() => artists);
			ParamIs.NotNull(() => creator);
			ParamIs.NotNull(() => entryLinkFactory);

			var coll = artists.ToArray();
			var users = coll
				.SelectMany(a => a.Users.Select(u => u.User))
				.Distinct()
				.Where(u => !u.Equals(creator))
				.ToArray();

			foreach (var user in users) {

				var followedArtists = coll.Where(a => a.Users.Any(u => u.User.Equals(user))).ToArray();

				string title;
				string msg;

				var entryLink = MarkdownHelper.CreateMarkdownLink(entryLinkFactory.GetFullEntryUrl(entry), entry.Names.SortNames[user.DefaultLanguageSelection]);
				var entryTypeName = entry.EntryType.ToString().ToLowerInvariant();

				if (followedArtists.Length == 1) {

					var artistName = followedArtists.First().TranslatedName[user.DefaultLanguageSelection];
					title = string.Format("New {0} by {1}", entryTypeName, artistName);
					msg = string.Format("A new {0}, '{1}', by {2} was just added.",
						entryTypeName, entryLink, artistName);

				} else if (followedArtists.Length > 1) {

					title = string.Format("New {0}", entryTypeName);
					msg = string.Format("A new {0}, '{1}', by multiple artists you're following was just added.",
						entryTypeName, entryLink);

 				} else {

					continue;

				}

				msg += "\nYou're receiving this notification because you're following the artist(s).";

				var notification = new UserMessage(user, title, msg, false);
				ctx.Save(notification);

			}

		}

	}
}
