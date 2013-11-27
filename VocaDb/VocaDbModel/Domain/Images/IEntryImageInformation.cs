
namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Provides information about an entry image.
	/// 
	/// This interface is used for saving entry images to disk.
	/// Currently this means thumbnails for songlists and tags, but
	/// will be expanded to album/artist thumbnails soon.
	/// </summary>
	public interface IEntryImageInformation {

		/// <summary>
		/// Type of entry.
		/// </summary>
		EntryType EntryType { get; }

		/// <summary>
		/// Image identifier. 
		/// This may be the entry Id, for example for album/artist main images and for entries with only one image (songlists and tags).
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Image MIME type.
		/// </summary>
		string Mime { get; }

	}

}
