/// <reference path="../../DataContracts/User/UserWithIconContract.ts" />
/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../typings/underscore/underscore.d.ts" />

module vdb.viewModels {

	import dc = vdb.dataContracts;

	export class UserMessagesViewModel {

		constructor(private urlMapper: vdb.UrlMapper, messages: UserMessageContract[]) {

			var messageViewModels = _.map(messages, msg => new UserMessageViewModel(msg));
			this.messages(messageViewModels);

		}

		private deleteMessage = (message: UserMessageViewModel) => {

			var url = this.urlMapper.mapRelative("/User/DeleteMessage");
			$.post(url, { messageId: message.id });
			this.messages.remove(message);

		};

		private getMessage = (message: UserMessageViewModel) => {

			// TODO: get body

		};

		messages: KnockoutObservableArray<UserMessageViewModel> = ko.observableArray([]);

		reply = () => {

			if (!this.selectedMessage())
				throw Error("No message selected");

			var msg = this.selectedMessage();
			$("#receiverName").val(msg.sender.name);

			$("#newMessageSubject").val("Re: " + msg.subject);

			var index = $('#tabs ul').index($('#composeTab'));
			$("#tabs").tabs("option", "active", index);

		};

		selectedMessage: KnockoutObservable<UserMessageViewModel> = ko.observable();

		selectMessage = (message: UserMessageViewModel) => {

			_.each(this.messages(), msg => {
				if (msg != message)
					msg.selected(false);
			});

			message.selected(true);
			this.selectedMessage(message);

		};

	}

	export class UserMessageViewModel {

		constructor(data: UserMessageContract) {
			this.date = data.date;
			this.id = data.id;
			this.sender = data.sender;
			this.subject = data.subject;
		}

		date: Date;

		id: number;

		selected = ko.observable(false);

		sender: vdb.dataContracts.UserWithIconContract;

		subject: string;

	}

	export class UserMessageContract {

		date: Date;

		id: number;

		sender: vdb.dataContracts.UserWithIconContract;

		subject: string;

	}

}