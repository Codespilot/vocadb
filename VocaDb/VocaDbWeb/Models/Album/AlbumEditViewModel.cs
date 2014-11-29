using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Album {

	[PropertyModelBinder]
	public class AlbumEditViewModel {

		public AlbumEditViewModel() {
			
			AllDiscTypes = Translate.DiscTypeNames.ValuesAndNamesStrings;

			DiscTypeDescriptions = ViewRes.Album.EditStrings.BaDiscTypeExplanation 
			                       + "<br /><br /><ul>" + string.Join("", 
				                       EnumVal<DiscType>.Values.Where(v => v != DiscType.Unknown).Select(v => string.Format("<li><strong>{0}</strong>: {1}</li>", 
					                       Translate.DiscTypeName(v), global::Resources.DiscTypeDescriptions.ResourceManager.GetString(v.ToString()))));


		}

		public AlbumEditViewModel(AlbumContract album, IUserPermissionContext permissionContext,
			AlbumForEditContract editedAlbum = null)
			: this() {

			ParamIs.NotNull(() => album);

			Album = album;
			EditedAlbum = editedAlbum;

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(permissionContext);

		}

		public AlbumContract Album { get; set; }

		public Dictionary<string, string> AllDiscTypes { get; set; }

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		public string DiscTypeDescriptions { get; set; }

		public bool Draft {
			get {
				return Album != null && Album.Status == EntryStatus.Draft;
			}
		}

		[FromJson]
		public AlbumForEditContract EditedAlbum { get; set; }

		public bool HasCoverPicture {
			get {
				return Album != null && !string.IsNullOrEmpty(Album.CoverPictureMime);
			}
		}

		public int Id {
			get { return Album != null ? Album.Id : 0; }
		}

		public string Name {
			get { return Album != null ? Album.Name : null; }
		}

		public void CheckModel() {

			if (EditedAlbum.ArtistLinks == null)
				throw new InvalidFormException("Artists list was null");

			if (EditedAlbum.Identifiers == null)
				throw new InvalidFormException("Identifiers list was null");

			if (EditedAlbum.Names == null)
				throw new InvalidFormException("Names list was null");

			if (EditedAlbum.Songs == null)
				throw new InvalidFormException("Tracks list was null");

		}

	}
}