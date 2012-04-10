using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VocaDb.Model.Service;
using VocaDb.Model.DataContracts.Albums;
using PagedList;

namespace VocaDb.Web.Models.Album {

	public class Index {

		public Index() {}

		public Index(PartialFindResult<AlbumWithAdditionalNamesContract> result, string filter, int? page, bool? draftsOnly) {

			Albums = new StaticPagedList<AlbumWithAdditionalNamesContract>(result.Items.OrderBy(a => a.Name), 
				page ?? 1, 30, result.TotalCount);
			DraftsOnly = draftsOnly ?? false;
			Filter = filter;

		}

		public IPagedList<AlbumWithAdditionalNamesContract> Albums { get; set; }

		[Display(ResourceType = typeof(ViewRes.EntryIndexStrings), Name = "OnlyDrafts")]
		public bool DraftsOnly { get; set; }

		public string Filter { get; set; }

	}

}