/// <reference path="../typings/moment.d.ts" />

module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export class Dashboard {
        private warningTreshold: number;
        private httpClient: JQueryHttpClient;

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
        public engine1RulWarning: KnockoutObservable<boolean>;
        public engine2RulWarning: KnockoutObservable<boolean>;

        constructor() {
            //rebinding...
            this.startSimulation = this.startSimulation.bind(this);
            this.stopSimulation = this.stopSimulation.bind(this);
            this.getTelemetryData = this.getTelemetryData.bind(this);
            this.getPredictionData = this.getPredictionData.bind(this);
            this.onPredictionDataLoaded = this.onPredictionDataLoaded.bind(this);

            //initialization...
            this.warningTreshold = 200;
            this.httpClient = new JQueryHttpClient();
            this.sensor1Data = ko.observable<ILineChartData>();
            this.sensor2Data = ko.observable<ILineChartData>();
            this.sensor3Data = ko.observable<ILineChartData>();
            this.sensor4Data = ko.observable<ILineChartData>();
            this.rulData = ko.observable<ILineChartData>();
            this.engine1Rul = ko.observable<string>("N/A");
            this.engine1Cycles = ko.observable<string>("N/A");
            this.engine2Rul = ko.observable<string>("N/A");
            this.engine2Cycles = ko.observable<string>("N/A");
            this.engine1RulWarning = ko.observable<boolean>(false);
            this.engine2RulWarning = ko.observable<boolean>(false);
            this.simulationState = ko.observable<string>(SimulationStates.stopped);

            //startup...
            this.getTelemetryData();
            this.getPredictionData();
        }

        private getTelemetryData() {
            var getTelemetryPromise = this.httpClient.get("api/telemetry");

            getTelemetryPromise.done((enginesTelemetry: IEnginesTelemetry) => {
                var sensor1Data = { categories: [], line1values: [], line2values: [] };
                var sensor2Data = { categories: [], line1values: [], line2values: [] };
                var sensor3Data = { categories: [], line1values: [], line2values: [] };
                var sensor4Data = { categories: [], line1values: [], line2values: [] };

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

                    sensor1Data.line2values.push(reading.sensor1);
                    sensor2Data.line2values.push(reading.sensor2);
                    sensor3Data.line2values.push(reading.sensor3);
                    sensor4Data.line2values.push(reading.sensor4);
                });

                this.sensor1Data(sensor1Data);
                this.sensor2Data(sensor2Data);
                this.sensor3Data(sensor3Data);
                this.sensor4Data(sensor4Data);
            });
        }

        private getPredictionData(): void {
            var getPredictionPromise = this.httpClient.get<IEnginesPrediction>("api/prediction");

            getPredictionPromise.done(this.onPredictionDataLoaded);
            //getPredictionPromise.fail(...); TODO: implement handling
        }

        private onPredictionDataLoaded(prediction: IEnginesPrediction) {
            var initialData: ILineChartData = { categories: [], line1values: [], line2values: [] };
            var rulData = initialData;

            prediction.engine1prediction.forEach(reading => {
                var timestamp = moment(reading.timestamp).format("h:mm a");
                rulData.categories.push(timestamp);
                rulData.line1values.push(reading.rul);
            });

            prediction.engine2prediction.forEach(reading => {
                var timestamp = moment(reading.timestamp).format("h:mm a");
                rulData.categories.push(timestamp);
                rulData.line2values.push(reading.rul);
            });

            this.rulData(rulData);

            var engine1PredictionRul = _.last(prediction.engine1prediction).rul;

            this.engine1Rul(engine1PredictionRul.toString());
            this.engine1Cycles(_.last(prediction.engine1prediction).cycles.toString());

            if (engine1PredictionRul < this.warningTreshold)
                this.engine1RulWarning(true);

            var engine2PredictionRul = _.last(prediction.engine2prediction).rul;

            this.engine2Rul(_.last(prediction.engine2prediction).rul.toString());
            this.engine2Cycles(_.last(prediction.engine2prediction).cycles.toString());

            if (engine2PredictionRul < this.warningTreshold)
                this.engine2RulWarning(true);
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