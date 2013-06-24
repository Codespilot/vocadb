/// <reference path="GlobalValues.ts" />

module vdb.functions {

    export function isNullOrWhiteSpace(str: string) {

        if (str == null || str.length == 0)
            return true;

        return !(/\S/.test(str));

    }

    export function mapAbsoluteUrl(relative: string) {

        if (relative.length && relative.charAt(0) == '/')
            relative = relative.substr(1);

        return vdb.values.baseAddress + relative;

    };

    export function mapFullUrl(relative: string) {

        if (relative.length && relative.charAt(0) == '/')
            relative = relative.substr(1);

        return vdb.values.hostAddress + relative;

    };

}