using System;

namespace Zeiss.PublicationManager.Data.DataSet.Model
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
