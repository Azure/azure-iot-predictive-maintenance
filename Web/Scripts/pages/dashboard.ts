/// <reference path="../typings/moment.d.ts" />
/// <reference path="../constants.ts" />

module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export class Dashboard {
        private warningTreshold: number;
        private httpClient: JQueryHttpClient;
        private engine1RecordId: number;
        private engine2RecordId: number;
        private emptyString: string;

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
        public errorMessages: KnockoutObservableArray<string>;
        public sendingCommand: KnockoutObservable<boolean>;

        constructor() {
            //rebinding...
            this.startSimulation = this.startSimulation.bind(this);
            this.stopSimulation = this.stopSimulation.bind(this);
            this.getTelemetryData = this.getTelemetryData.bind(this);
            this.onTelemetryDataLoaded = this.onTelemetryDataLoaded.bind(this);
            this.getPredictionData = this.getPredictionData.bind(this);
            this.onPredictionDataLoaded = this.onPredictionDataLoaded.bind(this);
            this.onTelemetryLoadError = this.onTelemetryLoadError.bind(this);
            this.onPredictionLoadError = this.onPredictionLoadError.bind(this);
            this.onSendCommandError = this.onSendCommandError.bind(this);
            this.closeMessage = this.closeMessage.bind(this);
            this.getSimulationState = this.getSimulationState.bind(this);
            this.onSimulationStateReceived = this.onSimulationStateReceived.bind(this);

            //initialization...
            this.emptyString = "N/A";
            this.warningTreshold = 160;
            this.httpClient = new JQueryHttpClient();
            this.sensor1Data = ko.observable<ILineChartData>();
            this.sensor2Data = ko.observable<ILineChartData>();
            this.sensor3Data = ko.observable<ILineChartData>();
            this.sensor4Data = ko.observable<ILineChartData>();
            this.rulData = ko.observable<ILineChartData>();
            this.engine1Rul = ko.observable<string>(this.emptyString);
            this.engine1Cycles = ko.observable<string>(this.emptyString);
            this.engine2Rul = ko.observable<string>(this.emptyString);
            this.engine2Cycles = ko.observable<string>(this.emptyString);
            this.engine1RulWarning = ko.observable<boolean>(false);
            this.engine2RulWarning = ko.observable<boolean>(false);
            this.simulationState = ko.observable<string>(SimulationStates.stopped);
            this.errorMessages = ko.observableArray<string>();
            this.sendingCommand = ko.observable<boolean>(false);

            //startup...
            this.getTelemetryData();
            this.getPredictionData();

            setInterval(this.getSimulationState, 3000);
        }

        private getSimulationState() {
            var getSimulationStatePromise = this.httpClient.get<string>("api/simulation/state");

            getSimulationStatePromise.done(this.onSimulationStateReceived);
        }

        private onSimulationStateReceived(state: string) {
            this.simulationState(state);
        }

        private getTelemetryData() {
            var getTelemetryPromise = this.httpClient.get("api/telemetry");

            getTelemetryPromise.done(this.onTelemetryDataLoaded);
            getTelemetryPromise.fail(this.onTelemetryLoadError);
        }

        private onTelemetryDataLoaded(enginesTelemetry: IEnginesTelemetry) {
            var sensor1Data = { categories: [], line1values: [], line2values: [] };
            var sensor2Data = { categories: [], line1values: [], line2values: [] };
            var sensor3Data = { categories: [], line1values: [], line2values: [] };
            var sensor4Data = { categories: [], line1values: [], line2values: [] };

            enginesTelemetry.engine1telemetry.forEach(reading => {
                var timestamp = new Date(reading.timestamp);

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
                var timestamp = new Date(reading.timestamp);

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

            setTimeout(this.getTelemetryData, 1000);
        }

        private onTelemetryLoadError(): void {
            this.errorMessages.push(Constants.couldNotLoadTelemetry);
        }

        private getPredictionData(): void {
            var getPredictionPromise = this.httpClient.get<IEnginesPrediction>("api/prediction");

            getPredictionPromise.done(this.onPredictionDataLoaded);
            getPredictionPromise.fail(this.onPredictionLoadError);
        }

        private onPredictionDataLoaded(prediction: IEnginesPrediction) {
            var initialData: ILineChartData = { categories: [], line1values: [], line2values: [] };
            var rulData = initialData;

            prediction.engine1prediction.forEach(reading => {
                var cycles = reading.cycles;

                rulData.categories.push(cycles);
                rulData.line1values.push(reading.rul);
            });

            prediction.engine2prediction.forEach(reading => {
                var cycles = reading.cycles;

                rulData.categories.push(cycles);
                rulData.line2values.push(reading.rul);
            });

            this.rulData(rulData);

            if (prediction.engine1prediction.length > 0) {
                var engine1PredictionRul = _.last(prediction.engine1prediction).rul;

                this.engine1Rul(engine1PredictionRul.toString());
                this.engine1Cycles(_.last(prediction.engine1prediction).cycles.toString());

                this.engine1RulWarning(engine1PredictionRul < this.warningTreshold);
            } else {
                this.engine1Rul(this.emptyString);
                this.engine1Cycles(this.emptyString);
                this.engine1RulWarning(false);
            }

            if (prediction.engine2prediction.length > 0) {
                var engine2PredictionRul = _.last(prediction.engine2prediction).rul;

                this.engine2Rul(_.last(prediction.engine2prediction).rul.toString());
                this.engine2Cycles(_.last(prediction.engine2prediction).cycles.toString());

                this.engine2RulWarning(engine2PredictionRul < this.warningTreshold);
            } else {
                this.engine2Rul(this.emptyString);
                this.engine2Cycles(this.emptyString);
                this.engine2RulWarning(false);
            }

            setTimeout(this.getPredictionData, 1000);
        }

        private onPredictionLoadError(): void {
            this.errorMessages.push(Constants.couldNotLoadPredictions);
        }

        private onSendCommandError(): void {
            this.errorMessages.push(Constants.couldNotSendCommand);
            this.sendingCommand(false);
        }

        public closeMessage(message: string) {
            this.errorMessages.remove(message);
        }

        public startSimulation() {
            var startEmulationPromise = this.httpClient.post("api/simulation/start", {});

            this.sendingCommand(true);

            startEmulationPromise.done((state: string) => {
                this.sendingCommand(false);
                this.simulationState(state);
            });

            startEmulationPromise.fail(this.onSendCommandError);
        }

        public stopSimulation() {
            var stopEmulationPromise = this.httpClient.post("api/simulation/stop", {});

            this.sendingCommand(true);

            stopEmulationPromise.done((state: string) => {
                this.sendingCommand(false);
                this.simulationState(state);
            });

            stopEmulationPromise.fail(this.onSendCommandError);
        }
    }
}