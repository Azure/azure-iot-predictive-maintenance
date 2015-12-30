namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Models
{
    public class TableStorageResponse<T>
    {
        public T Entity { get; set; }

        public TableStorageResponseStatus Status { get; set; }
    }

    public enum TableStorageResponseStatus
    {
        Successful,
        ConflictError,
        UnknownError,
        DuplicateInsert,
        NotFound
    }
}