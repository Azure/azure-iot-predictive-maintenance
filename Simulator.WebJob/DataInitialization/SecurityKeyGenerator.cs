namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Simulator.WebJob.DataInitialization
{
    using System;
    using System.Security.Cryptography;
    using Common.Models;

    /// <summary>
    /// Service to generate a security key pair for a device
    /// </summary>
    public class SecurityKeyGenerator : ISecurityKeyGenerator
    {
        // string will be about 33% longer than this
        const int _lengthInBytes = 16;

        /// <summary>
        /// Creates a random security key pair
        /// </summary>
        /// <returns>Populated SecurityKeys object</returns>
        public SecurityKeys CreateRandomKeys()
        {
            byte[] primaryRawRandomBytes = new byte[_lengthInBytes];
            byte[] secondaryRawRandomBytes = new byte[_lengthInBytes];

            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(primaryRawRandomBytes);
                rngCsp.GetBytes(secondaryRawRandomBytes);
            }

            string s1 = Convert.ToBase64String(primaryRawRandomBytes);
            string s2 = Convert.ToBase64String(secondaryRawRandomBytes);

            return new SecurityKeys(s1, s2);
        }
    }
}