﻿namespace VocaDb.Model.Domain.Users {

	/// <summary>
	/// Various additional properties for user that are not needed in most cases.
	/// For example, for authentication and user profile.
	/// </summary>
	public class UserOptions {

		private string aboutMe;
		private string albumFormatString;
		private string lastLoginAddress;
		private string location;
		private string oauthToken;
		private string oauthTokenSecret;
		private string realname;
		private string twitterName;
		private User user;

		public UserOptions() {		
	 
			LastLoginAddress 
				= AboutMe
				= AlbumFormatString
				= Location
				= Realname
				= TwitterName = TwitterOAuthToken = TwitterOAuthTokenSecret 
				= string.Empty;

			PublicRatings = true;

		}

		public UserOptions(User user)
			: this() {

			User = user;

		}

		public virtual string AboutMe {
			get { return aboutMe; }
			set { 
				ParamIs.NotNull(() => value);
				aboutMe = value; 
			}
		}

		public virtual string AlbumFormatString {
			get { return albumFormatString; }
			set {
				ParamIs.NotNull(() => value);
				albumFormatString = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual string LastLoginAddress {
			get { return lastLoginAddress; }
			set { 
				ParamIs.NotNull(() => value);
				lastLoginAddress = value; 
			}
		}

		public virtual string Location {
			get { return location; }
			set { 
				ParamIs.NotNull(() => value);
				location = value; 
			}
		}

		/// <summary>
		/// Poisoned accounts cause the user logging in to be banned.
		/// </summary>
		public virtual bool Poisoned { get; set; }

		public virtual bool PublicRatings { get; set; }

		public virtual string Realname {
			get { return realname; }
			set { 
				ParamIs.NotNull(() => value);
				realname = value; 
			}
		}

		public virtual int TwitterId { get; set; }

		public virtual string TwitterName {
			get { return twitterName; }
			set { 
				ParamIs.NotNull(() => value);
				twitterName = value; 
			}
		}

		public virtual string TwitterOAuthToken {
			get { return oauthToken; }
			set { 
				ParamIs.NotNull(() => value);
				oauthToken = value; 
			}
		}

		public virtual string TwitterOAuthTokenSecret {
			get { return oauthTokenSecret; }
			set { 
				oauthTokenSecret = value; 
				ParamIs.NotNull(() => value);
			}
		}

		public virtual User User {
			get { return user; }
			set { 
				ParamIs.NotNull(() => value);
				user = value; 
			}
		}
	}
}
