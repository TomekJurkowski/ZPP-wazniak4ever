var Roadkill;
(function (Roadkill) {
    (function (Site) {
        $(document).ready(function () {
            Site.Setup.bindConfirmDelete();
            AdminSetup.bindUserButtons();
        });

        var AdminSetup = (function () {
            function AdminSetup() {
            }
            AdminSetup.bindUserButtons = function () {
                $("#addadmin").click(function () {
                    $("#userdialog .title").html(ROADKILL_ADDADMIN_TITLE);
                    $("form#userform").attr("action", ROADKILL_ADDADMIN_FORMACTION);

                    $("#Id").val("{10000000-0000-0000-0000-000000000000}");
                    $("#ExistingUsername").val("");
                    $("#ExistingEmail").val("");
                    $("#NewUsername").val("");
                    $("#NewEmail").val("");
                    $("#IsBeingCreatedByAdmin").val("True");

                    $(".validation-summary-errors").hide();
                    Site.Dialogs.openModal("#userdialog");
                });

                $("#addeditor").click(function () {
                    $("#userdialog .title").html(ROADKILL_ADDEDITOR_TITLE);
                    $("form#userform").attr("action", ROADKILL_ADDEDITOR_FORMACTION);

                    $("#Id").val("{10000000-0000-0000-0000-000000000000}");
                    $("#ExistingUsername").val("");
                    $("#ExistingEmail").val("");
                    $("#NewUsername").val("");
                    $("#NewEmail").val("");
                    $("#IsBeingCreatedByAdmin").val("True");

                    $(".validation-summary-errors").hide();
                    Site.Dialogs.openModal("#userdialog");
                });

                $(".edit a").click(function () {
                    $("#userdialog .title").html(ROADKILL_EDITUSER_TITLE);
                    $("form#userform").attr("action", ROADKILL_EDITUSER_FORMACTION);

                    var user;
                    var anchor = $(this);
                    eval(anchor.attr("rel"));

                    $("#Id").val(user.id);
                    $("#ExistingUsername").val(user.username);
                    $("#ExistingEmail").val(user.email);
                    $("#NewUsername").val(user.username);
                    $("#NewEmail").val(user.email);
                    $("#IsBeingCreatedByAdmin").val("False");

                    $(".validation-summary-errors").hide();
                    Site.Dialogs.openModal("#userdialog");
                });
            };

            AdminSetup.showUserModal = function (action) {
                if (action == "addadmin") {
                    $("form#userform").attr("action", ROADKILL_ADDADMIN_FORMACTION);
                    $("#userdialog .title").html(ROADKILL_ADDADMIN_TITLE);
                } else if (action == "addeditor") {
                    $("form#userform").attr("action", ROADKILL_ADDEDITOR_FORMACTION);
                    $("#userdialog .title").html(ROADKILL_ADDEDITOR_TITLE);
                } else if (action == "edituser") {
                    $("form#userform").attr("action", ROADKILL_EDITUSER_FORMACTION);
                    $("#userdialog .title").html(ROADKILL_EDITUSER_TITLE);
                }

                Site.Dialogs.openModal("#userdialog");
            };
            return AdminSetup;
        })();
        Site.AdminSetup = AdminSetup;
    })(Roadkill.Site || (Roadkill.Site = {}));
    var Site = Roadkill.Site;
})(Roadkill || (Roadkill = {}));
