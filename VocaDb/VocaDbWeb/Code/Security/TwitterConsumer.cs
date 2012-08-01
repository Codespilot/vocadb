using System;
using System.Configuration;
using System.Web;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using NLog;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code.Security {

	public class TwitterConsumer {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Gets a value indicating whether the Twitter consumer key and secret are set in the web.config file.
		/// </summary>
		private bool IsTwitterConsumerConfigured {
			get {
				return !string.IsNullOrEmpty(AppConfig.TwitterConsumerKey) && !string.IsNullOrEmpty(AppConfig.TwitterConsumerSecret);
			}
		}

		private ServiceProviderDescription SignInWithTwitterServiceDescription { 
			get {
				return new ServiceProviderDescription {
					RequestTokenEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/request_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
					UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/authenticate", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
					AccessTokenEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/access_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
					TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
				};
			}
		}

		private IConsumerTokenManager TokenManager {
			get {

				var store = HttpContext.Current.Session;
				var tokenManager = (InMemoryTokenManager)store["TwitterShortTermUserSessionTokenManager"];
				if (tokenManager == null) {
					string consumerKey = AppConfig.TwitterConsumerKey;
					string consumerSecret = AppConfig.TwitterConsumerSecret;
					if (IsTwitterConsumerConfigured) {
						tokenManager = new InMemoryTokenManager(consumerKey, consumerSecret);
						store["TwitterShortTermUserSessionTokenManager"] = tokenManager;
					} else {
						throw new InvalidOperationException("No Twitter OAuth consumer key and secret could be found in web.config AppSettings.");
					}
				}

				return tokenManager;

			}
		}

		public WebConsumer TwitterSignIn {
			get {
				return new WebConsumer(SignInWithTwitterServiceDescription, TokenManager);
			}
		}

		public AuthorizedTokenResponse ProcessUserAuthorization() {

			try {
				return TwitterSignIn.ProcessUserAuthorization();
			} catch (ProtocolException x) {
				log.ErrorException("Unable to process Twitter authentication", x);
				return null;
			}

		}

	}
}