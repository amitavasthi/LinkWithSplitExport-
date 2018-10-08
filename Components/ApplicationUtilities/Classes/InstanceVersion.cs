using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationUtilities.Classes
{
    public class InstanceVersion
    {
        #region Properties

        public int Part1 { get; set; }

        public int Part2 { get; set; }

        public int Part3 { get; set; }

        public int Part4 { get; set; }

        #endregion


        #region Constructor

        public InstanceVersion(string str)
        {
            string[] parts = str.Split('.');

            this.Part1 = int.Parse(parts[0]);
            this.Part2 = int.Parse(parts[1]);
            this.Part3 = int.Parse(parts[2]);
            this.Part4 = int.Parse(parts[3]);
        }

        #endregion


        #region Methods

        public override string ToString()
        {
            return string.Join(".", new int[]
            {
                this.Part1,
                this.Part2,
                this.Part3,
                this.Part4
            });
        }

        public int ToInt()
        {
            return int.Parse(string.Join("", new int[]
            {
                this.Part1,
                this.Part2,
                this.Part3,
                this.Part4
            }));
        }

        #endregion


        #region Operators

        // overload operator -
        public static InstanceVersion operator -(InstanceVersion a, int b)
        {
            if (a.Part4 != 0)
            {
                a.Part4--;
                return a;
            }
            else if (a.Part3 != 0)
            {
                a.Part4 = 99;
                a.Part3--;
                return a;
            }
            else if (a.Part2 != 0)
            {
                a.Part4 = 99;
                a.Part3 = 99;
                a.Part2--;
                return a;
            }
            else if (a.Part1 != 0)
            {
                a.Part4 = 99;
                a.Part3 = 99;
                a.Part2 = 99;
                a.Part1--;
                return a;
            }

            return a;
        }

        // overload operator +
        public static InstanceVersion operator +(InstanceVersion a, int b)
        {
            if (a.Part4 != 99)
            {
                a.Part4++;
                return a;
            }
            else if (a.Part3 != 99)
            {
                a.Part4 = 0;
                a.Part3++;
                return a;
            }
            else if (a.Part2 != 99)
            {
                a.Part4 = 0;
                a.Part3 = 0;
                a.Part2++;
                return a;
            }
            else if (a.Part1 != 99)
            {
                a.Part4 = 0;
                a.Part3 = 0;
                a.Part2 = 0;
                a.Part1++;
                return a;
            }

            return a;
        }

        #endregion
    }
}