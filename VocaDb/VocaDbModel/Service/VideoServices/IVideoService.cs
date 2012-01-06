using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public interface IVideoService {

		bool IsValidFor(string url);

		bool IsValidFor(PVService service);

		VideoUrlParseResult ParseByUrl(string url);

		VideoUrlParseResult ParseById(string id);

	}

}
