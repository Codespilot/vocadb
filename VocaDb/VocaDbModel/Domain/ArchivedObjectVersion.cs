using System;
using System.Xml.Linq;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain {

	public class ArchivedObjectVersion {

		public ArchivedObjectVersion() {
			Created = DateTime.Now;
		}

		public ArchivedObjectVersion(XDocument data, AgentLoginData author, int version)
			: this() {

			ParamIs.NotNull(() => data);
			ParamIs.NotNull(() => author);

			Data = data;
			AgentName = author.Name;
			Author = author.User;
			Version = version;

		}

		public virtual string AgentName { get; protected set; }

		public virtual User Author { get; protected set; }

		public virtual DateTime Created { get; protected set; }

		public virtual XDocument Data { get; protected set; }

		public virtual int Id { get; protected set; }

		public virtual int Version { get; protected set; }

	}

}
