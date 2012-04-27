using System;
using MvcPaging;

namespace VocaDb.Web.Models.Shared {

	public class PagingData<T> {

		public PagingData() { }

		public PagingData(IPagedList<T> items, object id, string action, string containerName) {

			Items = items;
			Id = id;
			Action = action;
			ContainerName = containerName;

		}

		public string Action { get; set; }

		public string ContainerName { get; set; }

		public object Id { get; set; }

		public IPagedList<T> Items { get; set; }
		 
	}
}