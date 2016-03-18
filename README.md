#Microsoft Azure IoT Suite 
You can deploy preconfigured solutions that implement common Internet of Things (IoT) scenarios to Microsoft Azure using your Azure subscrption. You can use preconfigured solutions: 
- as a starting point for your own IoT solution. 
- to learn about the most common patterns in IoT solution design and development. 

Each preconfigured solution implements a common IoT scenario and is a complete, end-to-end implementation. You can deploy the Azure IoT Suite predictive maintenance preconfigured solution from [https://www.azureiotsuite.com](https://www.azureiotsuite.com), following the guidance on provisioning pre-configured solutions outlined in this [document](https://azure.microsoft.com/en-us/documentation/articles/iot-suite-getstarted-preconfigured-solutions/). In addition, you can download the complete source code from this repository to customize and extend the solution to meet your specific requirements. 

##Predictive Maintenance pre-configured solution
The predictive maintenance pre-configured solution illustrates how you can predict the point when failure is likely to occur. The solution combines key Azure IoT Suite services, including an ML workspace complete with experiments for predicting the Remaining Useful Life (RUL) of an aircraft engine, based on a public sample data set. 

##Contents of this repository

### Docs folder:

  * [Set up development environment (Windows)](Docs/dev-setup.md) outlines the prerequisites for deploying the remote monitoring preconfigured solution.
  * [Cloud deployment](Docs/cloud-deployment.md) describes building and deploying the remote monitoring preconfigured solution fully on Azure.
  * [Tutorial: modify the Remaining Useful Life predictive experiment](Docs/tutorial-rul.md) is a short walk through on modifying the Azure ML model slightly and making appropriate modifications to accommodate this.
  
Other useful [IoT Suite documentation](https://azure.microsoft.com/documentation/suites/iot-suite/):
  * [Frequently asked questions for IoT Suite](https://azure.microsoft.com/documentation/articles/iot-suite-faq/)
  * [Permissions on the azureiotsuite.com site](https://azure.microsoft.com/documentation/articles/iot-suite-permissions/)
  
### Web folder:
  * ASP.NET MVC 5 website containing user dashboard and device portal to manage IoT devices (add, remove, view, etc)

### Infrastructure folder:
  * APIs and application logic to support telemetry and device operations
 
### Simulator folder:
  * Simulator (Azure Web Job) that simulates one or more devices that send data to the IoT Hub for testing and troubleshooting

### EventProcessor folder:
  * Azure Worker Role that hosts an Event Hub **EventProcessorHost** instance to handle the event data from the devices forwarding event data to other back-end services or to the remote monitoring site

### Visual Studio solution:
  * **PredictiveMaintenance:** contains both the Dashboard web app, the EventProcessor worker role, and the Simulator worker role.

## Feedback 

Have ideas for how we can improve Azure IoT? Give us [Feedback](http://feedback.azure.com/forums/321918-azure-iot).
