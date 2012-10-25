using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

	[TestClass]
	public class ArtistHelperTests {

		[TestMethod]
		public void GetCanonizedName_NotPName() {

			var result = ArtistHelper.GetCanonizedName("devilish5150");

			Assert.AreEqual("devilish5150", result, "result");

		}

		[TestMethod]
		public void GetCanonizedName_PName() {

			var result = ArtistHelper.GetCanonizedName("devilishP");

			Assert.AreEqual("devilish", result, "result");

		}

		[TestMethod]
		public void GetCanonizedName_PDashName() {

			var result = ArtistHelper.GetCanonizedName("devilish-P");

			Assert.AreEqual("devilish", result, "result");

		}

	}

}
