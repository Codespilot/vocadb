using System.Configuration;
using System.IO;
using System.Web;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Utils {

	public static class AppConfig {

		private static ArtistType[] artistTypes;
		private static ArtistRoles[] artistRoles;

		/// <summary>
		/// List of roles that can be assigned to artist added to songs and albums.
		/// The list should be in the correct order.
		/// The Default role is excluded because it's not a valid selection.
		/// </summary>
		private static readonly ArtistRoles[] DefaultValidRoles = {
			Domain.Artists.ArtistRoles.Animator,
			Domain.Artists.ArtistRoles.Arranger,
			Domain.Artists.ArtistRoles.Composer,
			Domain.Artists.ArtistRoles.Distributor,
			Domain.Artists.ArtistRoles.Illustrator,
			Domain.Artists.ArtistRoles.Instrumentalist,
			Domain.Artists.ArtistRoles.Lyricist,
			Domain.Artists.ArtistRoles.Mastering,
			Domain.Artists.ArtistRoles.Mixer,
			Domain.Artists.ArtistRoles.Publisher,
			Domain.Artists.ArtistRoles.Vocalist,
			Domain.Artists.ArtistRoles.VoiceManipulator,
			Domain.Artists.ArtistRoles.Other
		};

		private static string Val(string key) {
			return ConfigurationManager.AppSettings[key];
		}

		private static bool Val(string key, bool def) {

			var val = Val(key);
			bool boolVal;
			if (bool.TryParse(val, out boolVal))
				return boolVal;
			else
				return def;
			
		}

		public static bool AllowCustomTracks {
			get { return Val("AllowCustomTracks", false); }
		}

		public static ArtistType[] ArtistTypes {
			get {
				
				if (artistTypes == null) {
					var val = Val("ArtistTypes");
					artistTypes = !string.IsNullOrEmpty(val) ? EnumVal<ArtistType>.ParseMultiple(val) : EnumVal<ArtistType>.Values;
				}

				return artistTypes;

			}
		}

		public static ArtistRoles[] ArtistRoles {
			get {
				
				if (artistRoles == null) {
					var val = Val("ArtistRoles");
					artistRoles = !string.IsNullOrEmpty(val) ? EnumVal<ArtistRoles>.ParseMultiple(val) : DefaultValidRoles;
				}

				return artistRoles;

			}
		}

		public static string BilibiliAppKey {
			get { return Val("BilibiliAppKey"); }
		}

		public static string BrandedStringsAssembly {
			get { return Val("BrandedStringsAssembly"); }
		}

		public static string DbDumpFolder {
			get {
				return Val("DbDumpFolder");
			}
		}

		public static string ExternalHelpPath {
			get { return Val("ExternalHelpPath"); }
		}

		public static string GAAccountId {
			get { return Val("GAAccountId"); }
		}

		public static string GADomain {
			get { return Val("GADomain"); }
		}

		public static GlobalLinksSection GetLobalLinksSection() {
		
			var bigBanners = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/bigBanners");
			var smallBanners = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/smallBanners");
			var socialSites = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/socialSites");

			return new  GlobalLinksSection { BigBanners = bigBanners, SmallBanners  = smallBanners, SocialSites = socialSites };

		}

		/// <summary>
		/// Host address of the main site, contains full path to the web application's root, including hostname.
		/// Could be either HTTP or HTTPS.
		/// For example http://vocadb.net
		/// </summary>
		public static string HostAddress {
			get {
				return Val("HostAddress");
			}
		}

		/// <summary>
		/// Host address of the SSL site, used for sensitive actions such as logging in.
		/// For example https://vocadb.net
		/// </summary>
		public static string HostAddressSecure {
			get {
				return Val("HostAddressSecure");
			}
		}

		public static string LockdownMessage {
			get {
				return Val("LockdownMessage");
			}
		}

		public static string ReCAPTCHAKey {
			get {
				return Val("ReCAPTCHAKey");
			}
		}

		public static string ReCAPTCHAPublicKey {
			get {
				return Val("ReCAPTCHAPublicKey");
			}
		}

		public static string StaticContentPath {
			get {
				return Val("StaticContentPath");
			}
		}

		public static string StaticContentHost {
			get {
				return Val("StaticContentHost");
			}
		}

		public static string StaticContentHostSSL {
			get {
				return Val("StaticContentHostSSL");
			}
		}

		public static string TwitterConsumerKey {
			get {
				return Val("TwitterConsumerKey");
			}
		}

		public static string TwitterConsumerSecret {
			get {
				return Val("TwitterConsumerSecret");
			}
		}

	}
}
