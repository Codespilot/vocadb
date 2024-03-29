﻿using System;
using System.Net;
using System.Web;
using NLog;

namespace VocaDb.Web.Code {

	public static class ErrorLogger {

		public const int Code_Forbidden = (int)HttpStatusCode.Forbidden;
		public const int Code_NotFound = (int)HttpStatusCode.NotFound;
		public const int Code_InternalServerError = (int)HttpStatusCode.InternalServerError;

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public static void LogHttpError(HttpRequestBase request, int code, LogLevel level = null) {

			log.Log(level ?? LogLevel.Warn, RequestInfo(string.Format("HTTP error code {0} for", code), request));

		}

		public static void LogHttpError(HttpRequest request, int code, LogLevel level = null) {

			LogHttpError(new HttpRequestWrapper(request), code, level);

		}

		public static void LogException(HttpRequest request, Exception ex, LogLevel level = null) {

			log.LogException(level ?? LogLevel.Error, RequestInfo("Exception for", new HttpRequestWrapper(request)), ex);

		}

		public static void LogMessage(HttpRequestBase request, string msg, LogLevel level = null) {

			log.Log(level ?? LogLevel.Error, RequestInfo(msg + " for", request));

		}

		public static void LogMessage(HttpRequest request, string msg, LogLevel level = null) {

			LogMessage(new HttpRequestWrapper(request), msg, level);

		}

		public static string RequestInfo(string msg, HttpRequestBase request) {

			return string.Format("{0} '{1}' [{2}], URL {3} '{4}', UA '{5}', referrer '{6}'",
				msg, request.UserHostAddress, request.UserHostName, request.HttpMethod, request.Unvalidated.RawUrl, request.UserAgent, request.UrlReferrer);

		}

	}

}