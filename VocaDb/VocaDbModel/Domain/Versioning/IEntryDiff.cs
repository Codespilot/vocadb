namespace VocaDb.Model.Domain.Versioning {

	public interface IEntryDiff {

		string ChangedFieldsString { get; }

		bool IsSnapshot { get; }

	}

}
