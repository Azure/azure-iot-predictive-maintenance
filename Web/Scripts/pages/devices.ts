module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export class Devices {
        private httpClient: IHttpClient;

        public devicesCollection: KnockoutObservableArray<IDevice>;
        public sortedDevices: any;
        public selectedDevice: KnockoutObservable<IDevice>;
        public selectedDeviceDetailsOpen: KnockoutObservable<boolean>;
        public hasDevices: KnockoutObservable<boolean>;
        public devicesCollectionSorted: SortedCollection<IDevice>;

        constructor(httpClient: IHttpClient) {
            this.httpClient = httpClient;

            this.refresh = this.refresh.bind(this);
            this.onDivicesLoaded = this.onDivicesLoaded.bind(this);
            this.selectDevice = this.selectDevice.bind(this);
            this.toggleSelectedDeviceDetails = this.toggleSelectedDeviceDetails.bind(this);

            this.devicesCollection = ko.observableArray<IDevice>();
            this.devicesCollectionSorted = new SortedCollection(this.devicesCollection);
            this.selectedDevice = ko.observable<IDevice>(null);
            this.selectedDeviceDetailsOpen = ko.observable<boolean>(false);
            this.hasDevices = ko.observable<boolean>(false);
            this.refresh();
        }

        private refresh() {
            var getDevicesPromise = this.httpClient.get("api/devices");

            getDevicesPromise.done(this.onDivicesLoaded);
        }

        private onDivicesLoaded(devices: Array<IDevice>) {
            if (devices.length == 0) {
                return;
            }

            this.devicesCollection(devices);
            this.selectedDevice(devices[0]);
        }

        public selectDevice(device: IDevice) {
            this.selectedDevice(device);
            this.selectedDeviceDetailsOpen(true);
        }

        public toggleSelectedDeviceDetails() {
            this.selectedDeviceDetailsOpen(!this.selectedDeviceDetailsOpen());
        }
    }
}