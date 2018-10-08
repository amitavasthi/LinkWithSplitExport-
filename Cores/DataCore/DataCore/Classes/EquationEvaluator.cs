using Crosstables.Classes.ReportDefinitionClasses.Collections;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataCore.Classes
{
    public class EquationEvaluator
    {
        #region Properties

        /// <summary>
        /// Gets or sets the equation to evaluate.
        /// </summary>
        public Equation Equation { get; set; }

        #endregion


        #region Constructor

        public EquationEvaluator(Equation equation)
        {
            this.Equation = equation;
        }

        #endregion


        #region Methods

        public EquationAssembly Compile()
        {
            CSharpCodeProvider test = new CSharpCodeProvider();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            StringBuilder arguments = new StringBuilder();

            foreach (EquationPlaceHolder placeHolder in this.Equation.Values.Values)
            {

                if (placeHolder.Type == EquationPlaceHolderType.ValuesArray)
                    arguments.Append("Dictionary<Guid, double[]>");
                else
                    arguments.Append("double");

                arguments.Append(" ");
                arguments.Append("_" + placeHolder.Identity.ToString().Replace("-", ""));
                arguments.Append(",");
            }
            
            if (this.Equation.Values.Count > 0)
                arguments = arguments.Remove(arguments.Length - 1, 1);

            string code = EquationTemplate.Template.Replace("###EQUATION###", this.Equation.Render(true));
            code = code.Replace("###ARGUMENTS###", arguments.ToString());

            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            if (directory.StartsWith("file:\\"))
                directory = directory.Remove(0, 6);

            parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.ReferencedAssemblies.Add("System.Numerics.dll");
            parameters.ReferencedAssemblies.Add(Path.Combine(directory, "RDotNet.dll"));
            parameters.ReferencedAssemblies.Add(Path.Combine(directory, "RDotNet.NativeLibrary.dll"));
            parameters.ReferencedAssemblies.Add(Path.Combine(directory, "EquationInclude.dll"));

            CompilerResults result = test.CompileAssemblyFromSource(parameters, code);

            if (result.Errors.Count > 0)
            {
                throw new Exception(result.Errors[0].ErrorText);
            }

            EquationAssembly assembly = new EquationAssembly(result.CompiledAssembly);

            return assembly;
        }

        #endregion
    }

    public class EquationAssembly
    {
        #region Properties

        public Assembly Assembly { get; set; }

        public Type EvaluatorType { get; set; }

        public object Evaluator { get; set; }

        #endregion


        #region Constructor

        public EquationAssembly(Assembly assembly)
        {
            this.Assembly = assembly;
            this.EvaluatorType = this.Assembly.GetType("UserScript.RunScript");
            this.Evaluator = Activator.CreateInstance(this.EvaluatorType);
        }

        #endregion


        #region Methods

        public string Evaluate(object[] arguments)
        {
            return this.EvaluatorType.InvokeMember(
                "Eval",
                BindingFlags.InvokeMethod,
                null,
                this.Evaluator,
                arguments
            ).ToString();
        }

        #endregion
    }

    public static class EquationTemplate
    {
        #region Properties

        private static string template;

        public static string Template
        {
            get
            {
                if (template == null)
                    LoadTemplate();

                return template;
            }
            set
            {
                template = value;
            }
        }


        #endregion


        #region Methods

        private static void LoadTemplate()
        {
            string fileName = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "")),
                "Resources",
                "EquationEvaluatorSource.cs"
            );

            Template = File.ReadAllText(fileName);
        }

        #endregion
    }
}
