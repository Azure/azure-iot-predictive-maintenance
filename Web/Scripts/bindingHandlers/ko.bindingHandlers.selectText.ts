/// <reference path="../typings/knockout.d.ts" />

module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    ko.bindingHandlers["selectText"] = {
        init: (element: HTMLElement, valueAccessor) => {
            var target = valueAccessor();

            ko.applyBindingsToNode(element, {
                click: () => { $(target).select(); }
            });
        }
    };
}