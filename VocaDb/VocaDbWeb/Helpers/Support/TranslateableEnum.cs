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
				return GetValuesAndNames(Values);
			}
		}

		public TEnum[] Values {
			get { 
				return EnumVal<TEnum>.Values; 
			} 
		}

		public string GetAllNameNames(TEnum flags, params TEnum[] except) {

			return string.Join(", ", Values.Where(f => !except.Contains(f) && EnumVal<TEnum>.FlagIsSet(flags, f)).Select(f => GetName(f)));

		}

		public string GetName(TEnum val) {
			return resourceManager().GetString(val.ToString());
		}

		public Dictionary<TEnum, string> GetValuesAndNames(TEnum[] values) {
			return values.ToDictionary(t => t, GetName);
		}

	}
}