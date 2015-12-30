namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.DataInitialization
{
    /// <summary>
    /// Represents component to create initial data for the system
    /// </summary>
    public interface IDataInitializer
    {
        void CreateInitialDataIfNeeded();
    }
}