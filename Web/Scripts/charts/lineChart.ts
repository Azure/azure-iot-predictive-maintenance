module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export class LineChart {
        private pluginService;
        private lineChartVisual;
        private humidityByFloorLineChartVisual;
        private singleVisualHostServices;
        private htmlElement: HTMLElement;

        constructor(htmlElement: HTMLElement) {
            this.htmlElement = htmlElement;
            this.createDefaultStyles = this.createDefaultStyles.bind(this);
            this.createLineChartVisual = this.createLineChartVisual.bind(this);
            this.createDataView = this.createDataView.bind(this);
            this.resizeViewport = this.resizeViewport.bind(this);

            this.pluginService = powerbi.visuals.visualPluginFactory.create();
            this.singleVisualHostServices = powerbi.visuals.singleVisualHostServices;
            this.lineChartVisual = this.createLineChartVisual();
        }

        private createDefaultStyles() {
            var dataColors = new powerbi.visuals.DataColorPalette();

            return {
                titleText: {
                    color: { value: "rgba(51,51,51,1)" }
                },
                subTitleText: {
                    color: { value: "rgba(145,145,145,1)" }
                },
                colorPalette: {
                    dataColors: dataColors
                },
                labelText: {
                    color: {
                        value: "rgba(51,51,51,1)"
                    },
                    fontSize: "12px"
                },
                isHighContrast: false
            };
        }

        private createLineChartVisual() {
            var $htmlElement = $(this.htmlElement);

            var width = $htmlElement.width();
            var height = $htmlElement.height();
            
            // Get a plugin
            var lineChartVisual = this.pluginService.getPlugin("lineChart").create();

            lineChartVisual.init({
                element: $htmlElement,
                host: this.singleVisualHostServices,
                style: this.createDefaultStyles(),
                viewport: { height: height, width: width },
                settings: { slicingEnabled: false },
                interactivity: { isInteractiveLegend: false, selection: false },
                animation: { transitionImmediate: true }
            });

            return lineChartVisual;
        }

        public updateChartData(categories: Array<any>, line1values: Array<any>, line2values: Array<any>) {
            var lineChartDataView = this.createDataView(categories, line1values, line2values);

            this.lineChartVisual.onDataChanged({
                dataViews: [lineChartDataView],
                viewport: this.lineChartVisual.viewport,
                duration: 0
            });
        }

        public resizeViewport(): void {
            this.lineChartVisual.currentViewport.width = this.htmlElement.clientWidth;
            this.lineChartVisual.render();
        }

        public createDataView(categories, line1values, line2values) {
            var fieldExpr = powerbi.data.SQExprBuilder.fieldExpr({ column: { schema: "s", entity: "table1", name: "engines" } });

            var categoryIdentities = categories.map(value => {
                var expr = powerbi.data.SQExprBuilder.equal(fieldExpr, powerbi.data.SQExprBuilder.text(value));
                return powerbi.data.createDataViewScopeIdentity(expr);
            });
        
            // Metadata, describes the data columns, and provides the visual with hints
            // so it can decide how to best represent the data
            var dataViewMetadata = {
                columns: [
                    {
                        displayName: "Sensor",
                        queryName: "Sensor",
                        type: powerbi.ValueType.fromDescriptor({ text: true })
                    },
                    {
                        displayName: "Engine 1",
                        isMeasure: true,
                        format: "$0,000.00",
                        queryName: "sales1",
                        type: powerbi.ValueType.fromDescriptor({ numeric: true }),
                        objects: { dataPoint: { fill: { solid: { color: "lightgreen" } } } }
                    },
                    {
                        displayName: "Engine 2",
                        isMeasure: true,
                        format: "$0,000.00",
                        queryName: "sales2",
                        type: powerbi.ValueType.fromDescriptor({ numeric: true }),
                        objects: { dataPoint: { fill: { solid: { color: "lightblue" } } } }
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
    }
}