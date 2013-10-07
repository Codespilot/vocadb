/// <reference path="../../DataContracts/User/UserWithIconContract.ts" />
/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../typings/underscore/underscore.d.ts" />

module vdb.viewModels {

	import dc = vdb.dataContracts;

	export class UserMessagesViewModel {

        constructor(private urlMapper: vdb.UrlMapper, data: UserWithMessagesContract) {

            this.notifications = new UserMessageFolderViewModel(urlMapper, data.notifications);
            this.receivedMessages = new UserMessageFolderViewModel(urlMapper, data.receivedMessages);
            this.sentMessages = new UserMessageFolderViewModel(urlMapper, data.sentMessages);

            var n = this.notifications.unread();

        }

        private getMessage = (message: UserMessageViewModel) => {

            var url = this.urlMapper.mapRelative("/User/MessageBody");
            $.get(url, { messageId: message.id }, (body: string) => {
                this.selectedMessageBody(body);
            });

        };

        notifications: UserMessageFolderViewModel;

        receivedMessages: UserMessageFolderViewModel;

        sentMessages: UserMessageFolderViewModel;

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

        selectedMessageBody: KnockoutObservable<string> = ko.observable("");

		selectMessage = (message: UserMessageViewModel) => {

            this.getMessage(message);

            this.receivedMessages.selectMessage(message);
            this.sentMessages.selectMessage(message);
            this.notifications.selectMessage(message);

            message.selected(true);
            message.read(true);
			this.selectedMessage(message);

		};

    }

    export class UserMessageFolderViewModel {

        constructor(private urlMapper: vdb.UrlMapper, messages: UserMessageContract[]) {

            var messageViewModels = _.map(messages, msg => new UserMessageViewModel(msg));
            this.messages(messageViewModels);
            this.unread = ko.computed(() => _.size(_.filter(messageViewModels, msg => !msg.read())));

        }

        private deleteMessage = (message: UserMessageViewModel) => {

            var url = this.urlMapper.mapRelative("/User/DeleteMessage");
            $.post(url, { messageId: message.id });
            this.messages.remove(message);

        };

        messages: KnockoutObservableArray<UserMessageViewModel> = ko.observableArray([]);

        selectMessage = (message: UserMessageViewModel) => {

            _.each(this.messages(), msg => {
                if (msg != message)
                    msg.selected(false);
            });

        };

        unread: KnockoutComputed<number>;

    }

	export class UserMessageViewModel {

		constructor(data: UserMessageContract) {
            this.created = data.createdFormatted;
            this.highPriority = data.highPriority;
            this.id = data.id;
            this.read = ko.observable(data.read);
            this.receiver = data.receiver;
			this.sender = data.sender;
			this.subject = data.subject;
		}

		created: string;

        highPriority: boolean;

		id: number;

        read: KnockoutObservable<boolean>;

        receiver: vdb.dataContracts.UserWithIconContract;

		selected = ko.observable(false);

		sender: vdb.dataContracts.UserWithIconContract;

		subject: string;

	}

    export class UserWithMessagesContract {

        notifications: UserMessageContract[];
        receivedMessages: UserMessageContract[];
        sentMessages: UserMessageContract[];

    }

	export class UserMessageContract {

        createdFormatted: string;

        highPriority: boolean;

		id: number;

        read: boolean;

        receiver: vdb.dataContracts.UserWithIconContract;

		sender: vdb.dataContracts.UserWithIconContract;

		subject: string;

	}

}