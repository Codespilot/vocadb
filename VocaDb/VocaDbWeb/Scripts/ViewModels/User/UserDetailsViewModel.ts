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

		public initComments = () => {

			if (this.comments().length)
				return;

			this.userRepo.getProfileComments(this.loggedUserId, { start: 0, maxEntries: 300, getTotalCount: false }, (result: dc.PartialFindResultContract<dc.CommentContract>) => {

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
			private loggedUserId: number,
			private userRepo: rep.UserRepository,
			private adminRepo: rep.AdminRepository,
			public followedArtistsViewModel: FollowedArtistsViewModel,
			public albumCollectionViewModel: AlbumCollectionViewModel,
			public ratedSongsViewModel: RatedSongsSearchViewModel) {
	        
        }

    }

}