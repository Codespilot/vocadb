using System.Collections.Generic;

namespace VocaDb.Web.Code.Security {

	public class HostCollection {

		private readonly HashSet<string> bannedIPs = new HashSet<string>();
 
		public void Add(string host) {
			lock (bannedIPs) {
				bannedIPs.Add(host);
			}
		}

		public bool Contains(string host) {
			return bannedIPs.Contains(host);
		}

	}
}