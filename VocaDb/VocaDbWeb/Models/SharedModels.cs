using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models {

	public class ObjectCreate {

		[Required]
		[Display(Name = "Name")]
		public string Name { get; set; }

	}

}