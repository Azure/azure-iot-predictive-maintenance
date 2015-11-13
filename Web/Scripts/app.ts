module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export class App {
        public sensor1Data;
        public sensor2Data;
        public devices: Devices;
        public dashboard: Dashboard;

        constructor() {
            this.sensor1Data = [];
            this.sensor2Data = [];

            var httpClient = new JQueryHttpClient();

            this.devices = new Devices(httpClient);
            this.dashboard = new Dashboard(httpClient);
        }
    }
}

$(() => {
    var app = new Microsoft.Azure.Devices.Applications.PredictiveMaintenance.App();
    ko.applyBindings(app);
});
