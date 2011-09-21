using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract]
	public class SongWithAdditionalNamesContract : SongContract {

		public SongWithAdditionalNamesContract(Song song)
			: base(song) {

			AdditionalNames = string.Join(", ", song.AllNames.Where(n => n != song.Name));

		}

		[DataMember]
		public string AdditionalNames { get; private set; }

	}
}
