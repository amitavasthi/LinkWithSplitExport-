namespace UserScript
{
    using System;
    using System.Collections.Generic;
    using EquationInclude;
    using RDotNet;

    public class RunScript
    {
        public object Eval(###ARGUMENTS###)
        {
            ###EQUATION###
        }

        private double SigDiff(double value1, double base1, double value2, double base2, int level = 90)
        {
            if (value1 == 0)
                return 0;

            if (value2 == 0)
                return 0;

            // Get the percentage of the first value.
            value1 = value1 * 100 / base1;

            // Get the percentage of the second value.
            value2 = value2 * 100 / base2;

            if (value1 == 0.0 || value2 == 0.0)
                return 0;

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

            double p = (zScoreLevel * (Math.Sqrt(((value1 * (100 - value1)) / base1) + (value2 * (100 - value2)) / base2))) / 100;

            if ((((value1 - value2) / 100) > p) || (((value2 - value1) / 100) > p))
            {
                // The difference is significant .
                return 1;
            }
            else
            {
                // The difference is not significant.
                return 0;
            }
        }

        private void R(string script)
        {
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance();

            GenericVector testResult = engine.Evaluate(script).AsList();
        }
        private double[] R<T>(string script)
        {
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance();

            GenericVector testResult = engine.Evaluate(script).AsList();

            double[] result = testResult["p.value"].AsNumeric().ToArray();

            return result;
        }
    }
}