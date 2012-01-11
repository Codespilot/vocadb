using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model;

namespace VocaDb.Web.Models.SongLists {

	public class SongListEdit {

		public SongListEdit() {
			SongLinks = new List<SongInListEditContract>();
		}

		public SongListEdit(SongListForEditContract contract) {

			ParamIs.NotNull(() => contract);

			Description = contract.Description;
			Id = contract.Id;
			Name = contract.Name;
			SongLinks = contract.SongLinks;

		}

		[Display(Name = "Description")]
		[StringLength(2000)]
		public string Description { get; set; }

		public int Id { get; set; }

		[Display(Name = "Name")]
		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		[Display(Name = "Songs")]
		public IList<SongInListEditContract> SongLinks { get; set; }

		public SongListForEditContract ToContract() {

			return new SongListForEditContract {
				Description = this.Description ?? string.Empty,
				Id = this.Id,
				Name = this.Name,
				SongLinks = this.SongLinks.ToArray()
			};

		}

	}

}