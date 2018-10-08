using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCore.Classes
{
    public class MeanSignificanceTest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the first sample for the significance test.
        /// </summary>
        public double[,] Sample1 { get; set; }

        /// <summary>
        /// Gets or sets the second sample for the significance test.
        /// </summary>
        public double[,] Sample2 { get; set; }

        #endregion


        #region Constructor

        public MeanSignificanceTest()
        {

        }

        public MeanSignificanceTest(double[,] sample1, double[,] sample2)
        {
            this.Sample1 = sample1;
            this.Sample2 = sample2;
        }

        public MeanSignificanceTest(double[] values1, double[] values2, double factor1, double factor2)
        {
            this.Sample1 = new double[values1.Length, 2];
            this.Sample2 = new double[values2.Length, 2];

            for (int i = 0; i < values1.Length; i++)
            {
                this.Sample1[i, 0] = factor1;
                this.Sample1[i, 1] = values1[0];
            }

            for (int i = 0; i < values2.Length; i++)
            {
                this.Sample2[i, 0] = factor2;
                this.Sample2[i, 1] = values2[0];
            }
        }

        #endregion


        #region Methods



        public bool IsMeanSignificant(int level)
        {
            double zScore = getZScore(
                GetMean(this.Sample1),
                getBase(this.Sample1),
                GetStandardDeviation(this.Sample1),
                GetMean(this.Sample2),
                getBase(this.Sample2),
                GetStandardDeviation(this.Sample2)
            );

            // 95% = 1.96
            // 90% = 1.645
            double zScoreLevel = 0.0;

            switch (level)
            {
                case 95:
                    zScoreLevel = 1.96;
                    break;

                case 90:
                    zScoreLevel = 1.645;
                    break;
            }

            if (Math.Abs(zScore) > zScoreLevel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private double getZScore(double meanScore1, double SampleSize1, double StdDev1, double meanScore2, double SampleSize2, double StdDev2)
        {
            double zScore = 0;
            if (SampleSize1 > 0 && SampleSize2 > 0)
            {
                zScore = (meanScore1 - meanScore2) / Math.Sqrt((StdDev1 * StdDev1 / SampleSize1) + (StdDev2 * StdDev2 / SampleSize2));
            }
            return zScore;

        }

        private double getBase(double[,] values, bool weight = true)
        {
            if (!weight)
                return values.Length / 2;

            double baseValue = 0;

            for (long i = 0; i < values.Length / 2; i++)
            {
                baseValue += values[i, 1];
            }

            return baseValue;
        }


        public double GetMean(double[,] values, bool weight = true)
        {
            double meanValue = 0;
            double sumVal = 0;
            double baseValue = 0;

            for (long i = 0; i < values.Length / 2; i++)
            {
                if (weight)
                    sumVal += values[i, 0] * values[i, 1];
                else
                    sumVal += values[i, 0];

                if (weight)
                    baseValue += values[i, 1];
                else
                {
                    if (values[i, 1] != 0)
                        baseValue += 1;
                }
            }
            if (baseValue != 0)
            {
                meanValue = sumVal / baseValue;
            }

            return meanValue;
        }

        public double GetStandardDeviation(double[,] values, bool weight = true)
        {
            //First number is answer
            //Second number is Respondent Weight

            double baseValue = 0;
            double meanValue = 0;
            double variance = 0;
            double stdDev = 0;

            meanValue = GetMean(values, weight);
            baseValue = getBase(values, weight);

            //Now calculate the variance
            for (long i = 0; i < values.Length / 2; i++)
            {
                variance += Math.Pow(values[i, 0] - meanValue, 2);
            }

            if (baseValue != 0)
            {
                variance = variance / (baseValue - 1);
            }

            //Now calculate the standard deviation
            stdDev = Math.Sqrt(variance);

            return stdDev;
        }

        #endregion
    }
}
