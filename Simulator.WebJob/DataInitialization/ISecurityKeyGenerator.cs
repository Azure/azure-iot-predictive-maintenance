namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.DataInitialization
{
    using Common.Models;

    public interface ISecurityKeyGenerator
    {
        /// <summary>
        /// Creates a random security key pair
        /// </summary>
        /// <returns>Populated SecurityKeys object</returns>
        SecurityKeys CreateRandomKeys();
    }
}