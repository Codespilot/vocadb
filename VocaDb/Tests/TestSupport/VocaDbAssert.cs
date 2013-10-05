using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Tests.TestSupport {

	public static class VocaDbAssert {

		public static void ContainsArtists(IEnumerable<IArtistWithSupport> artistLinks, params string[] artistNames) {

			foreach (var artistName in artistNames) {
				Assert.IsTrue(artistLinks.Any(a => a.Artist != null && artistName.Equals(a.Artist.DefaultName)), 
					string.Format("Found artist '{0}'", artistName));
			}

		}

	}
}
