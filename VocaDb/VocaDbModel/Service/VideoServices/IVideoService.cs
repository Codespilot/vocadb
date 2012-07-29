using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public interface IVideoService {

		bool IsValidFor(string url);

		bool IsValidFor(PVService service);

		VideoUrlParseResult ParseByUrl(string url, bool getTitle);

		VideoUrlParseResult ParseById(string id, string url, bool getTitle);

	}

}
