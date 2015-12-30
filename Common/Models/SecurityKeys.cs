namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Models
{
    using System.ComponentModel.DataAnnotations;

    public class SecurityKeys
    {
        public SecurityKeys(string primaryKey, string secondaryKey)
        {
            PrimaryKey = primaryKey;
            SecondaryKey = secondaryKey;
        }

        public string PrimaryKey { get; set; }

        public string SecondaryKey { get; set; }
    }

    public enum SecurityKey
    {
        None = 0,

        [Display(Name = "primary")]
        Primary,

        [Display(Name = "secondary")]
        Secondary
    }
}