using VocaDb.Model.Domain;

namespace VocaDb.Model.Service {

	public interface IEntryLinkFactory {

		string CreateEntryLink(IEntryBase entry);

	}
}
