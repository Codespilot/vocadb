
namespace VocaDb.Model.Domain.Images {

	public interface IEntryImageInformation {

		EntryType EntryType { get; }

		int Id { get; }

		string Mime { get; }

	}

}
