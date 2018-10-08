using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationInclude
{
    public class Include
    {
        #region R

        public static void R(string script)
        {
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance();

            if (!engine.IsRunning)
                engine.Initialize();

            GenericVector testResult = engine.Evaluate(script).AsList();

            //engine.Close();
            //engine.Dispose();
        }
        public static double[] R<T>(string script, string key)
        {
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance();

            GenericVector testResult = engine.Evaluate(script).AsList();

            string[] names = testResult.Names;
            
            double[] result = testResult[key].AsNumeric().ToArray();

            //engine.Close();
            //engine.Dispose();

            return result;
        }

        #endregion
    }
}
