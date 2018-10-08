
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebUtilities.Classes;

namespace LinkOnline.Handlers
{
    /// <summary>
    /// Summary description for UserValidationService
    /// </summary>
    public class UserValidationService : WebUtilities.BaseHandler
    {
        #region Properties

        #endregion


        #region Constructor

        public UserValidationService()
        {
            base.Methods.Add("Validate", Validate);
            base.Methods.Add("ChangePassword", ChangePassword);
        }

        #endregion


        #region Methods
        private bool PasswordValid(string password)
        {
            string regex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[*@$-+?_&=!%{}/])[A-Za-z\d*@$-+?_&=!%{}/]{8,32}"; ;
            var match = Regex.Match(password, regex);
            return match.Success;
        }

        #endregion


        #region Web Methods

        private void Validate(HttpContext context)
        {
            UserValidation userValidation = new UserValidation(Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "UserValidation",
                Global.Core.ClientName + ".xml"
            ));

            bool result = true;

            if (!userValidation.Exists)
            {
                result = false;
            }
            else
            {
                Dictionary<string, List<object[]>> values = Global.Core.UserValidationFieldValues.ExecuteReaderDict<string>(
                    "SELECT Field, Value FROM [UserValidationFieldValues] WHERE IdUser={0}",
                    new object[] { Global.IdUser.Value }
                );

                foreach (string field in userValidation.Fields.Keys)
                {
                    if (!values.ContainsKey(field))
                        continue;

                    if (context.Request.Params[field] != (string)values[field][0][1])
                    {
                        result = false;
                        break;
                    }
                }
            }

            if (userValidation.ForcePasswordChange)
            {
                context.Response.Write("password");
            }
            else
            {
                Global.Core.Users.SetValue("Id=" + Global.IdUser.Value, "Validated", result);

                context.Response.Write(result.ToString().ToLower());
            }
        }

        private void ChangePassword(HttpContext context)
        {
            UserValidation userValidation = new UserValidation(Path.Combine(
                context.Request.PhysicalApplicationPath,
                "App_Data",
                "UserValidation",
                Global.Core.ClientName + ".xml"
            ));

            if (!PasswordValid(context.Request.Params["Password"]))
            {
                context.Response.Write("false");
            }

            Global.Core.Users.SetValue(
                "Id=" + Global.IdUser.Value,
                "Password",
                Global.Core.Users.GetMD5Hash(context.Request.Params["Password"])
            );

            Global.Core.Users.SetValue("Id=" + Global.IdUser.Value, "Validated", true);

            context.Response.Write("true");
        }

        #endregion
    }
}