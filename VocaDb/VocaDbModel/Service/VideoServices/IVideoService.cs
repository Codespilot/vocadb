using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Model.Service.VideoServices {

	public interface IVideoService {

		bool IsValidFor(string url);

		bool IsValidFor(PVService service);

		VideoUrlParseResult ParseByUrl(string url);

		VideoUrlParseResult ParseById(string id);

	}

}
