﻿using System;

namespace VocaDb.Web.Code {

	public class SloganGenerator {

		private static readonly string[] slogans = { "Telling you who's whoo.", "1st place to check.", "We got APIs and ApiMikus." };

		public static string Generate() {

			var result = slogans[new Random().Next(slogans.Length)];

			return result;

		}

	}

}