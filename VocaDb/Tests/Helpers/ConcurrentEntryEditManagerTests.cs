using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

    /// <summary>
    /// Unit tests for <see cref="ConcurrentEntryEditManager"/>.
    /// </summary>
    [TestClass]
    public class ConcurrentEntryEditManagerTests {

        private EntryRef entryRef;
        private User user;

        [TestInitialize]
        public void SetUp() {
            
            entryRef = new EntryRef(EntryType.Album, 39);
            user = new User("Test user", "123", "test@vocadb.net", 0) { Id = 1 };

        }

        [TestMethod]
        public void CheckConcurrentEdits_NoOneEditing() {

            var result = ConcurrentEntryEditManager.CheckConcurrentEdits(entryRef, user);

            Assert.AreEqual(ConcurrentEntryEditManager.Nothing.UserId, result.UserId, "no one editing");

        }

    }

}
