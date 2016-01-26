using System;
using System.Collections;

namespace PRNG
{
    class Program
    {
        static void Main(string[] args)
        {
            #region uncomment this to generate a sequence with the PRNG
            //var seqLength = 128; // or whatever
            //var prng = new PRNG();
            //var key = prng.GenerateSequence(seqLength);

            //Console.WriteLine(key.ToBitString());
            //Console.ReadLine();
            #endregion

            PRNG_Test.Run();
        }
    }
}
