using System;

namespace BlazorElectron.Data.PublicationDataSet
{
    public class Randomizer
    {
        public static int GetRandomID()
        {
            Random Randomizer = new Random();
            return Randomizer.Next(10000);
        }
    }
}
