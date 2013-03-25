using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VocaDb.Web.Models.Ext {

	public class SongOEmbedResponse {

		public string html { get; set; }

		public string type {
			get { return "video"; }
		}

		public string version {
			get { return "1.0"; }
		}

	}

}