﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain {

	public class TrashedEntry {

		private XDocument data;
		private string name;
		private User user;

		public TrashedEntry() {
			Created = DateTime.Now;
		}

		public TrashedEntry(IEntryBase entry, XDocument data, User user)
			: this() {

			ParamIs.NotNull(() => entry);

			Data = data;
			EntryType = entry.EntryType;
			Name = entry.DefaultName;
			User = user;

		}

		public virtual DateTime Created { get; set; }

		public virtual XDocument Data {
			get { return data; }
			set { 
				ParamIs.NotNull(() => value);
				data = value; 
			}
		}

		public virtual EntryType EntryType { get; set; }

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set { name = value; }
		}

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value; 
			}
		}
	}

}
