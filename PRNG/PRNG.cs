using System;
using System.Collections;
using System.Text;

namespace PRNG
{
    class PRNG
    {
        private BitArray lfsr1;
        private BitArray lfsr2;
        private BitArray lfsr3;

        public PRNG()
        {
            var random = new Random();
            lfsr1 = new BitArray(new int[] { 42 });
            lfsr2 = new BitArray(new int[] { 666 });
            lfsr3 = new BitArray(new int[] { random.Next() });

            lfsr1.Length = 27;
            lfsr2.Length = 26;
            lfsr3.Length = 25;
        }

        public bool Next()
        {
            var x1 = this.Lfsr1Shift();
            var x2 = this.Lfsr2Shift();
            var x3 = this.Lfsr3Shift();

            var newBit = Combine(x1, x2, x3);
            return newBit;
        }

        public BitArray GenerateSequence(int length)
        {
            var sequence = new BitArray(length);

            for (int i = 0; i < length; i++)
            {
                sequence[i] = this.Next();
            }

            return sequence;
        }

        private bool Lfsr1Shift()
        {
            // (1, 2, 5, 27)
            var newBit = lfsr1[0] ^ lfsr1[1] ^ lfsr1[2] ^ lfsr1[5];
            this.Shift(lfsr1, newBit);
            return newBit;
        }

        private bool Lfsr2Shift()
        {
            // (1, 2, 6, 26)
            var newBit = lfsr2[0] ^ lfsr2[1] ^ lfsr2[2] ^ lfsr2[6];
            this.Shift(lfsr2, newBit);
            return newBit;
        }

        private bool Lfsr3Shift()
        {
            // (3, 25)
            var newBit = lfsr3[0] ^ lfsr3[3];
            this.Shift(lfsr3, newBit);
            return newBit;
        }

        /// <summary>
        /// Right-shift
        /// </summary>
        /// <param name="lfsr"></param>
        /// <param name="newBit"></param>
        private void Shift(BitArray lfsr, bool newBit)
        {
            for (int i = lfsr.Count - 2; i >= 0; i--)
            {
                lfsr[i + 1] = lfsr[i];
            }

            lfsr[0] = newBit;
        }

        private bool Combine(bool x1, bool x2, bool x3)
        {
            var newBit = (x1 & x3) ^ x2 ^ true;
            return newBit;
        }
    }


    public static class BitArrayExtensions
    {
        public static string ToBitString(this BitArray bitArray)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bitArray.Count; i++)
            {
                char c = bitArray[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
