using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Web.Controllers;

namespace VocaDb.Tests.Web.Controllers {

	/// <summary>
	/// Tests for <see cref="AlbumController"/>.
	/// </summary>
	[TestClass]
	public class AlbumControllerTests {

		private AlbumController controller;

		[TestInitialize]
		public void SetUp() {
			controller = new AlbumController();
		}

		// TODO

	}
}
