using System;
using System.Collections.Generic;
using VocaDb.Model;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models.Song {

	public class Index {

		public Index() {}

		public Index(PartialFindResult<SongWithAlbumAndPVsContract> result, string filter, NameMatchMode nameMatchMode, SongType songType, string timeFilter, bool onlyWithPVs, 
			SongSortRule sortRule, SongViewMode viewMode, 
			bool draftsOnly, int page, int pageSize) {

			DraftsOnly = draftsOnly;
			Songs = new StaticPagedList<SongWithAlbumAndPVsContract>(result.Items, 
				page, pageSize, result.TotalCount);
			Filter = filter;
			NameMatchMode = nameMatchMode;
			SongType = songType;
			Since = timeFilter;
			OnlyWithPVs = onlyWithPVs;
			SortRule = sortRule;
			ViewMode = viewMode;

		}

		[Display(ResourceType = typeof(ViewRes.EntryIndexStrings), Name = "OnlyDrafts")]
		public bool DraftsOnly { get; set; }

		public string Filter { get; set; }

		public Dictionary<SongType, string> FilterableArtistTypes { get; set; }

		public NameMatchMode NameMatchMode { get; set; }

		public bool OnlyWithPVs { get; set; }

		public string Since { get; set; }

		public IPagedList<SongWithAlbumAndPVsContract> Songs { get; set; }

		public SongType SongType { get; set; }

		public SongSortRule SortRule { get; set; }

		public SongViewMode ViewMode { get; set; }

	}

	public class IndexParams {

		public IndexParams() {

			matchMode = NameMatchMode.Auto;
			page = 1;
			pageSize = 30;
			sort = SongSortRule.Name;
			songType = SongType.Unspecified;

		}

		public IndexParams(Index index, int page)
			: this() {

			ParamIs.NotNull(() => index);

			draftsOnly = index.DraftsOnly;
			filter = index.Filter;
			matchMode = index.NameMatchMode;
			onlyWithPVs = index.OnlyWithPVs;
			pageSize = Math.Min(index.Songs.PageSize, 30);
			since = index.Since;
			songType = index.SongType;
			sort = index.SortRule;
			view = index.ViewMode;
			this.page = page;

		}

		public bool draftsOnly { get; set; }

		public string filter { get; set; }

		public NameMatchMode matchMode { get; set; }

		public bool onlyWithPVs { get; set; }

		public int page { get; set; }

		public int pageSize { get; set; }

		public string since { get; set; }

		public SongType songType { get; set; }

		public SongSortRule sort { get; set; }

		public SongViewMode view { get; set; }

	}

	public enum SongViewMode {

		Details,

		Preview

	}

}