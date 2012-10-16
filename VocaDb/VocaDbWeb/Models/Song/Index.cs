using System.Collections.Generic;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models.Song {

	public class Index {

		public Index() {}

		public Index(PartialFindResult<SongWithAlbumAndPVsContract> result, string filter, SongType songType, string timeFilter, bool onlyWithPVs, SongSortRule sortRule, SongViewMode viewMode, 
			bool? draftsOnly, int? page, int pageSize) {

			DraftsOnly = draftsOnly ?? false;
			Songs = new StaticPagedList<SongWithAlbumAndPVsContract>(result.Items, 
				page ?? 1, pageSize, result.TotalCount);
			Filter = filter;
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

		public bool OnlyWithPVs { get; set; }

		public IPagedList<SongWithAlbumAndPVsContract> Songs { get; set; }

		public SongType SongType { get; set; }

		public SongSortRule SortRule { get; set; }

		public string Since { get; set; }

		public SongViewMode ViewMode { get; set; }

	}

	public enum SongViewMode {

		Details,

		Preview

	}

}