/// <reference path="../SongContract.ts" />

module vdb.dataContracts {

    export interface SongWithPVPlayerAndVoteContract {
        
        playerHtml: string;

        song: SongWithComponentsContract;
    
    }

}