using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using VocaDb.Model;

namespace VocaDb.Web.Helpers.Support {

	public class TranslateableEnum<TEnum> where TEnum : struct, IConvertible {

		private readonly Func<ResourceManager> resourceManager;

		public TranslateableEnum(Func<ResourceManager> resourceManager) {
			this.resourceManager = resourceManager;
		}

		public string this[TEnum val] {
			get {
				return GetName(val);
			}
		}

		public Dictionary<TEnum, string> ValuesAndNames {
			get {
				return Values.ToDictionary(t => t, GetName);
			}
		}

		public TEnum[] Values {
			get { 
				return EnumVal<TEnum>.Values; 
			} 
		}

		public string GetName(TEnum val) {
			return resourceManager().GetString(val.ToString());
		}

	}
}