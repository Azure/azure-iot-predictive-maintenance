module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export interface ICategorical {
        categories: Array<ICategory>;
        values: any;
    }
}