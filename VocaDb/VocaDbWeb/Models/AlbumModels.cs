using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts.UseCases;

namespace VocaDb.Web.Models {

	public class AlbumEdit {

		public AlbumEdit(AlbumForEditContract album) {

			Name = album.Name;

		}

		public string Name { get; protected set; }

	}

}