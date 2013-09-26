/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../Repositories/AdminRepository.ts" />

module vdb.viewModels {

    import rep = vdb.repositories;

    export class UserDetailsViewModel {

        public checkSFS = (ip: string) => {

            this.adminRepo.checkSFS(ip, html => {

                $("#sfsCheckDialog").html(html);
                $("#sfsCheckDialog").dialog("open");

            });

        };

        constructor(private adminRepo: rep.AdminRepository) { }

    }

}