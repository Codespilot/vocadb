using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	public class ArchivedAlbumVersionDetailsContract {

		public ArchivedAlbumVersionDetailsContract() { }

		public ArchivedAlbumVersionDetailsContract(ArchivedAlbumVersion archived, ArchivedAlbumVersion comparedVersion, 
			ContentLanguagePreference languagePreference) {

			Album = new AlbumContract(archived.Album, languagePreference);
			ArchivedVersion = new ArchivedAlbumVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ArchivedAlbumVersionContract(comparedVersion) : null; 
			ComparedVersionData = (comparedVersion != null ? ArchivedAlbumContract.GetAllProperties(comparedVersion) : null);
			ComparedVersionId = (comparedVersion != null ? comparedVersion.Id : 0);
			Data = ArchivedAlbumContract.GetAllProperties(archived);
			Name = Album.Name;

			ComparableVersions = archived.Album.ArchivedVersionsManager.Versions
				.Where(v => v != archived)
				.Select(a => new ArchivedObjectVersionContract(a))
				.ToArray();

		}

		public AlbumContract Album { get; set; }

		public ArchivedAlbumVersionContract ArchivedVersion { get; set; }

		public ArchivedObjectVersionContract[] ComparableVersions { get; set; }

		public ArchivedAlbumVersionContract ComparedVersion { get; set; }

		public ArchivedAlbumContract ComparedVersionData { get; set; }

		public int ComparedVersionId { get; set; }

		public ArchivedAlbumContract Data { get; set; }

		public string Name { get; set; }

	}

}
