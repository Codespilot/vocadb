using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using VocaDb.Model.Service.BrandableStrings.Collections;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.BrandableStrings {

	public class BrandableStringsManager {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private bool LoadBrandedStrings() {
			
			var brandedStringsAssembly = AppConfig.BrandedStringsAssembly;

			if (string.IsNullOrEmpty(brandedStringsAssembly))
				return false;
			
			Assembly assembly;

			try {
				assembly = Assembly.LoadFrom(brandedStringsAssembly);
			} catch (FileNotFoundException) {
				log.Warn("Branded strings assembly '" + brandedStringsAssembly + "' not found.");
				return false;
			}
			
			var headerType = assembly.GetTypes().FirstOrDefault(t => t.GetInterface("IBrandedStringsAssemblyHeader") != null);

			if (headerType == null) {
				log.Warn("No header type found in branded strings assembly.");				
				return false;
			}

			var header = (IBrandedStringsAssemblyHeader)Activator.CreateInstance(headerType);

			Home = header.Home;
			Layout = header.Layout;

			return true;

		}

		public BrandableStringsManager() {

			if (!LoadBrandedStrings()) {
				Home = new HomeStrings(Resources.ViewRes.HomeRes.ResourceManager);				
				Layout = new LayoutStrings(Resources.ViewRes.LayoutRes.ResourceManager);				
			}

		}

		public HomeStrings Home { get; private set; }
	
		public LayoutStrings Layout { get; private set; }

	}
}
