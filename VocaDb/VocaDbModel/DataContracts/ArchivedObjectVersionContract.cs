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
			Version = archivedObjectVersion.Version;

		}

		public string AgentName { get; set; }

		public UserContract Author { get; set; }

		public DateTime Created { get; set; }

		public int Id { get; set; }

		public int Version { get; set; }

	}

}
