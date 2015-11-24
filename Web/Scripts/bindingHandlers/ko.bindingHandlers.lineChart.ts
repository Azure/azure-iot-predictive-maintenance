/// <reference path="../typings/knockout.d.ts" />
/// <reference path="../charts/linechart.ts" />

module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    ko.bindingHandlers["lineChart"] = {
        init: (element: HTMLElement, valueAccessor) => {
            $(element).addClass("chart-indicator-progress");

            var sourceObservable = valueAccessor();

            var lineChart = new LineChart(element);

            var update = chartData => {
                if (!chartData) {
                    return;
                }

                lineChart.updateChartData(chartData.categories, chartData.line1values, chartData.line2values);
                $(element).removeClass("chart-indicator-progress");
            }

            sourceObservable.subscribe(update);

            $(window).resize(() => {
                lineChart.resizeViewport();
            });

            update(sourceObservable());
        }
    };
}