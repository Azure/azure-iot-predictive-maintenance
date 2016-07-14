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
  * [Configuring Azure IoT Suite preconfigured solutions for demo purposes](https://github.com/Azure/azure-iot-remote-monitoring/blob/master/Docs/configure-preconfigured-demo.md) walks you through changing the footprint of the underlying Azure services for your solution.
  
### EventProcessor folder:
  * Azure Worker Role that hosts an Event Hub **EventProcessorHost** instance to handle the event data from the devices forwarding event data to other back-end services or to the remote monitoring site

### Visual Studio solution:
  * **PredictiveMaintenance:** contains the source code for the whole preconfigured solution, including the solution portal web app, the EventProcessor web job, and the Simulator web job. 

## Feedback 

Have ideas for how we can improve Azure IoT? Give us [Feedback](http://feedback.azure.com/forums/321918-azure-iot).

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.