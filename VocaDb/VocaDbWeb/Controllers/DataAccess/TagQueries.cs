using System;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Search;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries for <see cref="Tag"/>.
	/// </summary>
	public class TagQueries : QueriesBase<ITagRepository, Tag> {

		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEntryImagePersisterOld imagePersister;

		private Tag GetRealTag(IRepositoryContext<Tag> ctx, string tagName, Tag ignoreSelf) {

			if (string.IsNullOrEmpty(tagName))
				return null;

			if (ignoreSelf != null && Tag.Equals(ignoreSelf, tagName))
				return null;

			return ctx.Query().FirstOrDefault(t => t.AliasedTo == null && t.Name == tagName);

		}

		public TagQueries(ITagRepository repository, IUserPermissionContext permissionContext,
		                  IEntryLinkFactory entryLinkFactory, IEntryImagePersisterOld imagePersister)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;

		}

		public void Archive(IRepositoryContext<Tag> ctx, Tag tag, TagDiff diff, EntryEditEvent reason) {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = tag.CreateArchivedVersion(diff, agentLoginData, reason);
			ctx.OfType<ArchivedTagVersion>().Save(archived);

		}

		public void Delete(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			repository.HandleTransaction(ctx => {

				var tag = ctx.Load(name);

				tag.Delete();

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", tag));

				ctx.Delete(tag);

			});

		}

		public PartialFindResult<T> Find<T>(Func<Tag, T> fac, CommonSearchParams queryParams, PagingProperties paging, 
			bool allowAliases = false, string categoryName = "")
			where T : class {

			var matchMode = queryParams.NameMatchMode;
			queryParams.Query = FindHelpers.GetMatchModeAndQueryForSearch(queryParams.Query ?? string.Empty, ref matchMode, NameMatchMode.Partial);
			queryParams.Query = queryParams.Query.Replace(' ', '_');

			return HandleQuery(ctx => {

				var query = ctx.Query()
					.WhereHasName(queryParams.Query, matchMode)
					.WhereAllowAliases(allowAliases)
					.WhereHasCategoryName(categoryName);

				var tags = query
					.OrderBy(t => t.Name)
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray();

				var count = 0;

				if (paging.GetTotalCount) {
					
					count = query.Count();

				}

				var result = tags.Select(fac).ToArray();

				return new PartialFindResult<T>(result, count, queryParams.Query, false);

			});

		}

		public string[] FindNames(string query, bool allowAliases, bool allowEmptyName, int maxEntries) {

			if (!allowEmptyName && string.IsNullOrWhiteSpace(query))
				return new string[] { };

			query = query != null ? query.Trim().Replace(' ', '_') : string.Empty;

			return HandleQuery(session => {

				var q = session.Query();

				if (query.Length < 3)
					q = q.Where(t => t.Name.StartsWith(query));
				else
					q = q.Where(t => t.Name.Contains(query));

				if (!allowAliases)
					q = q.Where(t => t.AliasedTo == null);

				var tags = q
					.OrderBy(t => t.Name)
					.Take(maxEntries)
					.Select(t => t.Name)
					.ToArray();

				return tags;

			});

		}

		public void Update(TagContract contract, UploadedFileContract uploadedImage) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

			repository.HandleTransaction(ctx => {

				var tag = ctx.Load(contract.Name);

				permissionContext.VerifyEntryEdit(tag);

				var diff = new TagDiff();

				var newAliasedTo = contract.AliasedTo ?? string.Empty;
				if (!Tag.Equals(tag.AliasedTo, contract.AliasedTo)) {
					diff.AliasedTo = true;
					tag.AliasedTo = GetRealTag(ctx, newAliasedTo, tag);
				}

				if (tag.CategoryName != contract.CategoryName)
					diff.CategoryName = true;

				if (tag.Description != contract.Description)
					diff.Description = true;

				if (!Tag.Equals(tag.Parent, contract.Parent)) {

					var newParent = GetRealTag(ctx, contract.Parent, tag);

					if (!Equals(newParent, tag.Parent)) {
						diff.Parent = true;
						tag.SetParent(newParent);						
					}

				}

				if (tag.Status != contract.Status)
					diff.Status = true;

				tag.CategoryName = contract.CategoryName;
				tag.Description = contract.Description;
				tag.Status = contract.Status;

				if (uploadedImage != null) {

					diff.Picture = true;

					var thumb = new EntryThumb(tag, uploadedImage.Mime);
					tag.Thumb = thumb;
					var thumbGenerator = new ImageThumbGenerator(imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(uploadedImage.Stream, thumb, ImageSizes.Original | ImageSizes.SmallThumb, originalSize: 500);

				}

				var logStr = string.Format("updated properties for tag {0} ({1})", entryLinkFactory.CreateEntryLink(tag), diff.ChangedFieldsString);
				ctx.AuditLogger.AuditLog(logStr);
				Archive(ctx, tag, diff, EntryEditEvent.Updated);

				ctx.Update(tag);

			});

		}


	}

}