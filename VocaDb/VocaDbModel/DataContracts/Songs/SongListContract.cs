using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongListContract {

		public SongListContract() { }

		public SongListContract(SongList list) {

			ParamIs.NotNull(() => list);

			Description = list.Description;
			Id = list.Id;
			Name = list.Name;

		}

		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

	}
}
