using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service;
using VocaDb.Model.DataContracts.Albums;
using PagedList;

namespace VocaDb.Web.Models.Album {

	public class Index {

		public Index() {}

		public Index(PartialFindResult<AlbumWithAdditionalNamesContract> result, string filter, DiscType discType, 
			AlbumSortRule sortRule, int? page, bool? draftsOnly) {

			Page = page ?? 1;
			Albums = new StaticPagedList<AlbumWithAdditionalNamesContract>(result.Items, 
				Page, 30, result.TotalCount);
			DiscType = discType;
			DraftsOnly = draftsOnly ?? false;
			Filter = filter;
			Sort = sortRule;

		}

		public IPagedList<AlbumWithAdditionalNamesContract> Albums { get; set; }

		public DiscType DiscType { get; set; }

		[Display(ResourceType = typeof(ViewRes.EntryIndexStrings), Name = "OnlyDrafts")]
		public bool DraftsOnly { get; set; }

		public string Filter { get; set; }

		public int Page { get; set; }

		public AlbumSortRule Sort { get; set; }

	}

}