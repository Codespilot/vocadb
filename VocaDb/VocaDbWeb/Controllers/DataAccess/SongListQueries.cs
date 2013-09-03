using NLog;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	public class SongListQueries {

		private readonly IEntryLinkFactory entryLinkFactory;
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly IUserPermissionContext permissionContext;
		private readonly ISongListRepository repository;

		private IUserPermissionContext PermissionContext {
			get { return permissionContext; }
		}

		private User GetLoggedUser(IRepositoryContext<SongList> ctx) {

			permissionContext.VerifyLogin();

			return ctx.OfType<User>().Load(permissionContext.LoggedUser.Id);

		}

		private SongList CreateSongList(IRepositoryContext<SongList> ctx, SongListForEditContract contract, UploadedFileContract uploadedFile) {

			var user = GetLoggedUser(ctx);
			var newList = new SongList(contract.Name, user);
			newList.Description = contract.Description;

			if (EntryPermissionManager.CanManageFeaturedLists(permissionContext))
				newList.FeaturedCategory = contract.FeaturedCategory;

			ctx.Save(newList);

			var songDiff = newList.SyncSongs(contract.SongLinks, c => ctx.OfType<Song>().Load(c.SongId));
			ctx.OfType<SongInList>().Sync(songDiff);

			SetThumb(newList, uploadedFile);

			ctx.Update(newList);

			ctx.AuditLogger.AuditLog(string.Format("created song list {0}", entryLinkFactory.CreateEntryLink(newList)), user);

			return newList;

		}

		private void SetThumb(SongList list, UploadedFileContract uploadedFile) {

			if (uploadedFile != null) {

				var thumb = new EntryThumb(list, uploadedFile.Mime);
				list.Thumb = thumb;
				var thumbGenerator = new ImageThumbGenerator(new ServerImagePathMapper());
				thumbGenerator.GenerateThumbsAndMoveImage(uploadedFile.Stream, thumb, ImageSizes.Original | ImageSizes.SmallThumb, originalSize: 500);

			}

		}

		public SongListQueries(ISongListRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) {
			this.repository = repository;
			this.permissionContext = permissionContext;
			this.entryLinkFactory = entryLinkFactory;
		}

		public int UpdateSongList(SongListForEditContract contract, UploadedFileContract uploadedFile) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return repository.HandleTransaction(ctx => {

				var user = GetLoggedUser(ctx);
				SongList list;

				if (contract.Id == 0) {

					list = CreateSongList(ctx, contract, uploadedFile);

				} else {

					list = ctx.Load(contract.Id);

					EntryPermissionManager.VerifyEdit(PermissionContext, list);

					list.Description = contract.Description;
					list.Name = contract.Name;

					if (EntryPermissionManager.CanManageFeaturedLists(PermissionContext))
						list.FeaturedCategory = contract.FeaturedCategory;

					var songDiff = list.SyncSongs(contract.SongLinks, c => ctx.OfType<Song>().Load(c.SongId));
					ctx.OfType<SongInList>().Sync(songDiff);
					SetThumb(list, uploadedFile);

					ctx.Update(list);

					ctx.AuditLogger.AuditLog(string.Format("updated song list {0}", entryLinkFactory.CreateEntryLink(list)), user);

				}

				return list.Id;

			});

		}

	}

}