/// <reference path="typings/knockout.d.ts" />
/// <reference path="charts/linechart.ts" />

module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    ko.bindingHandlers["sortBy"] = {
        init: (element: HTMLElement, valueAccessor) => {
            var options = valueAccessor();
            var column = options.column;
            var collection = options.collection;

            $(element).addClass("sorting");

            collection.sortBy.subscribe((columnName: string) => {
                if (column != columnName) {
                    $(element).removeClass("sorting_asc");
                    $(element).removeClass("sorting_desc");
                }
            });

            $(element).click(() => {
                collection.sortBy(column);
                collection.sortAccending(!collection.sortAccending());

                $(element).removeClass("sorting_asc");
                $(element).removeClass("sorting_desc");

                if (collection.sortAccending()) {
                    $(element).addClass("sorting_asc").attr("aria-sort", "ascending");
                }
                else {
                    $(element).addClass("sorting_desc").attr("aria-sort", "descending");
                }
            });
        }
    };
}