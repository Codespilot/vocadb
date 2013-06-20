
module vdb.models {

    // Song vote values.
    // Corresponds to the enum SongVoteRating.
    export enum SongVoteRating {
        Nothing     = 0,
        Like        = 3,
        Favorite    = 5,        
    }

    export function parseSongVoteRating(rating: SongVoteRating) {
        
        switch (rating) {
            case "Nothing": return SongVoteRating.Nothing;
            case "Like": return SongVoteRating.Like;
            case "Favorite": return SongVoteRating.Favorite;
        }
    
    }

}

