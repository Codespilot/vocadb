using System;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumTagVote : TagVote {

		private AlbumTagUsage tagUsage;

		public virtual AlbumTagUsage Usage {
			get { return tagUsage; }
			set {
				ParamIs.NotNull(() => value);
				tagUsage = value; 
			}
		}

		public override TagUsage UsageBase {
			get { return Usage; }
		}

	}
}
