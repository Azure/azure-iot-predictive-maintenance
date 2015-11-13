module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export interface IHttpClient {
        get<T>(url: string): JQueryPromise<T>;
        post<T>(url: string, data: any): JQueryPromise<T>;
        patch<T>(url: string, data: any): JQueryPromise<T>;
        put<T>(url: string, data: any): JQueryPromise<T>;
        delete<T>(url: string): JQueryPromise<T>;
        head<T>(url: string): JQueryPromise<T>;
    }
}