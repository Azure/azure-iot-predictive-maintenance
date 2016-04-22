# Tutorial: modify the Remaining Useful Life predictive experiment

This is a short tutorial in modifying the Remaining Useful Life experiment to exemplify how the preconfigured solution interacts with the ML experiment for RUL. 

1. Provision a Predictive maintenance pre-configured solution in your Azure subscription. You may do this either through a [[Cloud deployment]] or via https://www.azureiotsuite.com. 

2. Navigate to the ML workspace for your preconfigured solution. This can be found on https://www.azureiotsuite.com when you click on the tile for your preconfigured solution, in the details pane to the right, click on the ML Workspace link.

3. Select the Remaining Useful Life [Predictive Exp.] _Note: The current implementation creates two experiments with this name, they are duplicates - feel free to modify only one experiment and delete the other._ 

4. Under R Language Modules on the left hand menu, select Execute R Script. Drag this between the Metadata Editor and the Web service output. Move the connector from the output of the Metadata Editor to the left-most input of the Execute R script. Move the connector to the web service output from the left most output of the Execute R script. 

5. Modify the Execute R Script to the following: 

    ```
    \# Map 1-based optional input ports to variables
    dataset1 <- maml.mapInputPort(1) # class: data.frame
    
    \# Increase the resulting RUL by 1000
    dataset1$rul=dataset1$rul+1000
    
    \# Select data.frame to be sent to the output Dataset port
    maml.mapOutputPort("dataset1");
    ```

6. Modify the Web Service input from "input1" to "data". Modify the Web service output from "output1" to "data".

7. Click Run from the bottom menu. 

8. Once the run completes, click Deploy Web Service from the bottom menu. 

9. Open the RequestResponse link. Copy the RequestURI from the beginning until "/execute?api-version=2.0&details=true". _Note: This will look something like: https://ussouthcentral.services.azureml.net/workspaces/<guid>/services/<guid>_ This is your MLApiUrl. Copy the MLApiKey as well. 

10. In portal.azure.com, navigate to the Azure Resource group for your preconfigured solution. Go to the web job named <solutionname>-jobhost. Click All settings > Application settings. Paste the MLApiUrl and MLApiKey from the last step in the App settings. Save the application settings blade. 

Now, when you run the simulation in your predictive maintenance dashboard, you'll see the remaining useful life is 1000 higher than previous values. 