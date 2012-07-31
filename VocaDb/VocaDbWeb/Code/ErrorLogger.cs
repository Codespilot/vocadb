using System;
using System.Web;
using NLog;

namespace VocaDb.Web.Code {

	public static class ErrorLogger {

		public const int Code_Forbidden = 403;
		public const int Code_NotFound = 404;
		public const int Code_InternalServerError = 500;

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static string RequestInfo(string msg, HttpRequestBase request) {

			return string.Format("{0} '{1}' [{2}], URL '{3}', UA '{4}', referrer '{5}'",
				msg, request.UserHostAddress, request.UserHostName, request.RawUrl, request.UserAgent, request.UrlReferrer);

		}

		public static void LogHttpError(HttpRequestBase request, int code) {

			log.Warn(RequestInfo(string.Format("HTTP error code {0} for", code), request));

		}

		public static void LogHttpError(HttpRequest request, int code) {

			log.Warn(RequestInfo(string.Format("HTTP error code {0} for", code), new HttpRequestWrapper(request)));

		}

		public static void LogException(HttpRequest request, Exception ex) {

			log.ErrorException(RequestInfo("Exception for", new HttpRequestWrapper(request)), ex);

		}

	}

}