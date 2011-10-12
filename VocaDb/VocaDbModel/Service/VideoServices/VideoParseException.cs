﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoParseException : Exception {

		public VideoParseException() { }

		public VideoParseException(string message)
			: base(message) { }

		public VideoParseException(string message, Exception inner)
			: base(message, inner) { }

	}

}
