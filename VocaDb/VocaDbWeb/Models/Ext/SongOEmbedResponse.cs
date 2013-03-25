using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VocaDb.Web.Models.Ext {

	public class SongOEmbedResponse {

		public string Html { get; set; }

		public string Type {
			get { return "video"; }
		}

		public string Version {
			get { return "1.0"; }
		}

	}

}