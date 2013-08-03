using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VocaDb.Tests.TestSupport {

	public static class ResourceHelper {

		public static string ReadTextFile(string fileName) {

			var asm = typeof(ResourceHelper).Assembly;
			var s = asm.GetManifestResourceNames();
			using (var stream = asm.GetManifestResourceStream(asm.GetName().Name + ".TestData." + fileName))
			using (var reader = new StreamReader(stream)) {

				return reader.ReadToEnd();

			}

		}

		public static HtmlDocument ReadHtmlDocument(string fileName) {

			var asm = typeof(ResourceHelper).Assembly;
			var s = asm.GetManifestResourceNames();
			using (var stream = asm.GetManifestResourceStream(asm.GetName().Name + ".TestData." + fileName)) {

				var doc = new HtmlDocument();
				doc.Load(stream);
				return doc;

			}

		}

	}

}
