module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export interface IDevice {
        status: string;
        id: string;
        manufacturer: string;
        modelNumber: string;
        serialNumber: string;
        firmware: string;
        platform: string;
        processor: string;
        memory: string;
        updateTime: string;
        createTime: string;
        state: string;
        hostname: string;
        hubEnabledState: string;
    }
}