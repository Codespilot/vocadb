using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Domain.Images {

	/// <summary>
	/// Tests for <see cref="ImageThumbGenerator"/>.
	/// TODO: these tests should use some virtual file system.
	/// </summary>
	[TestClass]
	public class ImageThumbGeneratorTests {

		private TempImagePathMapper imageMapper;
		private ImageThumbGenerator target;

		private Stream TestImage() {
			return ResourceHelper.GetFileStream("yokohma_bay_concert.jpg");
		}

		private void AssertDimensions(string file, int width, int height) {

			using (var img = Image.FromFile(file)) {
				Assert.AreEqual(width, img.Width, "Image width");
				Assert.AreEqual(height, img.Height, "Image height");
			}

		}

		[TestInitialize]
		public void SetUp() {

			imageMapper = new TempImagePathMapper();
			if (Directory.Exists(imageMapper.Folder)) {
				Directory.Delete(imageMapper.Folder, true);				
			}
			target = new ImageThumbGenerator(imageMapper);

		}

		[TestMethod]
		public void GenerateThumbsAndMoveImage_Original() {

			var thumb = new EntryThumbContract {EntryType = EntryType.SongList, FileName = "test.jpg"};

			target.GenerateThumbsAndMoveImage(TestImage(), thumb, ImageSizes.Original);

			var path = imageMapper.GetImagePath(EntryType.SongList, "test.jpg");
			Assert.IsTrue(File.Exists(path), "File was created");
			AssertDimensions(path, 480, 800);

		}

		[TestMethod]
		public void GenerateThumbsAndMoveImage_Thumbnail() {

			var thumb = new EntryThumbContract { EntryType = EntryType.SongList, FileNameThumb = "test-t.jpg" };

			target.GenerateThumbsAndMoveImage(TestImage(), thumb, ImageSizes.Thumb);

			var path = imageMapper.GetImagePath(EntryType.SongList, "test-t.jpg");
			Assert.IsTrue(File.Exists(path), "File was created");
			AssertDimensions(path, 150, 250);

		}

		[TestMethod]
		public void GenerateThumbsAndMoveImage_OriginalAndSmallThumb() {

			var thumb = new EntryThumbContract { EntryType = EntryType.SongList, FileName = "test.jpg", FileNameSmallThumb = "test-st.jpg" };

			target.GenerateThumbsAndMoveImage(TestImage(), thumb, ImageSizes.Original | ImageSizes.SmallThumb);

			var origPath = imageMapper.GetImagePath(EntryType.SongList, "test.jpg");
			Assert.IsTrue(File.Exists(origPath), "File was created");
			AssertDimensions(origPath, 480, 800);
			var thumbPath = imageMapper.GetImagePath(EntryType.SongList, "test-st.jpg");
			Assert.IsTrue(File.Exists(thumbPath), "File was created");
			AssertDimensions(thumbPath, 90, 150);

		}

	}

}
