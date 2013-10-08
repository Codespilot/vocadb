/// <reference path="UserMessageSummaryContract.ts" />

module vdb.dataContracts {

    export class UserMessagesContract {

        //notifications: UserMessageContract[];
        receivedMessages: UserMessageSummaryContract[];
        sentMessages: UserMessageSummaryContract[];

    }

}