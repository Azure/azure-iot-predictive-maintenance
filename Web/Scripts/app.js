/// <reference path="../typings/knockout.d.ts" />
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    ko.bindingHandlers["selectText"] = {
                        init: function (element, valueAccessor) {
                            var target = valueAccessor();
                            ko.applyBindingsToNode(element, {
                                click: function () { $(target).select(); }
                            });
                        }
                    };
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices.Applications || (Devices.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="../typings/knockout.d.ts" />
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    ko.bindingHandlers["status"] = {
                        init: function (element, valueAccessor) {
                            var status = valueAccessor();
                            var statusClass;
                            switch (status) {
                                case "Running":
                                    statusClass = "status_true";
                                    break;
                                case "Pending":
                                    statusClass = "status_pending";
                                    break;
                                case "Disabled":
                                    statusClass = "status_false";
                                    break;
                                default:
                                    throw new Error("Unknown device state.");
                            }
                            ko.applyBindingsToNode(element, { css: statusClass });
                        }
                    };
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices.Applications || (Devices.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="../typings/knockout.d.ts" />
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    ko.bindingHandlers["sortBy"] = {
                        init: function (element, valueAccessor) {
                            var options = valueAccessor();
                            var column = options.column;
                            var collection = options.collection;
                            $(element).addClass("sorting");
                            collection.sortBy.subscribe(function (columnName) {
                                if (column != columnName) {
                                    $(element).removeClass("sorting_asc");
                                    $(element).removeClass("sorting_desc");
                                }
                            });
                            $(element).click(function () {
                                collection.sortBy(column);
                                collection.sortAccending(!collection.sortAccending());
                                $(element).removeClass("sorting_asc");
                                $(element).removeClass("sorting_desc");
                                if (collection.sortAccending()) {
                                    $(element).addClass("sorting_asc").attr("aria-sort", "ascending");
                                }
                                else {
                                    $(element).addClass("sorting_desc").attr("aria-sort", "descending");
                                }
                            });
                        }
                    };
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices.Applications || (Devices.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    var SortedCollection = (function () {
                        function SortedCollection(collection) {
                            this.collection = collection;
                            this.observable = this.observable.bind(this);
                            this.sortBy = ko.observable();
                            this.sortAccending = ko.observable(true);
                        }
                        SortedCollection.prototype.observable = function () {
                            return _.sortByOrder(this.collection(), this.sortBy(), this.sortAccending());
                        };
                        return SortedCollection;
                    })();
                    PredictiveMaintenance.SortedCollection = SortedCollection;
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices.Applications || (Devices.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    var Dashboard = (function () {
                        function Dashboard(httpClient) {
                            this.httpClient = httpClient;
                            this.startSimulation = this.startSimulation.bind(this);
                            this.stopSimulation = this.stopSimulation.bind(this);
                            this.getTelemetryData = this.getTelemetryData.bind(this);
                            this.getPredictionData = this.getPredictionData.bind(this);
                            this.simulationState = ko.observable(PredictiveMaintenance.SimulationStates.stopped);
                            this.sensor1Data = ko.observable();
                            this.sensor2Data = ko.observable();
                            this.sensor3Data = ko.observable();
                            this.sensor4Data = ko.observable();
                            this.rulData = ko.observable();
                            this.engine1Rul = ko.observable("N/A");
                            this.engine1Cycles = ko.observable("N/A");
                            this.engine2Rul = ko.observable("N/A");
                            this.engine2Cycles = ko.observable("N/A");
                            this.getTelemetryData();
                            this.getPredictionData();
                        }
                        Dashboard.prototype.getTelemetryData = function () {
                            var _this = this;
                            var getTelemetryPromise = this.httpClient.get("api/telemetry");
                            getTelemetryPromise.done(function (enginesTelemetry) {
                                var initialData = { categories: [], line1values: [], line2values: [] };
                                var sensor1Data = initialData;
                                var sensor2Data = initialData;
                                var sensor3Data = initialData;
                                var sensor4Data = initialData;
                                enginesTelemetry.engine1telemetry.forEach(function (reading) {
                                    var timestamp = new Date(Date.parse(reading.timestamp)).toTimeString();
                                    sensor1Data.categories.push(timestamp);
                                    sensor2Data.categories.push(timestamp);
                                    sensor3Data.categories.push(timestamp);
                                    sensor4Data.categories.push(timestamp);
                                    sensor1Data.line1values.push(reading.sensor1);
                                    sensor2Data.line1values.push(reading.sensor2);
                                    sensor3Data.line1values.push(reading.sensor3);
                                    sensor4Data.line1values.push(reading.sensor4);
                                });
                                enginesTelemetry.engine2telemetry.forEach(function (reading) {
                                    var timestamp = new Date(Date.parse(reading.timestamp)).toTimeString();
                                    sensor1Data.categories.push(timestamp);
                                    sensor2Data.categories.push(timestamp);
                                    sensor3Data.categories.push(timestamp);
                                    sensor4Data.categories.push(timestamp);
                                    sensor1Data.line2values.push(reading.sensor1);
                                    sensor2Data.line2values.push(reading.sensor2);
                                    sensor3Data.line2values.push(reading.sensor3);
                                    sensor4Data.line2values.push(reading.sensor4);
                                });
                                _this.sensor1Data(sensor1Data);
                                _this.sensor2Data(sensor2Data);
                                _this.sensor3Data(sensor3Data);
                                _this.sensor4Data(sensor4Data);
                            });
                        };
                        Dashboard.prototype.getPredictionData = function () {
                            var _this = this;
                            var getPredictionPromise = this.httpClient.get("api/prediction");
                            getPredictionPromise.done(function (prediction) {
                                var initialData = { categories: [], line1values: [], line2values: [] };
                                var rulData = initialData;
                                prediction.engine1prediction.forEach(function (reading) {
                                    var timestamp = new Date(Date.parse(reading.timestamp)).toTimeString();
                                    rulData.categories.push(timestamp);
                                    rulData.line1values.push(reading.rul);
                                });
                                prediction.engine2prediction.forEach(function (reading) {
                                    var timestamp = new Date(Date.parse(reading.timestamp)).toTimeString();
                                    rulData.categories.push(timestamp);
                                    rulData.line2values.push(reading.rul);
                                });
                                _this.rulData(rulData);
                                _this.engine1Rul(_.last(prediction.engine1prediction).rul.toString());
                                _this.engine1Cycles(_.last(prediction.engine1prediction).cycles.toString());
                                _this.engine2Rul(_.last(prediction.engine2prediction).rul.toString());
                                _this.engine2Cycles(_.last(prediction.engine2prediction).cycles.toString());
                            });
                        };
                        Dashboard.prototype.startSimulation = function () {
                            var _this = this;
                            var startEmulationPromise = this.httpClient.post("api/simulation/start", {});
                            startEmulationPromise.done(function () {
                                _this.simulationState(PredictiveMaintenance.SimulationStates.running);
                            });
                        };
                        Dashboard.prototype.stopSimulation = function () {
                            var _this = this;
                            var stopEmulationPromise = this.httpClient.post("api/simulation/stop", {});
                            stopEmulationPromise.done(function () {
                                _this.simulationState(PredictiveMaintenance.SimulationStates.stopped);
                            });
                        };
                        return Dashboard;
                    })();
                    PredictiveMaintenance.Dashboard = Dashboard;
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices.Applications || (Devices.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    var SimulationStates = (function () {
                        function SimulationStates() {
                        }
                        SimulationStates.stopped = "stopped";
                        SimulationStates.running = "running";
                        SimulationStates.completed = "completed";
                        return SimulationStates;
                    })();
                    PredictiveMaintenance.SimulationStates = SimulationStates;
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices.Applications || (Devices.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
String.prototype.format = function () {
    var values = [];
    for (var _i = 0; _i < arguments.length; _i++) {
        values[_i - 0] = arguments[_i];
    }
    var formatted = this;
    for (var i = 0; i < values.length; i++) {
        var regexp = new RegExp("\\{" + i + "\\}", "gi");
        if (values[i])
            formatted = formatted.replace(regexp, values[i]);
        else
            formatted = formatted.replace(regexp, "");
    }
    return formatted;
};
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    var App = (function () {
                        function App() {
                            this.sensor1Data = [];
                            this.sensor2Data = [];
                            var httpClient = new PredictiveMaintenance.JQueryHttpClient();
                            this.devices = new PredictiveMaintenance.Devices(httpClient);
                            this.dashboard = new PredictiveMaintenance.Dashboard(httpClient);
                        }
                        App.prototype.onUnhandledError = function (errorMessage, url, lineNumber, columnNumber, errorObject) {
                            var errorContent;
                            if (errorObject && errorObject.message !== undefined) {
                                errorContent = errorObject.message;
                            }
                            else if (errorObject !== undefined) {
                                errorContent = errorObject;
                            }
                            else {
                                errorContent = errorMessage;
                            }
                        };
                        return App;
                    })();
                    PredictiveMaintenance.App = App;
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices.Applications || (Devices.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
$(function () {
    var app = new Microsoft.Azure.Devices.Applications.PredictiveMaintenance.App();
    ko.applyBindings(app);
});
/// <reference path="ihttpclient.ts" />
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    var JQueryHttpClient = (function () {
                        function JQueryHttpClient() {
                            this.get = this.get.bind(this);
                            this.post = this.post.bind(this);
                            this.delete = this.delete.bind(this);
                            this.onBeforeSend = this.onBeforeSend.bind(this);
                            this.handleErrorResponse = this.handleErrorResponse.bind(this);
                            this.handle401Response = this.handle401Response.bind(this);
                            this.urlFormat = "/{0}";
                        }
                        JQueryHttpClient.prototype.onBeforeSend = function (xhrObj) {
                        };
                        JQueryHttpClient.prototype.handleErrorResponse = function (xmlHttpRequest) {
                            switch (xmlHttpRequest.status) {
                                case 401: this.handle401Response(xmlHttpRequest);
                            }
                        };
                        JQueryHttpClient.prototype.handle401Response = function (xmlHttpRequest) {
                            window.location.reload();
                        };
                        JQueryHttpClient.prototype.get = function (url) {
                            var promise = jQuery.ajax({
                                url: this.urlFormat.format(url),
                                method: "GET",
                                beforeSend: this.onBeforeSend,
                                error: this.handleErrorResponse,
                                cache: false
                            });
                            return promise;
                        };
                        JQueryHttpClient.prototype.post = function (url, data) {
                            var promise = jQuery.ajax({
                                url: this.urlFormat.format(url),
                                method: "POST",
                                data: data,
                                beforeSend: this.onBeforeSend,
                                error: this.handleErrorResponse,
                                cache: false
                            });
                            return promise;
                        };
                        JQueryHttpClient.prototype.patch = function (url, data) {
                            var promise = jQuery.ajax({
                                url: this.urlFormat.format(url),
                                method: "PATCH",
                                data: data,
                                beforeSend: this.onBeforeSend,
                                error: this.handleErrorResponse,
                                cache: false
                            });
                            return promise;
                        };
                        JQueryHttpClient.prototype.put = function (url, data) {
                            var promise = jQuery.ajax({
                                url: this.urlFormat.format(url),
                                method: "PUT",
                                data: data,
                                beforeSend: this.onBeforeSend,
                                error: this.handleErrorResponse,
                                cache: false
                            });
                            return promise;
                        };
                        JQueryHttpClient.prototype.delete = function (url) {
                            var promise = jQuery.ajax({
                                url: this.urlFormat.format(url),
                                method: "DELETE",
                                beforeSend: this.onBeforeSend,
                                error: this.handleErrorResponse,
                                cache: false
                            });
                            return promise;
                        };
                        JQueryHttpClient.prototype.head = function (url) {
                            var promise = jQuery.ajax({
                                url: this.urlFormat.format(url),
                                method: "HEAD",
                                beforeSend: this.onBeforeSend,
                                error: this.handleErrorResponse,
                                cache: false
                            });
                            return promise;
                        };
                        return JQueryHttpClient;
                    })();
                    PredictiveMaintenance.JQueryHttpClient = JQueryHttpClient;
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices.Applications || (Devices.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    var LineChart = (function () {
                        function LineChart(htmlElement) {
                            this.htmlElement = htmlElement;
                            this.createDefaultStyles = this.createDefaultStyles.bind(this);
                            this.createLineChartVisual = this.createLineChartVisual.bind(this);
                            this.createDataView = this.createDataView.bind(this);
                            this.resizeViewport = this.resizeViewport.bind(this);
                            this.pluginService = powerbi.visuals.visualPluginFactory.create();
                            this.singleVisualHostServices = powerbi.visuals.singleVisualHostServices;
                            this.lineChartVisual = this.createLineChartVisual();
                        }
                        LineChart.prototype.createDefaultStyles = function () {
                            var dataColors = new powerbi.visuals.DataColorPalette();
                            return {
                                titleText: {
                                    color: { value: "rgba(51,51,51,1)" }
                                },
                                subTitleText: {
                                    color: { value: "rgba(145,145,145,1)" }
                                },
                                colorPalette: {
                                    dataColors: dataColors
                                },
                                labelText: {
                                    color: {
                                        value: "rgba(51,51,51,1)"
                                    },
                                    fontSize: "12px"
                                },
                                isHighContrast: false
                            };
                        };
                        LineChart.prototype.createLineChartVisual = function () {
                            var $htmlElement = $(this.htmlElement);
                            var width = $htmlElement.width();
                            var height = $htmlElement.height();
                            var lineChartVisual = this.pluginService.getPlugin("lineChart").create();
                            lineChartVisual.init({
                                element: $htmlElement,
                                host: this.singleVisualHostServices,
                                style: this.createDefaultStyles(),
                                viewport: { height: height, width: width },
                                settings: { slicingEnabled: false },
                                interactivity: { isInteractiveLegend: false, selection: false },
                                animation: { transitionImmediate: true }
                            });
                            return lineChartVisual;
                        };
                        LineChart.prototype.updateChartData = function (categories, line1values, line2values) {
                            var lineChartDataView = this.createDataView(categories, line1values, line2values);
                            this.lineChartVisual.onDataChanged({
                                dataViews: [lineChartDataView],
                                viewport: this.lineChartVisual.viewport,
                                duration: 0
                            });
                        };
                        LineChart.prototype.resizeViewport = function () {
                            this.lineChartVisual.currentViewport.width = this.htmlElement.clientWidth;
                            this.lineChartVisual.render();
                        };
                        LineChart.prototype.createDataView = function (categories, line1values, line2values) {
                            var fieldExpr = powerbi.data.SQExprBuilder.fieldExpr({ column: { schema: "s", entity: "table1", name: "country" } });
                            var categoryIdentities = categories.map(function (value) {
                                var expr = powerbi.data.SQExprBuilder.equal(fieldExpr, powerbi.data.SQExprBuilder.text(value));
                                return powerbi.data.createDataViewScopeIdentity(expr);
                            });
                            var dataViewMetadata = {
                                columns: [
                                    {
                                        displayName: "Sensor",
                                        queryName: "Sensor",
                                        type: powerbi.ValueType.fromDescriptor({ text: true })
                                    },
                                    {
                                        displayName: "Engine 1",
                                        isMeasure: true,
                                        format: "$0,000.00",
                                        queryName: "sales1",
                                        type: powerbi.ValueType.fromDescriptor({ numeric: true }),
                                        objects: { dataPoint: { fill: { solid: { color: "lightgreen" } } } }
                                    },
                                    {
                                        displayName: "Engine 2",
                                        isMeasure: true,
                                        format: "$0,000.00",
                                        queryName: "sales2",
                                        type: powerbi.ValueType.fromDescriptor({ numeric: true }),
                                        objects: { dataPoint: { fill: { solid: { color: "lightblue" } } } }
                                    }
                                ]
                            };
                            var columns = [
                                {
                                    source: dataViewMetadata.columns[1],
                                    values: line1values
                                },
                                {
                                    source: dataViewMetadata.columns[2],
                                    values: line2values
                                }
                            ];
                            var dataValues = powerbi.data.DataViewTransform.createValueColumns(columns);
                            var tableDataValues = categories.map(function (countryName, idx) {
                                return [countryName, columns[0].values[idx], columns[1].values[idx]];
                            });
                            return {
                                metadata: dataViewMetadata,
                                categorical: {
                                    categories: [{
                                            source: dataViewMetadata.columns[0],
                                            values: categories,
                                            identity: categoryIdentities,
                                        }],
                                    values: dataValues
                                },
                                table: {
                                    rows: tableDataValues,
                                    columns: dataViewMetadata.columns
                                }
                            };
                        };
                        return LineChart;
                    })();
                    PredictiveMaintenance.LineChart = LineChart;
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices.Applications || (Devices.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="../typings/knockout.d.ts" />
/// <reference path="../charts/linechart.ts" />
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    ko.bindingHandlers["lineChart"] = {
                        init: function (element, valueAccessor) {
                            var sourceObservable = valueAccessor();
                            var lineChart = new PredictiveMaintenance.LineChart(element);
                            var update = function (chartData) {
                                if (!chartData) {
                                    return;
                                }
                                lineChart.updateChartData(chartData.categories, chartData.line1values, chartData.line2values);
                            };
                            sourceObservable.subscribe(update);
                            $(window).resize(function () {
                                lineChart.resizeViewport();
                            });
                            update(sourceObservable());
                        }
                    };
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices.Applications || (Devices.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
var Microsoft;
(function (Microsoft) {
    var Azure;
    (function (Azure) {
        var Devices;
        (function (Devices_1) {
            var Applications;
            (function (Applications) {
                var PredictiveMaintenance;
                (function (PredictiveMaintenance) {
                    var Devices = (function () {
                        function Devices(httpClient) {
                            this.httpClient = httpClient;
                            this.refresh = this.refresh.bind(this);
                            this.onDivicesLoaded = this.onDivicesLoaded.bind(this);
                            this.selectDevice = this.selectDevice.bind(this);
                            this.toggleSelectedDeviceDetails = this.toggleSelectedDeviceDetails.bind(this);
                            this.devicesCollection = ko.observableArray();
                            this.devicesCollectionSorted = new PredictiveMaintenance.SortedCollection(this.devicesCollection);
                            this.selectedDevice = ko.observable(null);
                            this.selectedDeviceDetailsOpen = ko.observable(false);
                            this.hasDevices = ko.observable(false);
                            this.refresh();
                        }
                        Devices.prototype.refresh = function () {
                            var getDevicesPromise = this.httpClient.get("api/devices");
                            getDevicesPromise.done(this.onDivicesLoaded);
                        };
                        Devices.prototype.onDivicesLoaded = function (devices) {
                            if (devices.length == 0) {
                                return;
                            }
                            this.devicesCollection(devices);
                            this.selectedDevice(devices[0]);
                        };
                        Devices.prototype.selectDevice = function (device) {
                            this.selectedDevice(device);
                            this.selectedDeviceDetailsOpen(true);
                        };
                        Devices.prototype.toggleSelectedDeviceDetails = function () {
                            this.selectedDeviceDetailsOpen(!this.selectedDeviceDetailsOpen());
                        };
                        return Devices;
                    })();
                    PredictiveMaintenance.Devices = Devices;
                })(PredictiveMaintenance = Applications.PredictiveMaintenance || (Applications.PredictiveMaintenance = {}));
            })(Applications = Devices_1.Applications || (Devices_1.Applications = {}));
        })(Devices = Azure.Devices || (Azure.Devices = {}));
    })(Azure = Microsoft.Azure || (Microsoft.Azure = {}));
})(Microsoft || (Microsoft = {}));
//# sourceMappingURL=app.js.map