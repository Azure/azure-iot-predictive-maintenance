module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export interface IBag<T> {
        [key: string]: T;
    }
}