﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model;

namespace VocaDb.Web.Models {

	public class ObjectCreate {

		[Required]
		[Display(Name = "Name")]
		public string Name { get; set; }

	}

	public class LocalizedStringEdit {

		public LocalizedStringEdit() { }

		public LocalizedStringEdit(LocalizedStringWithIdContract contract) {

			ParamIs.NotNull(() => contract);

			Id = contract.Id;
			Language = contract.Language;
			Value = contract.Value;
			AllLanguages = EnumVal<ContentLanguageSelection>.Values;

		}

		public ContentLanguageSelection[] AllLanguages { get; set; }

		public int Id { get; set; }

		[Required]
		[Display(Name = "Language")]
		public ContentLanguageSelection Language { get; set; }

		[Required]
		[Display(Name = "Value")]
		public string Value { get; set; }

	}

	public class AddNewLinkRowModel {

		public string EntityName { get; set; }

		public string Prefix { get; set; }

		public string SearchUrl { get; set; }

	}

}