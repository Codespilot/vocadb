using System;
using VocaDb.Web.Resources.Views.Shared;

namespace VocaDb.Web.Code {

	public static class TimeAgoStringBuilder {

		public static string FormatTimeAgo(TimeSpan timeSpan) {

			if (timeSpan.Days > 1)
				return string.Format(TimeStrings.TimeAgo, (int)timeSpan.TotalDays, TimeStrings.Days);

			if (timeSpan.Hours > 1)
				return string.Format(TimeStrings.TimeAgo, (int)timeSpan.TotalHours, TimeStrings.Hours);

			return string.Format(TimeStrings.TimeAgo, (int)timeSpan.TotalMinutes, TimeStrings.Minutes);

		}

	}
}