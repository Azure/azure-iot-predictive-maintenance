namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.SampleDataGenerator
{
    using System;

    public class RandomGenerator : IRandomGenerator
    {
        static readonly Random Random = BuildRandomSource();

        public double GetRandomDouble()
        {
            lock (Random)
            {
                return Random.NextDouble();
            }
        }

        static Random BuildRandomSource()
        {
            return new Random(Guid.NewGuid().GetHashCode());
        }
    }
}