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
                animation: { transitionImmediate: false }
            });

            return lineChartVisual;
        }

        public updateChartData(lineChartDataView) {
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
    }
}