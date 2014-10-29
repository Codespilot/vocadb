/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../Repositories/AdminRepository.ts" />

module vdb.viewModels.user {

	import dc = vdb.dataContracts;
    import rep = vdb.repositories;

    export class UserDetailsViewModel {

        public checkSFS = (ip: string) => {

            this.adminRepo.checkSFS(ip, html => {

                $("#sfsCheckDialog").html(html);
                $("#sfsCheckDialog").dialog("open");

            });

		};

		public deleteComment = (profileComment: dc.CommentContract) => {

			this.userRepo.deleteComment(profileComment.id, () => {
				this.comments.remove(profileComment);
			});

		};

		public getRatingsByGenre = (callback: (data: HighchartsOptions) => void) => {

			var url = '../User/SongsPerGenre/' + this.userId;
			$.getJSON(url, data => {
				callback(vdb.helpers.HighchartsHelper.simplePieChart(null, "Songs", data));
			});

		}

		public initComments = () => {

			if (this.comments().length)
				return;

			this.userRepo.getProfileComments(this.userId, { start: 0, maxEntries: 300, getTotalCount: false }, (result: dc.PartialFindResultContract<dc.CommentContract>) => {

				_.forEach(result.items, comment => {

					var commentAny: any = comment;
					commentAny.canBeDeleted = (this.canDeleteComments || this.userId == this.loggedUserId || (comment.author && comment.author.id == this.loggedUserId));

				});

				this.comments(result.items);

			});

		};

		public comments = ko.observableArray<dc.CommentContract>();
		public view = ko.observable("Overview");

		public setView = (viewName: string) => {
			
			switch (viewName) {
				case "Albums":
					this.albumCollectionViewModel.init();
					break;
				case "Artists":
					this.followedArtistsViewModel.init();
					break;
				case "Comments":
					this.initComments();
					break;
				case "Songs":
					this.ratedSongsViewModel.init();
					break;
			}

			window.location.hash = viewName != "Overview" ? viewName : "";
			this.view(viewName);		

		}

		public setOverview = () => this.setView("Overview");
		public setViewAlbums = () => this.setView("Albums");
		public setViewArtists = () => this.setView("Artists");
		public setComments = () => this.setView("Comments");
		public setCustomLists = () => this.setView("CustomLists");
		public setViewSongs = () => this.setView("Songs");

		constructor(
			private userId: number,
			private loggedUserId: number,
			private canDeleteComments: boolean,
			private userRepo: rep.UserRepository,
			private adminRepo: rep.AdminRepository,
			public followedArtistsViewModel: FollowedArtistsViewModel,
			public albumCollectionViewModel: AlbumCollectionViewModel,
			public ratedSongsViewModel: RatedSongsSearchViewModel) {
	        
        }

    }

}