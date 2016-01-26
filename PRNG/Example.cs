using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using CryptoMath;

namespace PRNG
{
    static class Example
    {
        #region Background Information

        private static readonly Dictionary<int, int> blockLengths = new Dictionary<int, int>()
        {
            { 128,    8     },
            { 6272,   128   },
            { 750000, 10000 }
        };

        private static readonly Dictionary<int, List<int>> consideredMaxRunLengths = new Dictionary<int, List<int>>() //a.k.a vi (v0, v1, v2...) in NIST
        {
            { 8,     new List<int>() { 1, 2, 3, 4 }                 },
            { 128,   new List<int>() { 4, 5, 6, 7, 8, 9 }           },
            { 10000, new List<int>() { 10, 11, 12, 13, 14, 15, 16 } }
        };

        private static readonly Dictionary<int, Tuple<int, int>> coefficientKNs = new Dictionary<int, Tuple<int, int>>()
        {
            { 8,     new Tuple<int, int> (3, 16) },
            { 128,   new Tuple<int, int> (5, 49) },
            { 10000, new Tuple<int, int> (6, 75) }
        };

        private static readonly Dictionary<int, List<double>> coefficientPIs = new Dictionary<int, List<double>>()
        {
            { 8,     new List<double>() { 0.2148, 0.3672, 0.2305, 0.1875 }                         },
            { 128,   new List<double>() { 0.1174, 0.2430, 0.2493, 0.1752, 0.1027, 0.1124 }         },
            { 10000, new List<double>() { 0.0882, 0.2092, 0.2483, 0.1933, 0.1208, 0.0675, 0.0727 } }
        };

        #endregion

        private static List<string> results = new List<string>();


        public static void Run()
        {
            Console.WriteLine("Testing...");

            var prng = new PRNG();

            foreach (var seqLength in blockLengths.Keys)
            {
                var sequence = prng.GenerateSequence(seqLength);

                var m = blockLengths[seqLength];
                var mBlocks = Split(sequence, m);

                var maxRuns = FindMaxRuns(mBlocks);
                var maxRunsOccurrences = CountMaxRunsOccurrences(maxRuns, m);

                // or how to call?
                var randomnessCoefficient = ComputeRandomnessCoefficient(maxRunsOccurrences.Keys.ToList(), m);
                var pValue = ComputePValue(randomnessCoefficient, m);

                AnalyzePValue(pValue);
            }

            PrintResults();
        }

        private static BitArray[] Split(BitArray sequence, int blockLength)
        {
            var numberOfBlocks = sequence.Length / blockLength;
            var mBlocks = new BitArray[numberOfBlocks];

            // BitArray class is quite poor thus some manual work
            for (int blockNumber = 0; blockNumber < numberOfBlocks; blockNumber++)
            {
                mBlocks[blockNumber] = new BitArray(blockLength);
                var blockStartIndex = blockNumber * blockLength;

                for (int bitNumber = 0; bitNumber < blockLength; bitNumber++)
                {
                    mBlocks[blockNumber][bitNumber] = sequence[blockStartIndex + bitNumber];
                }
            }

            return mBlocks;
        }

        private static int[] FindMaxRuns(BitArray[] mBlocks)
        {
            var maxRuns = new int[mBlocks.Length];

            for (int i = 0; i < mBlocks.Length; i++)
            {
                maxRuns[i] = FindMaxRun(mBlocks[i]);
            }

            return maxRuns;
        }

        // A good task for some school IT olympiad
        private static int FindMaxRun(BitArray mBlock)
        {
            // Test here, todo: write unit-test
            // bitArray = new BitArray(new bool[] {false, true, true, true, false, true, true, true});

            var maxRun = 0;
            var counter = 0;

            for (int i = 0; i < mBlock.Length; i++)
            {
                if (mBlock[i] == true)
                {
                    counter = 0;
                    for (int j = i; j < mBlock.Length; j++)
                    {
                        if (mBlock[j] == true)
                        {
                            counter++;

                            // if the last bit is 1
                            if (j == mBlock.Length - 1)
                            {
                                // if maxRun is at the end
                                if (counter > maxRun)
                                {
                                    maxRun = counter;
                                }

                                break;
                            }
                        }

                        else
                        {
                            if (counter > maxRun)
                            {
                                maxRun = counter;
                            }

                            i = j;
                            break;
                        }
                    }
                }
            }

            return maxRun;
        }

        private static Dictionary<int, int> CountMaxRunsOccurrences(int[] maxRuns, int blockLength)
        {
            var maxRunsOccurrences = new Dictionary<int, int>();

            // initialize
            foreach (var length in consideredMaxRunLengths[blockLength])
            {
                maxRunsOccurrences.Add(length, 0);
            }

            foreach (var run in maxRuns)
            {
                var minSignificantRun = maxRunsOccurrences.Keys.Min();
                var maxSignificantRun = maxRunsOccurrences.Keys.Max();

                if (maxRunsOccurrences.ContainsKey(run))
                {
                    maxRunsOccurrences[run]++;
                }
                else // extreme cases
                {
                    if (run < minSignificantRun)
                    {
                        maxRunsOccurrences[minSignificantRun]++;
                    }
                    if (run > maxSignificantRun)
                    {
                        maxRunsOccurrences[maxSignificantRun]++;
                    }
                }
            }

            return maxRunsOccurrences;
        }

        private static double ComputeRandomnessCoefficient(List<int> occurrences, int blockNumber)
        {
            var N = coefficientKNs[blockNumber].Item2;
            var PIs = coefficientPIs[blockNumber];
            double coefficient = 0;

            for (int i = 0; i < occurrences.Count; i++)
            {
                double newSummand = (occurrences[i] - N * PIs[i]) * (occurrences[i] - N * PIs[i]) / (N * PIs[i]);
                coefficient += newSummand;
            }

            return coefficient;
        }

        private static double ComputePValue(double randomnessCoefficient, int blockNumber)
        {
            var K = coefficientKNs[blockNumber].Item1;

            double pValue = MoreFunctions.igamc(K / 2, randomnessCoefficient / 2);
            return pValue;
        }

        // perhaps redo it to write results one by one, not all at the same time
        private static void AnalyzePValue(double pValue)
        {
            if (pValue >= 0.01)
            {
                results.Add("Success");
            }
            else
            {
                results.Add("Fail");
            }
        }

        private static void PrintResults()
        {
            for (int i = 0; i < results.Count; i++)
            {
                Console.WriteLine("test {0}: {1}", i + 1, results[i]);
            }

            Console.ReadLine();
        }
    }
}
