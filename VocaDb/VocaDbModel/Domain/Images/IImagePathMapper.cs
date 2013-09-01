using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Images {

	public interface IImagePathMapper {

		string GetImagePath(EntryType entryType, string fileName);

		string GetImageUrlAbsolute(EntryType entryType, string fileName);

	}

	public static class IImagePathMapperExtender {
		
		public static string GetImagePath(this IImagePathMapper imagePathMapper, IPictureWithThumbs picture, ImageSize size) {

			return imagePathMapper.GetImagePath(picture.EntryType, ImageHelper.GetImageFileName(picture, size));

		}

	}

}
