using System;
using System.Drawing;
using System.IO;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Images {

	public class ImageThumbGenerator {

		private readonly IImagePathMapper imagePathMapper;

		public ImageThumbGenerator(IImagePathMapper imagePathMapper) {

			ParamIs.NotNull(() => imagePathMapper);

			this.imagePathMapper = imagePathMapper;

		}

		private void EnsureDirExistsForFile(string path) {

			var dir = Path.GetDirectoryName(path);

			if (dir != null && !Directory.Exists(dir))
				Directory.CreateDirectory(dir);

		}

		/// <summary>
		/// Writes a stream to a file, overwriting any existing file.
		/// </summary>
		/// <param name="file">Stream to be written. Cannot be null.</param>
		/// <param name="path">Full file system path of the file to be written to. Can be null or empty.</param>
		private void WriteFile(Stream file, string path) {

			if (string.IsNullOrEmpty(path))
				return;

			EnsureDirExistsForFile(path);

			file.Seek(0, SeekOrigin.Begin);

			using (var f = File.Create(path)) {
				file.CopyTo(f);
			}

			file.Seek(0, SeekOrigin.Begin);

		}

		/// <summary>
		/// Writes an image to a file, overwriting any existing file.
		/// If the dimensions of the original image are smaller or equal than the thumbnail size,
		/// the file is simply copied. Otherwise it will be shrunk.
		/// </summary>
		/// <param name="file">Stream to be written. Cannot be null.</param>
		/// <param name="path">Full file system path of the file to be written to. Can be null or empty.</param>
		/// <param name="original">Original image. Cannot be null.</param>
		/// <param name="dimensions">Dimensions of the thumbnail.</param>
		private void WriteThumb(Stream file, string path, Image original, int dimensions) {

			if (string.IsNullOrEmpty(path))
				return;

			EnsureDirExistsForFile(path);

			if (original.Width > dimensions || original.Height > dimensions) {
				using (var thumb = ImageHelper.ResizeToFixedSize(original, dimensions, dimensions)) {
					thumb.Save(path);					
				}
			} else {
				WriteFile(file, path);
			}

		}

		/// <summary>
		/// Generates thumbnails and writes the original file into external image files.
		/// </summary>
		/// <param name="file">Image to be written. Cannot be null.</param>
		/// <param name="originalPath">Target path of the original size image. Can be null or empty, in which case the image is skipped.</param>
		/// <param name="thumbPath">Target path of the 250x250px thumbnail. Can be null or empty, in which case the image is skipped.</param>
		/// <param name="smallThumbPath">Target path of the 150x150px thumbnail. Can be null or empty, in which case the image is skipped.</param>
		/// <param name="tinyThumbPath">Target path of the 70x70px thumbnail. Can be null or empty, in which case the image is skipped.</param>
		public void GenerateThumbsAndMoveImage(Stream file, string originalPath = null, string thumbPath = null, string smallThumbPath = null, string tinyThumbPath = null) {

			WriteFile(file, originalPath);

			using (var original = ImageHelper.OpenImage(file)) {

				WriteThumb(file, thumbPath, original, ImageHelper.DefaultThumbSize);
				WriteThumb(file, smallThumbPath, original, ImageHelper.DefaultSmallThumbSize);
				WriteThumb(file, tinyThumbPath, original, ImageHelper.DefaultTinyThumbSize);

			}

		}

		public void GenerateThumbsAndMoveImage(Stream input, IPictureWithThumbs pictureFile, ImageSizes imageSizes) {

			GenerateThumbsAndMoveImage(input,
				imageSizes.HasFlag(ImageSizes.Original) ? imagePathMapper.GetImagePath(pictureFile, ImageSize.Original) : null,
				imageSizes.HasFlag(ImageSizes.Thumb) ? imagePathMapper.GetImagePath(pictureFile, ImageSize.Thumb) : null,
				imageSizes.HasFlag(ImageSizes.SmallThumb) ? imagePathMapper.GetImagePath(pictureFile, ImageSize.SmallThumb) : null);

		}

	}

	public enum ImageSize {

		Original = 1,

		Thumb = 2,

		SmallThumb = 4,

		TinyThumb = 8,

	}

	[Flags]
	public enum ImageSizes {

		Nothing = 0,

		Original = 1,

		Thumb = 2,

		SmallThumb = 4,

		TinyThumb = 8,

		All = (Original | Thumb | SmallThumb | TinyThumb)

	}

}
