/// <reference path="../typings/moment.d.ts" />

module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export class Dashboard {
        private httpClient: IHttpClient;

        public simulationState: KnockoutObservable<string>;
        public sensor1Data: KnockoutObservable<ILineChartData>;
        public sensor2Data: KnockoutObservable<ILineChartData>;
        public sensor3Data: KnockoutObservable<ILineChartData>;
        public sensor4Data: KnockoutObservable<ILineChartData>;
        public rulData: KnockoutObservable<ILineChartData>;
        public engine1Rul: KnockoutObservable<string>;
        public engine1Cycles: KnockoutObservable<string>;
        public engine2Rul: KnockoutObservable<string>;
        public engine2Cycles: KnockoutObservable<string>;

        constructor(httpClient: IHttpClient) {
            this.httpClient = httpClient;

            //rebinding...
            this.startSimulation = this.startSimulation.bind(this);
            this.stopSimulation = this.stopSimulation.bind(this);
            this.getTelemetryData = this.getTelemetryData.bind(this);
            this.getPredictionData = this.getPredictionData.bind(this);

            this.simulationState = ko.observable<string>(SimulationStates.stopped);

            this.sensor1Data = ko.observable<ILineChartData>();
            this.sensor2Data = ko.observable<ILineChartData>();
            this.sensor3Data = ko.observable<ILineChartData>();
            this.sensor4Data = ko.observable<ILineChartData>();
            this.rulData = ko.observable<ILineChartData>();
            this.engine1Rul = ko.observable<string>("N/A");
            this.engine1Cycles = ko.observable<string>("N/A");
            this.engine2Rul = ko.observable<string>("N/A");
            this.engine2Cycles = ko.observable<string>("N/A");

            this.getTelemetryData();
            this.getPredictionData();
        }

        private getTelemetryData() {
            var getTelemetryPromise = this.httpClient.get("api/telemetry");

            getTelemetryPromise.done((enginesTelemetry: IEnginesTelemetry) => {
                var initialData: ILineChartData = { categories: [], line1values: [], line2values: [] };
                var sensor1Data = initialData;
                var sensor2Data = initialData;
                var sensor3Data = initialData;
                var sensor4Data = initialData;

                enginesTelemetry.engine1telemetry.forEach(reading => {
                    var timestamp = moment(reading.timestamp).format("h:mm a");

                    sensor1Data.categories.push(timestamp);
                    sensor2Data.categories.push(timestamp);
                    sensor3Data.categories.push(timestamp);
                    sensor4Data.categories.push(timestamp);

                    sensor1Data.line1values.push(reading.sensor1);
                    sensor2Data.line1values.push(reading.sensor2);
                    sensor3Data.line1values.push(reading.sensor3);
                    sensor4Data.line1values.push(reading.sensor4);
                });

                enginesTelemetry.engine2telemetry.forEach(reading => {
                    var timestamp = moment(reading.timestamp).format("h:mm a");
                    
                    sensor1Data.categories.push(timestamp);
                    sensor2Data.categories.push(timestamp);
                    sensor3Data.categories.push(timestamp);
                    sensor4Data.categories.push(timestamp);

                    sensor1Data.line2values.push(reading.sensor1)
                    sensor2Data.line2values.push(reading.sensor2)
                    sensor3Data.line2values.push(reading.sensor3)
                    sensor4Data.line2values.push(reading.sensor4)
                });

                this.sensor1Data(sensor1Data);
                this.sensor2Data(sensor2Data);
                this.sensor3Data(sensor3Data);
                this.sensor4Data(sensor4Data);
            });
        }

        private getPredictionData() {
            var getPredictionPromise = this.httpClient.get("api/prediction");

            getPredictionPromise.done((prediction: IEnginesPrediction) => {
                var initialData: ILineChartData = { categories: [], line1values: [], line2values: [] };
                var rulData = initialData;

                prediction.engine1prediction.forEach(reading => {
                    var timestamp = new Date(Date.parse(reading.timestamp)).toTimeString();
                    rulData.categories.push(timestamp);
                    rulData.line1values.push(reading.rul);
                });

                prediction.engine2prediction.forEach(reading => {
                    var timestamp = new Date(Date.parse(reading.timestamp)).toTimeString();
                    rulData.categories.push(timestamp);
                    rulData.line2values.push(reading.rul);
                });

                this.rulData(rulData);

                this.engine1Rul(_.last(prediction.engine1prediction).rul.toString());
                this.engine1Cycles(_.last(prediction.engine1prediction).cycles.toString());
                this.engine2Rul(_.last(prediction.engine2prediction).rul.toString());
                this.engine2Cycles(_.last(prediction.engine2prediction).cycles.toString());
            });
        }

        public startSimulation() {
            var startEmulationPromise = this.httpClient.post("api/simulation/start", {});

            startEmulationPromise.done(() => {
                this.simulationState(SimulationStates.running);
            });
        }

        public stopSimulation() {
            var stopEmulationPromise = this.httpClient.post("api/simulation/stop", {});

            stopEmulationPromise.done(() => {
                this.simulationState(SimulationStates.stopped);
            });
        }
    }
}