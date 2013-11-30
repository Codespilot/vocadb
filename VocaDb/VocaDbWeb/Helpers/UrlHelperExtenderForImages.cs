using System.Web.Mvc;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Web.Helpers {

	public static class UrlHelperExtenderForImages {

		private static readonly ServerEntryThumbPersister imagePersister = new ServerEntryThumbPersister();

		private static string GetUnknownImageUrl(UrlHelper urlHelper, IEntryImageInformation imageInfo) {
			return urlHelper.Content("~/Content/unknown.png");
		}

		private static bool ShouldExist(IEntryImageInformation imageInfo) {
			// Image should have MIME type, otherwise it's assumed not to exist.
			return !string.IsNullOrEmpty(imageInfo.Mime);
		}

		/// <summary>
		/// Returns an URL to entry thumbnail image.
		/// Currently only used for album and artist main images.
		/// 
		/// Gets the URL to the static images folder on disk if possible,
		/// otherwise gets the image from the DB.
		/// </summary>
		/// <param name="urlHelper">URL helper. Cannot be null.</param>
		/// <param name="imageInfo">Image information. Cannot be null.</param>
		/// <param name="size">Requested image size.</param>
		/// <returns>URL to the image thumbnail.</returns>
		public static string ImageThumb(this UrlHelper urlHelper, IEntryImageInformation imageInfo, ImageSize size) {
			
			if (imageInfo == null)
				return null;

			var shouldExist = ShouldExist(imageInfo);

			// Use MVC dynamic actions when requesting original or an image that doesn't exist on disk.
			if (imageInfo.EntryType == EntryType.Album) {

				if (size == ImageSize.Original)
					return urlHelper.Action("CoverPicture", "Album", new { id = imageInfo.Id, v = imageInfo.Version });
				else if (shouldExist && !imagePersister.HasImage(imageInfo, size))
					return urlHelper.Action("CoverPictureThumb", "Album", new { id = imageInfo.Id, v = imageInfo.Version });

			} else if (imageInfo.EntryType == EntryType.Artist) {
				
				if (size == ImageSize.Original)
					return urlHelper.Action("Picture", "Artist", new { id = imageInfo.Id, v = imageInfo.Version });
				else if (shouldExist && !imagePersister.HasImage(imageInfo, size))
					return urlHelper.Action("PictureThumb", "Artist", new { id = imageInfo.Id, v = imageInfo.Version });

			}

			if (!shouldExist)
				return GetUnknownImageUrl(urlHelper, imageInfo);

			return imagePersister.GetUrlAbsolute(imageInfo, size);

		}

	}
}