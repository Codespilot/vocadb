/// <reference path="EntryRefContract.ts" />

module vdb.dataContracts {

    export interface DuplicateEntryResultContract {

        entry: EntryRefWithNameContract;

        matchProperty: string;

    }

    export interface EntryNameContract {

        additionalNames: string;

        displayName: string;

    }

    export interface EntryRefWithNameContract extends EntryRefContract {

        entryTypeName: string;

        name: EntryNameContract;

    }

}
