module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export interface IPrediction {
        timestamp: string;
        rul: number;
        cycles: number;
    }
}