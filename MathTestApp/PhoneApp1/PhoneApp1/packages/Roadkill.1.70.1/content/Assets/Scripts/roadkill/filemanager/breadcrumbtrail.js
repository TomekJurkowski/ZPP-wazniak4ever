var Roadkill;
(function (Roadkill) {
    (function (Site) {
        (function (FileManager) {
            var BreadCrumbTrail = (function () {
                function BreadCrumbTrail() {
                }
                BreadCrumbTrail.removeLastItem = function () {
                    var item = $("ul.navigator li:last-child");
                    var level = item.attr("data-level");

                    if (level == 0)
                        $("ul.navigator li").remove(); else
                        $("ul.navigator li:gt(" + (level - 1) + ")").remove();
                };

                BreadCrumbTrail.removePriorBreadcrumb = function () {
                    var count = $("ul.navigator li").length;
                    if (count == 1)
                        return;

                    var li = $("ul.navigator li:last-child").prev("li");
                    var level = li.attr("data-level");

                    this.removeLastItem(level);
                };

                BreadCrumbTrail.addNewItem = function (data) {
                    var htmlBuilder = new FileManager.HtmlBuilder();
                    var count = $("ul.navigator li").length;
                    var breadCrumbHtml = htmlBuilder.getBreadCrumb(data, count);

                    $("ul.navigator").append(breadCrumbHtml);
                    $("li[data-urlpath='" + data.UrlPath + "'] a").on("click", function () {
                        var li = $(this).parent();
                        li.nextAll().remove();
                        FileManager.TableEvents.update(li.attr("data-urlpath"), false);
                    });
                };
                return BreadCrumbTrail;
            })();
            FileManager.BreadCrumbTrail = BreadCrumbTrail;
        })(Site.FileManager || (Site.FileManager = {}));
        var FileManager = Site.FileManager;
    })(Roadkill.Site || (Roadkill.Site = {}));
    var Site = Roadkill.Site;
})(Roadkill || (Roadkill = {}));
