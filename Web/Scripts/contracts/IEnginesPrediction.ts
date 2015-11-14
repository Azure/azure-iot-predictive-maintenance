module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export interface IEnginesPrediction {
        engine1prediction: Array<IPrediction>;
        engine2prediction: Array<IPrediction>;
    }
}