namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Web.Services
{
    using System.Threading.Tasks;

    public interface ISimulationService
    {
        Task<string> StartSimulation();

        Task<string> StopSimulation();

        Task<string> GetSimulationState();
    }
}