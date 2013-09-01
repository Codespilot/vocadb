using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocaDb.Model.DataContracts {

	public class UploadedFileContract {

		public string Mime { get; set; }

		public Stream Stream { get; set; }

	}
}
