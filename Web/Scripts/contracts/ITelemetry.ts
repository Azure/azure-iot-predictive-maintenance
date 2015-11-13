module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export interface ITelemetry {
        timestamp: string;
        sensor1: number;
        sensor2: number;
        sensor3: number;
        sensor4: number;
    }
}