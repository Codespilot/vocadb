using System;
using System.Net;
using System.Runtime.Serialization;
using NLog;

namespace VocaDb.Web.Code.Security {

	/// <summary>
	/// Contacts http://www.stopforumspam.com API and queries whether there's a high probability that an IP is malicious.
	/// </summary>
	public class StopForumSpamClient {

		[DataContract(Name="response")]
		class ResponseContract {

			[DataMember]
			public string Appears { get; set; }

			[DataMember]
			public double Confidence { get; set; }

			[DataMember]
			public int Frequency { get; set; }

			[DataMember]
			public string IP { get; set; }

			[DataMember]
			public DateTime LastSeen { get; set; }

		}

		private const string apiUrl = "http://www.stopforumspam.com/api?ip={0}&confidence";
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private ResponseContract CallApi(string ip) {

			var url = string.Format(apiUrl, ip);

			var request = WebRequest.Create(url);
			WebResponse response;

			try {
				response = request.GetResponse();
			} catch (WebException x) {
				log.WarnException("Unable to get response for user name", x);
				return null;
			}

			using (var stream = response.GetResponseStream()) {

				var serializer = new DataContractSerializer(typeof(ResponseContract));
				return (ResponseContract)serializer.ReadObject(stream);

			}

		}

		public bool IsMalicious(string ip) {

			var result = CallApi(ip);

			if (result == null || result.Appears == "no")
				return false;

			double confidenceTreshold = 75d;

			return (result.Confidence > confidenceTreshold);

		}

	}

}