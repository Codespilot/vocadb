using System;
using System.IO;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain {

	public abstract class EntryPictureFile {

		private static string GetExtension(string mime) {
			return ImageHelper.GetExtensionFromMime(mime) ?? string.Empty;
		}

		public static string GetFileName(int id, string mime) {
			return string.Format("{0}{1}", id, GetExtension(mime));
		}

		public static string GetFileNameSmallThumb(int id, string mime) {
			return string.Format("{0}-st{1}", id, GetExtension(mime));
		}

		public static string GetFileNameThumb(int id, string mime) {
			return string.Format("{0}-t{1}", id, GetExtension(mime));
		}

		private User author;
		private string mime;
		private string name;

		protected string Extension {
			get {
				return ImageHelper.GetExtensionFromMime(Mime) ?? string.Empty;
			}
		}

		protected EntryPictureFile() {
			Created = DateTime.Now;
		}

		protected EntryPictureFile(string name, string mime, User author)
			: this() {

			Name = name;
			Mime = mime;
			Author = author;

		}

		public virtual User Author {
			get { return author; }
			set { 
				ParamIs.NotNull(() => value);
				author = value; 
			}
		}

		public virtual DateTime Created { get; set; }

		public abstract EntryType EntryType { get; }

		public virtual string FileName {
			get {
				return GetFileName(Id, Mime);
			}
		}

		public virtual string FileNameThumb {
			get {
				return GetFileNameThumb(Id, Mime);
			}
		}

		public virtual int Id { get; set; }

		public virtual string Mime {
			get { return mime; }
			set { 
				ParamIs.NotNull(() => value);
				mime = value; 
			}
		}

		public virtual string Name {
			get { return name; }
			set { 
				ParamIs.NotNull(() => value);
				name = value; 
			}
		}

		public virtual Stream UploadedFile { get; set; }

		public override string ToString() {
			return string.Format("Picture file {0} [{1}]", Name, Id);
		}

	}

}
