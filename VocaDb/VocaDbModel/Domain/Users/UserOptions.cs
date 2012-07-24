namespace VocaDb.Model.Domain.Users {

	public class UserOptions {

		private string oauthToken;
		private string oauthTokenSecret;
		private User user;

		public UserOptions() {
			TwitterOAuthToken = TwitterOAuthTokenSecret = string.Empty;
		}

		public UserOptions(User user)
			: this() {

			User = user;

		}

		public virtual int Id { get; set; }

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
