using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries for <see cref="Tag"/>.
	/// </summary>
	public class TagQueries : QueriesBase<ITagRepository> {

		public TagQueries(ITagRepository repository, IUserPermissionContext permissionContext) 
			: base(repository, permissionContext) {}

		public void Archive(IRepositoryContext<Tag> ctx, Tag tag, TagDiff diff, EntryEditEvent reason) {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = tag.CreateArchivedVersion(diff, agentLoginData, reason);
			ctx.OfType<ArchivedTagVersion>().Save(archived);

		}

		public void UpdateTag(TagContract contract, UploadedFileContract uploadedImage) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

			repository.HandleTransaction(ctx => {

				var tag = ctx.Load(contract.Name);
				var diff = new TagDiff();

				var oldAliasedTo = tag.AliasedTo != null ? tag.AliasedTo.Name : string.Empty;
				var newAliasedTo = contract.AliasedTo ?? string.Empty;
				if (oldAliasedTo != newAliasedTo) {
					diff.AliasedTo = true;
					tag.AliasedTo = ctx.Query().FirstOrDefault(t => t.AliasedTo == null && t.Name == newAliasedTo);
				}

				if (tag.CategoryName != contract.CategoryName)
					diff.CategoryName = true;

				if (tag.Description != contract.Description)
					diff.Description = true;

				tag.CategoryName = contract.CategoryName;
				tag.Description = contract.Description;

				if (uploadedImage != null) {

					diff.Picture = true;

					var thumb = new EntryThumb(tag, uploadedImage.Mime);
					tag.Thumb = thumb;
					var thumbGenerator = new ImageThumbGenerator(new ServerImagePathMapper());
					thumbGenerator.GenerateThumbsAndMoveImage(uploadedImage.Stream, thumb, ImageSizes.Original | ImageSizes.SmallThumb, originalSize: 500);

				}

				var logStr = string.Format("updated properties for {0} ({1})", tag, diff.ChangedFieldsString);
				ctx.AuditLogger.AuditLog(logStr);
				Archive(ctx, tag, diff, EntryEditEvent.Updated);

				ctx.Update(tag);

			});

		}


	}

}