using System;
using System.Web.Routing;
using MvcPaging;

namespace VocaDb.Web.Models.Shared {

	public interface IPagingData {

		string Action { get; }

		string ContainerName { get; }

		object Id { get; }

		IPagedList ItemsBase { get; }

		RouteValueDictionary RouteValues { get; }

	}

	public class PagingData<T> : IPagingData {

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

		public RouteValueDictionary RouteValues { get; set; } 

		public IPagedList ItemsBase {
			get { return Items; }
		}
	}
}