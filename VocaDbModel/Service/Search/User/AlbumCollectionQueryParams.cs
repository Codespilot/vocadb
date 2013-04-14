using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.User {

	public class AlbumCollectionQueryParams {

		public AlbumCollectionQueryParams(int userId, PagingProperties paging) {

			ParamIs.NotNull(() => paging);

			Paging = paging;
			UserId = userId;

			FilterByStatus = PurchaseStatus.Nothing;

		}

		public PurchaseStatus FilterByStatus { get; set; }

		/// <summary>
		/// Paging properties. Cannot be null.
		/// </summary>
		public PagingProperties Paging { get; set; }

		/// <summary>
		/// Id of the user whose albums to get.
		/// </summary>
		public int UserId { get; set; }


	}

}
