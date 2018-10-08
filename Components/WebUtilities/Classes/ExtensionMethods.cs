using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using WebUtilities;

namespace System
{
    public static class CustomExtensionMethods
    {
        /// <summary>
        /// Formats the double value with the current session's language.
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <returns></returns>
        public static string ToFormattedString(this double value, int decimalPlaces = 0)
        {
            // Get the current session's language manager.
            LanguageManager languageManager = (LanguageManager)HttpContext.Current.Session["LanguageManager"];

            // Get the format for a decimal place.
            string decimalPlace = languageManager.GetText("NumberFormatDoubleDecimalPlace");

            // Get the double format.
            string format = languageManager.GetText("NumberFormatDouble");

            // Set the decimal places.
            format = format.Replace("###DECIMALS###", string.Join(decimalPlace, new string[decimalPlaces + 1]));

            // Get the double format from the language manager
            // and return the formatted text.
            return String.Format(
                format,
                value
            );
        }

        /// <summary>
        /// Formats the double value with the current session's language.
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <returns></returns>
        public static string ToFormattedString(this double value, LanguageManager languageManager, Language language, int decimalPlaces = 0)
        {
            // Get the format for a decimal place.
            string decimalPlace = languageManager.GetText(language, "NumberFormatDoubleDecimalPlace");

            // Get the double format.
            string format = languageManager.GetText(language, "NumberFormatDouble");

            // Set the decimal places.
            format = format.Replace("###DECIMALS###", string.Join(decimalPlace, new string[decimalPlaces + 1]));

            // Get the double format from the language manager
            // and return the formatted text.
            return String.Format(
                format,
                value
            );
        }

        /// <summary>
        /// Formats the date value with the current session's language.
        /// </summary>
        /// <param name="value">The date value.</param>
        /// <returns></returns>
        public static string ToFormattedString(this DateTime value)
        {
            // Get the current session's language manager.
            LanguageManager languageManager = (LanguageManager)HttpContext.Current.Session["LanguageManager"];

            // Get the date format from the language manager
            // and return the formatted text.
            return value.ToString(
                languageManager.GetText("DateFormat") + " " +
                languageManager.GetText("TimeFormat")
            );
        }

        /// <summary>
        /// Formats the date value with the current session's language.
        /// </summary>
        /// <param name="value">The date value.</param>
        /// <returns></returns>
        public static string ToTimeDifference(this DateTime value)
        {
            // Get the current session's language manager.
            LanguageManager languageManager = (LanguageManager)HttpContext.Current.Session["LanguageManager"];

            TimeSpan timeSpan = DateTime.Now - value;

            if (timeSpan.TotalMinutes < 1)
            {
                return languageManager.GetText("TimeDifferenceLessThanMinute");
            }
            else if ((int)timeSpan.TotalMinutes == 1)
            {
                return string.Format(
                    languageManager.GetText("TimeDifferenceMinute"),
                    (int)timeSpan.TotalMinutes
                );
            }
            else if (timeSpan.TotalHours < 1)
            {
                return string.Format(
                    languageManager.GetText("TimeDifferenceMinutes"),
                    (int)timeSpan.TotalMinutes
                );
            }
            else if (timeSpan.TotalDays < 1)
            {
                return string.Format(
                    languageManager.GetText("TimeDifferenceHours"),
                    (int)timeSpan.TotalHours
                );
            }
            else if (timeSpan.TotalDays < 8)
            {
                return string.Format(
                    languageManager.GetText("TimeDifferenceDays"),
                    (int)timeSpan.TotalDays
                );
            }
            else
            {
                return value.ToFormattedString();
            }
        }

        /// <summary>
        /// Returns the report control as html string.
        /// </summary>
        public static string ToHtml(this Web.UI.WebControls.WebControl webControl)
        {
            string result = "";

            StringBuilder sb;
            StringWriter stWriter;
            HtmlTextWriter htmlWriter;

            sb = new StringBuilder();
            stWriter = new StringWriter(sb);
            htmlWriter = new HtmlTextWriter(stWriter);

            //this.RenderControl(htmlWriter);
            webControl.RenderBeginTag(htmlWriter);

            foreach (Control control in webControl.Controls)
            {
                control.RenderControl(htmlWriter);
            }

            webControl.RenderEndTag(htmlWriter);

            result = sb.ToString();

            return result;
        }
    }
}