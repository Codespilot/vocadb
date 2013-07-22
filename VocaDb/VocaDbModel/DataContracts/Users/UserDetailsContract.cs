using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// Data contract for <see cref="User"/>, for details view.
	/// SECURITY NOTE: take care when sending to client due to the contained sensitive information.
	/// </summary>
	public class UserDetailsContract : UserWithPermissionsContract {

		public UserDetailsContract() {}

		public UserDetailsContract(User user, IUserPermissionContext permissionContext) 
			: base(user, permissionContext.LanguagePreference) {

			AboutMe = user.Options.AboutMe;
			LastLogin = user.LastLogin;
			LastLoginAddress = user.Options.LastLoginAddress;
			Location = user.Options.Location;
			SongLists = user.SongLists
				.Where(l => l.FeaturedCategory == SongListFeaturedCategory.Nothing)
				.Select(l => new SongListContract(l, permissionContext)).ToArray();
			WebLinks = user.WebLinks.OrderBy(w => w.DescriptionOrUrl).Select(w => new WebLinkContract(w)).ToArray();

		}

		public string AboutMe { get; set; }

		public int AlbumCollectionCount { get; set; }

		public int ArtistCount { get; set; }

		public int CommentCount { get; set; }

		public int EditCount { get; set; }

		public int FavoriteSongCount { get; set; }

		public DateTime LastLogin { get; set; }

		public string LastLoginAddress { get; set; }

		[DataMember]
		public CommentContract[] LatestComments { get; set; }

		public int Level { get; set; }

		public string Location { get; set; }

		public int Power { get; set; }

		public SongListContract[] SongLists { get; set; }

		public int SubmitCount { get; set; }

		public int TagVotes { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}
}
