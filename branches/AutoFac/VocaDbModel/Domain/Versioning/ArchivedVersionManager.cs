using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Versioning {

	public class ArchivedVersionManager<TVersion, TField> 
		where TVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<TField> 
		where TField : struct, IConvertible {

		private IList<TVersion> archivedVersions = new List<TVersion>();

		public virtual IList<TVersion> Versions {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual TVersion Add(TVersion newVersion) {

			ParamIs.NotNull(() => newVersion);

			Versions.Add(newVersion);
			return newVersion;

		}

		public virtual void Clear() {
			Versions.Clear();
		}

		public virtual TVersion GetLatestVersion() {
			return Versions.OrderByDescending(m => m.Created).FirstOrDefault();
		}

		public virtual TVersion GetLatestVersionWithField(TField field, int lastVersion) {

			return Versions
				.Where(a => a.Version <= lastVersion && a.IsIncluded(field))
				.OrderByDescending(m => m.Version)
				.FirstOrDefault();

		}

	}

}
