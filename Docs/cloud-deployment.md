# Cloud deployment

## Cloud deployment IoT services
The build.cmd script in the repository builds the solution code and also deploys the required IoT services to your Azure subscription. Cloud deployment creates the following:
* 1 x IotHub - S2
* 2 x Storage - Standard GRS
* 1 x Servicebus namespace - basic
* 1 x Eventhub
* 1 x Stream Analytics jobs - standard
* 2 x App Service Plan - standard
* 1 x Azure Machine Learning Workspace

## Steps for cloud deployment
1. Use your Git client to pull the latest version of the solution from this repository. 
2. Open a **Developer Command Prompt for VS2013 as an Administrator**. 
3. Navigate to the repository root directory. 
4. Run `build.cmd cloud [debug | release] <deploymentname>` for an Azure cloud deployment. 

   For a national cloud deployment, run the same as above but include CloudName at the end (eg. `build.cmd cloud debug AzureGermanyCloud` or `build.cmd cloud release mydeployment AzureGermanyCloud`)

This command will:
* save account name, subscription, and deployment location into the <serviceName>.config.user file
* create an Active Directory application in your directory and assign you as role administrator
* provision and connect the Azure resources for the e2e solution.
* build and upload the current repo code as a zip package to the storage account
* deploy 1 instance of web app and 1 instances of web job
