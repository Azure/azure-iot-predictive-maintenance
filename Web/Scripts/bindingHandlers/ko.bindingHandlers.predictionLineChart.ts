/// <reference path="../typings/knockout.d.ts" />
/// <reference path="../charts/linechart.ts" />

module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export class PredictionLineChartBinding {
        constructor() {
            this.init = this.init.bind(this);
            this.createDataView = this.createDataView.bind(this);
        }

        private createDataView(categories, line1values, line2values) {
            var fieldExpr = powerbi.data.SQExprBuilder.fieldExpr({ column: { schema: "s", entity: "table2", name: "cycle" } });

            var categoryIdentities = categories.map(value => {
                var expr = powerbi.data.SQExprBuilder.equal(fieldExpr, powerbi.data.SQExprBuilder.integer(value));
                return powerbi.data.createDataViewScopeIdentity(expr);
            });
        
            // Metadata, describes the data columns, and provides the visual with hints
            // so it can decide how to best represent the data
            var dataViewMetadata = {
                columns: [
                    {
                        displayName: "Cycle",
                        queryName: "Cycle",
                        isMeasure: true,
                        format: "00",
                        type: powerbi.ValueType.fromDescriptor({ numeric: true })
                    },
                    {
                        displayName: "Engine 1",
                        isMeasure: true,
                        queryName: "engine1",
                        type: powerbi.ValueType.fromDescriptor({ numeric: true }),
                        objects: { dataPoint: { fill: { solid: { color: "#C02FB0" } } } }
                    },
                    {
                        displayName: "Engine 2",
                        isMeasure: true,
                        queryName: "engine2",
                        type: powerbi.ValueType.fromDescriptor({ numeric: true }),
                        objects: { dataPoint: { fill: { solid: { color: "#00AAAD" } } } }
                    }
                ]
            };

            var columns = [
                {
                    source: dataViewMetadata.columns[1],
                    values: line1values
                },
                {
                    source: dataViewMetadata.columns[2],
                    values: line2values
                }
            ];

            var dataValues = powerbi.data.DataViewTransform.createValueColumns(columns);
            var tableDataValues = categories.map((countryName, idx) => [countryName, columns[0].values[idx], columns[1].values[idx]]);

            return {
                metadata: dataViewMetadata,
                categorical: {
                    categories: [{
                        source: dataViewMetadata.columns[0],
                        values: categories,
                        identity: categoryIdentities,
                    }],
                    values: dataValues
                },
                table: {
                    rows: tableDataValues,
                    columns: dataViewMetadata.columns
                }
            };
        }

        public init(element: HTMLElement, valueAccessor: () => any) {
            $(element).addClass("chart-indicator-progress");

            var sourceObservable = valueAccessor();

            var lineChart = new LineChart(element);

            var onChartDataReceived = chartData => {
                if (!chartData) {
                    return;
                }

                var dataView = this.createDataView(chartData.categories, chartData.line1values, chartData.line2values);

                lineChart.updateChartData(dataView);

                $(element).removeClass("chart-indicator-progress");
            }

            sourceObservable.subscribe(onChartDataReceived);

            $(window).resize(() => {
                lineChart.resizeViewport();
            });

            onChartDataReceived(sourceObservable());
        }
    }

    var sensorLineChartBinding = new PredictionLineChartBinding();

    ko.bindingHandlers["predictionLineChart"] = {
        init: sensorLineChartBinding.init
    };
}