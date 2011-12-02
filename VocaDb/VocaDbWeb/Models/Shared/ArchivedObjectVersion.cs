using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Models.Shared {

	public class ArchivedObjectVersion {

		public ArchivedObjectVersion() { }

		public ArchivedObjectVersion(ArchivedObjectVersionContract contract, string reasonName, string changeMessage) {

			Contract = contract;
			Id = contract.Id;
			Reason = reasonName;
			Status = contract.Status;
			ChangeMessage = changeMessage;

		}

		public string ChangeMessage { get; set; }

		public ArchivedObjectVersionContract Contract { get; set; }

		public int Id { get; set; }

		public string Reason { get; set; }

		public EntryStatus Status { get; set; }

	}

}