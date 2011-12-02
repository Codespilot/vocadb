using System;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	public class ArchivedObjectVersionContract {

		public ArchivedObjectVersionContract() {}

		public ArchivedObjectVersionContract(ArchivedObjectVersion archivedObjectVersion) {
			
			ParamIs.NotNull(() => archivedObjectVersion);

			AgentName = archivedObjectVersion.AgentName;
			Author = (archivedObjectVersion.Author != null ? new UserContract(archivedObjectVersion.Author) : null);
			Created = archivedObjectVersion.Created;
			Id = archivedObjectVersion.Id;
			IsSnapshot = archivedObjectVersion.DiffBase.IsSnapshot;
			Notes = archivedObjectVersion.Notes;
			Status = archivedObjectVersion.Status;
			Version = archivedObjectVersion.Version;

		}

		public string AgentName { get; set; }

		public UserContract Author { get; set; }

		public DateTime Created { get; set; }

		public int Id { get; set; }

		public bool IsSnapshot { get; set; }

		public string Notes { get; set; }

		public EntryStatus Status { get; set; }

		public int Version { get; set; }

	}

}
