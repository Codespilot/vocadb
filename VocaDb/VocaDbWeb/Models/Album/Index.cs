using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.Service;
using VocaDb.Model.DataContracts.Albums;
using PagedList;

namespace VocaDb.Web.Models.Album {

	public class Index {

		public Index() {}

		public Index(PartialFindResult<AlbumWithAdditionalNamesContract> result, string filter, int? page) {

			Albums = new StaticPagedList<AlbumWithAdditionalNamesContract>(result.Items.OrderBy(a => a.Name), 
				page ?? 1, 30, result.TotalCount);
			Filter = filter;

		}

		public IPagedList<AlbumWithAdditionalNamesContract> Albums { get; set; }

		public string Filter { get; set; }

	}

}