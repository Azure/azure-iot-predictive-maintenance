module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export interface IEnginesTelemetry {
        engine1telemetry: Array<ITelemetry>;
        engine2telemetry: Array<ITelemetry>;
    }
}