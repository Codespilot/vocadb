using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Shared {

	public class ArchivedObjectVersion {

		public ArchivedObjectVersion() { }

		public ArchivedObjectVersion(ArchivedObjectVersionContract contract, string reasonName, string changeMessage) {

			Contract = contract;
			Id = contract.Id;
			Notes = contract.Notes;
			Reason = reasonName;
			Status = contract.Status;
			ChangeMessage = changeMessage;

		}

		public ArchivedObjectVersion(ArchivedObjectVersionContract contract, string changeMessage) {

			Contract = contract;
			Id = contract.Id;
			Notes = contract.Notes;
			Reason = Translate.EntryEditEventNames[contract.EditEvent];
			Status = contract.Status;
			ChangeMessage = changeMessage;

		}

		public string ChangeMessage { get; set; }

		public ArchivedObjectVersionContract Contract { get; set; }

		public int Id { get; set; }

		public string Notes { get; set; }

		public string Reason { get; set; }

		public EntryStatus Status { get; set; }

	}

}