﻿using Microsoft.Web.Helpers;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Code {

	/// <summary>
	/// Generates Gravatar URLs for user profile icons.
	/// </summary>
	public class GravatarUserIconFactory : IUserIconFactory {

		private readonly int size;

		public GravatarUserIconFactory(int size = 80) {
			this.size = size;
		}

		public string GetIconUrl(User user) {

			if (string.IsNullOrEmpty(user.Email))
				return string.Empty;

			return Gravatar.GetUrl(user.Email, size);

		}

	}

}