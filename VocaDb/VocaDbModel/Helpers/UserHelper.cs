using System;
using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Helpers {

	public static class UserHelper {

		public static int GetLevel(int power) {

			if (power <= 0)
				return 0;

			return (int)Math.Log(power, Math.E);

		}

		public static int GetPower(UserDetailsContract detailsContract, User user) {

			ParamIs.NotNull(() => detailsContract);
			ParamIs.NotNull(() => user);

			var ownedAlbumCount = user.Albums.Count(a => a.PurchaseStatus == PurchaseStatus.Owned);
			var albumRatingCount = user.Albums.Count(a => a.Rating != 0);
			var songListCount = detailsContract.SongLists.Count();

			var power =
				detailsContract.EditCount / 4
				+ detailsContract.SubmitCount / 2
				+ detailsContract.TagVotes * 2
				+ detailsContract.AlbumCollectionCount * 5
				+ ownedAlbumCount * 10
				+ albumRatingCount * 2
				+ detailsContract.FavoriteSongCount * 2
				+ detailsContract.CommentCount * 4
				+ songListCount * 5;

			return power;

		}

	}

}
