module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export interface ITelemetry {
        timestamp: string;
        deviceId: number;
        recordId: number;
        sensor1: number;
        sensor2: number;
        sensor3: number;
        sensor4: number;
    }
}