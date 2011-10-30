using System;
using System.Xml.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain {

	public class ArchivedObjectVersion {

		private string notes;

		public ArchivedObjectVersion() {
			Created = DateTime.Now;
		}

		public ArchivedObjectVersion(XDocument data, AgentLoginData author, int version, string notes)
			: this() {

			ParamIs.NotNull(() => data);
			ParamIs.NotNull(() => author);

			Data = data;
			AgentName = author.Name;
			Author = author.User;
			Notes = notes;
			Version = version;

		}

		public virtual string AgentName { get; protected set; }

		public virtual User Author { get; protected set; }

		public virtual DateTime Created { get; protected set; }

		public virtual XDocument Data { get; protected set; }

		public virtual int Id { get; protected set; }

		public virtual string Notes {
			get { return notes; }
			protected set {
				ParamIs.NotNull(() => value);
				notes = value; 
			}
		}

		public virtual int Version { get; protected set; }

	}

}
