module Microsoft.Azure.Devices.Applications.PredictiveMaintenance {
    export class JQueryHttpClient {
        private urlFormat: string;
        private accessToken: string;

        constructor() {
            this.get = this.get.bind(this);
            this.post = this.post.bind(this);
            this.delete = this.delete.bind(this);
            this.onBeforeSend = this.onBeforeSend.bind(this);
            this.handleErrorResponse = this.handleErrorResponse.bind(this);
            this.handle401Response = this.handle401Response.bind(this);

            this.urlFormat = "/{0}";
        }

        private onBeforeSend(xhrObj: JQueryXHR) {
        }

        private handleErrorResponse(xmlHttpRequest: JQueryXHR) {
            switch (xmlHttpRequest.status) {
                case 401: this.handle401Response(xmlHttpRequest);
            }
        }

        private handle401Response(xmlHttpRequest: JQueryXHR) {
            window.location.reload();
        }

        public get<T>(url: string): JQueryPromise<T> {
            var promise = jQuery.ajax({
                url: this.urlFormat.format(url),
                method: "GET",
                beforeSend: this.onBeforeSend,
                error: this.handleErrorResponse,
                cache: false
            });

            return promise;
        }

        public post<T>(url: string, data: any): JQueryPromise<T> {
            var promise = jQuery.ajax({
                url: this.urlFormat.format(url),
                method: "POST",
                data: data,
                beforeSend: this.onBeforeSend,
                error: this.handleErrorResponse,
                cache: false
            });

            return promise;
        }

        public patch<T>(url: string, data: any): JQueryPromise<T> {
            var promise = jQuery.ajax({
                url: this.urlFormat.format(url),
                method: "PATCH",
                data: data,
                beforeSend: this.onBeforeSend,
                error: this.handleErrorResponse,
                cache: false
            });

            return promise;
        }

        public put<T>(url: string, data: any): JQueryPromise<T> {
            var promise = jQuery.ajax({
                url: this.urlFormat.format(url),
                method: "PUT",
                data: data,
                beforeSend: this.onBeforeSend,
                error: this.handleErrorResponse,
                cache: false
            });

            return promise;
        }

        public delete<T>(url: string): JQueryPromise<T> {
            var promise = jQuery.ajax({
                url: this.urlFormat.format(url),
                method: "DELETE",
                beforeSend: this.onBeforeSend,
                error: this.handleErrorResponse,
                cache: false
            });

            return promise;
        }

        public head<T>(url: string): JQueryPromise<T> {
            var promise = jQuery.ajax({
                url: this.urlFormat.format(url),
                method: "HEAD",
                beforeSend: this.onBeforeSend,
                error: this.handleErrorResponse,
                cache: false
            });

            return promise;
        }
    }
}
