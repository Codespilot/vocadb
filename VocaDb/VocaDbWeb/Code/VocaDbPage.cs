﻿using System.Web.Mvc;
using Remotion.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code {

	public abstract class VocaDbPage<TModel> : WebViewPage<TModel> {

		public BrandableStringsManager BrandableStrings {
			get { return DependencyResolver.Current.GetService<BrandableStringsManager>(); }
		}

		public VdbConfigManager Config {
			get { return DependencyResolver.Current.GetService<VdbConfigManager>(); }
		}

		/// <summary>
		/// Relative path to application root.
		/// 
		/// Cannot be null or empty.
		/// If the application is installed in the root folder, for example http://vocadb.net/, this will be just "/".
		/// For http://server.com/vocadb/ this would be "/vocadb/".
		/// </summary>
		public string RootPath {
			get { return Url.Content("~/"); }
		}

		public string ToJS(bool val) {
			return val ? "true" : "false";
		}

		public string ToJS(int? val) {
			return val.HasValue ? val.ToString() : "null";
		}

		public VocaUrlMapper UrlMapper {
			get {
				return new VocaUrlMapper(WebHelper.IsSSL(Request));
			}
		}

	}

	public abstract class VocaDbPage : WebViewPage {
		
		public BrandableStringsManager BrandableStrings {
			get { return DependencyResolver.Current.GetService<BrandableStringsManager>(); }
		}

		public VdbConfigManager Config {
			get { return DependencyResolver.Current.GetService<VdbConfigManager>(); }
		}

		public string RootPath {
			get { return Url.Content("~/"); }
		}

		public IUserPermissionContext UserContext {
			get { return DependencyResolver.Current.GetService<IUserPermissionContext>(); }
		}

	}

}