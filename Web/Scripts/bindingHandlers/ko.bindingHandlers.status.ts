/// <reference path="../typings/knockout.d.ts" />

module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    ko.bindingHandlers["status"] = {
        init: (element: HTMLElement, valueAccessor) => {
            var status = valueAccessor();
            var statusClass;

            switch (status) {
                case "Running":
                    statusClass = "status_true";
                    break;
                case "Pending":
                    statusClass = "status_pending";
                    break;
                case "Disabled":
                    statusClass = "status_false";
                    break;
                default:
                    throw new Error("Unknown device state.");
            }

            ko.applyBindingsToNode(element, { css: statusClass });
        }
    };
}