using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoUrlParseResult {

		public VideoUrlParseResult(PVService service, string id, string title) {
			Service = service;
			Id = id;
			Title = title;
		}

		public string Id { get; set; }

		public PVService Service { get; set; }

		public string Title { get; set; }

	}

}
