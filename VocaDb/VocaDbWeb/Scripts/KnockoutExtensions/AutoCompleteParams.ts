
module vdb.knockoutExtensions {

    export interface AutoCompleteParams {

        acceptSelection?: (id: number, term: string) => void;

        allowCreateNew?: boolean;

        createNewItem?: string;

        extraQueryParams?;

        filter?: (any) => boolean;

        height?: number;

    }

}

