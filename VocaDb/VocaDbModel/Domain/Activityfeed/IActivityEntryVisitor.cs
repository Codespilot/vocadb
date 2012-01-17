namespace VocaDb.Model.Domain.Activityfeed {

	public interface IActivityEntryVisitor {

		void Visit(EntryEditedEntry entry);

		void Visit(PlaintextEntry entry);

	}
}
