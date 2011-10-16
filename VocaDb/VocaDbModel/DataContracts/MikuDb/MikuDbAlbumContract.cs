using System;
using VocaDb.Model.Domain.MikuDb;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.MikuDb {

	public class MikuDbAlbumContract {

		public MikuDbAlbumContract() {}

		public MikuDbAlbumContract(MikuDbAlbum album) {
			
			ParamIs.NotNull(() => album);

			Created = album.Created;
			Data = XmlHelper.DeserializeFromXml<ImportedAlbumDataContract>(album.Data);
			Id = album.Id;
			SourceUrl = album.SourceUrl;
			Status = album.Status;
			Title = album.Title;

		}

		public DateTime Created { get; set; }

		public ImportedAlbumDataContract Data { get; set; }

		public int Id { get; set; }

		public string SourceUrl { get; set; }

		public AlbumStatus Status { get; set; }

		public string Title { get; set; }

	}

}
