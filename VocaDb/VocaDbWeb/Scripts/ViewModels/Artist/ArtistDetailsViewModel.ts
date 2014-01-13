/// <reference path="../../typings/knockout/knockout.d.ts" /> 
/// <reference path="../../Repositories/UserRepository.ts" />

module vdb.viewModels {

	import rep = vdb.repositories;

	export class ArtistDetailsViewModel {

		customizeSubscriptionDialog: CustomizeArtistSubscriptionViewModel;

		constructor(artistId: number, emailNotifications: boolean, userRepository: rep.UserRepository) {

			this.customizeSubscriptionDialog = new CustomizeArtistSubscriptionViewModel(artistId, emailNotifications, userRepository);

		}

	}

	export class CustomizeArtistSubscriptionViewModel {
		
		public dialogVisible = ko.observable(false);

		public emailNotifications: KnockoutObservable<boolean>;

		constructor(artistId: number, emailNotifications: boolean, userRepository: rep.UserRepository) {

			this.emailNotifications = ko.observable(emailNotifications);

			this.emailNotifications.subscribe(notify => {

				userRepository.updateArtistSubscription(artistId, notify);

			});

		}

		public show = () => {

			this.dialogVisible(true);

		};

	}

}