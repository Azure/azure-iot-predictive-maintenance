/// <reference path="../typings/knockout.d.ts" />
/// <reference path="../charts/linechart.ts" />

module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    ko.bindingHandlers["lineChart"] = {
        init: (element: HTMLElement, valueAccessor) => {
            $(element).addClass("chart-indicator-progress");

            var sourceObservable = valueAccessor();

            var lineChart = new LineChart(element);

            var onChartDataReceived = chartData => {
                if (!chartData || !chartData.categories || !chartData.line1values || !chartData.line2values) {
                    return;
                }

                lineChart.updateChartData(chartData.categories, chartData.line1values, chartData.line2values);
                $(element).removeClass("chart-indicator-progress");
            }

            sourceObservable.subscribe(onChartDataReceived);

            $(window).resize(() => {
                lineChart.resizeViewport();
            });

            onChartDataReceived(sourceObservable());
        }
    };
}