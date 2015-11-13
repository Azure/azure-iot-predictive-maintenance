module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export interface IDataView {
        metadata: any;
        categorical: ICategorical;
    }
}