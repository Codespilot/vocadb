﻿using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// Provides URL to user's profile icon.
	/// 
	/// Gravatar icons are created based on email address, but most of the time we don't
	/// want to send the email address to the client, especially other users' email addresses.
	/// 
	/// This factory can be used to create URL to the profile icon based on user's email address
	/// (or possibly other properties) without revealing the email address.
	/// </summary>
	public interface IUserIconFactory {

		string GetIconUrl(User user);

	}

}
