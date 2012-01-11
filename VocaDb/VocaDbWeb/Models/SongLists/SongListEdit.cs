using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model;

namespace VocaDb.Web.Models.SongLists {

	public class SongListEdit {

		public SongListEdit() {
			SongLinks = new List<SongInListContract>();
		}

		public SongListEdit(SongListDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			Description = contract.Description;
			Id = contract.Id;
			Name = contract.Name;

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
		public IList<SongInListContract> SongLinks { get; set; }

	}

}