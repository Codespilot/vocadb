using System;
using System.Xml.Linq;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.MikuDb {

	public class MikuDbAlbum {

		private XDocument data;
		private string sourceUrl;
		private string title;

		public MikuDbAlbum() {

			Created = DateTime.Now;

		}

		public MikuDbAlbum(MikuDbAlbumContract contract)
			: this() {
			
			ParamIs.NotNull(() => contract);

			Data = XmlHelper.SerializeToXml(contract.Data);
			SourceUrl = contract.SourceUrl;
			Status = contract.Status;
			Title = contract.Title;

		}

		public virtual PictureData CoverPicture { get; set; }

		public virtual DateTime Created { get; set; }

		public virtual XDocument Data {
			get { return data; }
			set { data = value; }
		}

		public virtual int Id { get; set; }

		public virtual string SourceUrl {
			get { return sourceUrl; }
			set { sourceUrl = value; }
		}

		public virtual AlbumStatus Status { get; set; }

		public virtual string Title {
			get { return title; }
			set { title = value; }
		}
	}

}
