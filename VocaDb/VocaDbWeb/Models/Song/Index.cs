using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Domain.Artists;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models.Song {

	public class Index {

		public Index() {}

		public Index(PartialFindResult<SongWithAdditionalNamesContract> result, string filter, bool? draftsOnly, int? page) {

			DraftsOnly = draftsOnly ?? false;
			Songs = new StaticPagedList<SongWithAdditionalNamesContract>(result.Items.OrderBy(a => a.Name), 
				page ?? 1, 30, result.TotalCount);
			Filter = filter;

		}

		[Display(ResourceType = typeof(ViewRes.EntryIndexStrings), Name = "OnlyDrafts")]
		public bool DraftsOnly { get; set; }

		public string Filter { get; set; }

		public IPagedList<SongWithAdditionalNamesContract> Songs { get; set; }

	}
}