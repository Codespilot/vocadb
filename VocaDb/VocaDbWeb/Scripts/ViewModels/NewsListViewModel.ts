/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />

module vdb.viewModels {

    export class NewsListViewModel {

        constructor() {

            var url = "https://public-api.wordpress.com/rest/v1/sites/blog.vocadb.net/posts/";
            $.ajax({ dataType: 'jsonp', url: url, data: { number: 3 } }).done((response: WordpressResponse) => {

                _.forEach(response.posts, post => {

                    if (post.content.length > 400) {
                        post.content = post.content.substring(0, 400) + "...";
                        post.date = new Date(post.date).toLocaleString();
                    }

                });

                this.posts(response.posts);

            }).always(() => this.loaded(true));

        }

        loaded = ko.observable(false);

        posts: KnockoutObservableArray<WordpressPost> = ko.observableArray();

    }

    export interface WordpressResponse {

        posts: WordpressPost[];

    }

    export interface WordpressPost {

        abstract: string;

        author: WordpressAuthor;

        content: string;

        date: string;

        title: string;

        URL: string;

    }

    export interface WordpressAuthor {

        avatar_URL: string;

        name: string;

    }

}