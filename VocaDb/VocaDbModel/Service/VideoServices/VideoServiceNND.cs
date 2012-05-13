﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceNND : VideoService {

		private static readonly Regex numIdRegex = new Regex(@"nicovideo.jp/watch/(\d{6,12})");

		public VideoServiceNND(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}


		public override string GetThumbUrlById(string id) {

			var numId = numIdRegex.Match(id);

			if (!numId.Success)
				return null;

			return string.Format("http://tn-skr1.smilevideo.jp/smile?i={0}", numId.Value);

		}

	}
}
