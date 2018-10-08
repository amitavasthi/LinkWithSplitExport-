using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ApplicationUtilities.Classes
{
    public class ColorCalculator
    {
        public HexColor AdjustBrightness(HexColor color, double factor)
        {
            double r = ((color.Red * factor) > 255) ? 255 : (color.Red * factor);
            double g = ((color.Green * factor) > 255) ? 255 : (color.Green * factor);
            double b = ((color.Blue * factor) > 255) ? 255 : (color.Blue * factor);

            HexColor result = new HexColor();

            result.Red = (int)r;
            result.Green = (int)g;
            result.Blue = (int)b;

            return result;
        }

        public HexColor AdjustBrightness(HexColor color, double factorRed, double factorGreen, double factorBlue)
        {
            double r = ((color.Red * factorRed) > 255) ? 255 : (color.Red * factorRed);
            double g = ((color.Green * factorGreen) > 255) ? 255 : (color.Green * factorGreen);
            double b = ((color.Blue * factorBlue) > 255) ? 255 : (color.Blue * factorBlue);

            HexColor result = new HexColor();

            result.Red = (int)r;
            result.Green = (int)g;
            result.Blue = (int)b;

            return result;
        }

        public static HexColor GenerateRandomColor()
        {
            HexColor result = new HexColor();

            Random ran = new Random(DateTime.Now.Second);

            result.Red = ran.Next(0, 255);
            result.Green = ran.Next(0, 255);
            result.Blue = ran.Next(0, 255);

            return result;
        }
    }

    public class HexColor
    {
        #region Properties

        public int Red { get; set; }

        public int Green { get; set; }

        public int Blue { get; set; }

        #endregion


        #region Constructor

        public HexColor()
        {
            this.Red = 0;
            this.Green = 0;
            this.Blue = 0;
        }

        public HexColor(string hexColor)
        {
            if (hexColor.Length != 6 && (hexColor.Length == 7 && hexColor[0] != '#'))
            {
                throw new Exception("An hex color must contain 6 digits.");
            }

            if (hexColor[0] != '#')
                hexColor = "#" + hexColor;

            this.Red = FromHex(hexColor.Substring(1, 2));
            this.Green = FromHex(hexColor.Substring(3, 2));
            this.Blue = FromHex(hexColor.Substring(5, 2));
        }

        #endregion


        #region Operators



        #endregion


        #region Methods

        public override string ToString()
        {
            return "#" + ToHex(this.Red) +
                ToHex(this.Green) + ToHex(this.Blue);
        }

        private string ToHex(int value)
        {
            string result = value.ToString("X");

            if (result.Length == 1)
                result = "0" + result;

            return result;
        }

        private int FromHex(string value)
        {
            int num = int.Parse(value, System.Globalization.NumberStyles.AllowHexSpecifier);

            return num;
        }

        public Color ToDrawingColor()
        {
            ColorConverter col = new ColorConverter();

            col.ConvertFromString(this.ToString());

            return (Color)col.
                ConvertFromString(this.ToString());
        }

        public string ToImage(string fileName)
        {
            Bitmap btm = new Bitmap(1, 14);

            Color color = ToDrawingColor();

            for (int i = 0; i < 14; i++)
            {
                btm.SetPixel(0, i, color);
            }

            btm.Save(Path.Combine(HttpContext.Current.Request.
                PhysicalApplicationPath, "Images", "CompanyTemplate", fileName + ".png"),
                System.Drawing.Imaging.ImageFormat.Png);

            return "/Images/CompanyTemplate/" + fileName + ".png";
        }

        #endregion


        #region Event Handlers



        #endregion
    }
}
