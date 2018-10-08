using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCore.Classes
{
    public class Data
    {
        #region Properties

        public double Factor { get; set; }

        public double Value { get; set; }

        public double UnweightedValue { get; set; }

        public double EffectiveValue { get; set; }

        public double Base { get; set; }

        public double EffectiveBase { get; set; }

        public Dictionary<Guid, double[]> Responses { get; set; }
        //public RespondentCollection Responses { get; set; }

        #endregion


        #region Constructor

        public Data()
        {
            this.Responses = new Dictionary<Guid, double[]>();
            //this.Responses = new RespondentCollection();
        }

        #endregion


        #region Operators

        /*public static implicit operator Dictionary<Guid, double[]>(Data d)
        {
            return d.Responses;
        }

        public static explicit operator Data(Dictionary<Guid, double[]> dict)
        {
            Data data = new Data();
            data.Responses = dict;

            return data;
        }*/

        #endregion


        #region Methods

        public double GetStdDev(double factor, double mean, bool weighted = true)
        {
            double variance = 0.0;

            // Run through all respondents of the result.
            foreach (Guid idRespondent in this.Responses.Keys)
            {
                if (weighted)
                    variance += Math.Pow(this.Responses[idRespondent][0] - mean, 2);
                else
                    variance += Math.Pow(1 - mean, 2);
            }

            if (weighted)
            {
                if (this.Base != 0)
                    variance /= this.Base - 1;

            }
            else
            {
                if (this.Responses.Count != 0)
                    variance /= this.Responses.Count;
            }

            return Math.Sqrt(variance);
        }

        public double GetMean(double factor, bool weighted = true)
        {
            double result = 0.0;

            // Run through all respondents of the result.
            foreach (Guid idRespondent in this.Responses.Keys)
            {
                if (weighted)
                    result += this.Responses[idRespondent][0] * factor;
                else
                    result += 1 * factor;
            }

            if (weighted)
                result /= this.Base;
            else
                result /= this.Responses.Count;

            return result;
        }

        #endregion
    }
    public class Data2 : Data
    {
        #region Properties
        public bool[] Filter { get; set; }

        #endregion


        #region Constructor

        public Data2(long respondentsCount)
        {
            this.Filter = new bool[respondentsCount + 1];
        }

        #endregion
    }

    public class NumericData : Data
    {
        #region Properties

        /// <summary>
        /// Gets or sets the maximum numeric value of the data.
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the minimum numeric value of the data.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// Gets or sets the weighted mean value of the data.
        /// </summary>
        public double MeanValue { get; set; }

        /// <summary>
        /// Gets or sets the unweighted mean value of the data.
        /// </summary>
        public double UMeanValue { get; set; }

        /// <summary>
        /// Gets or sets the weighted standard deviation of the data.
        /// </summary>
        public double StdDev { get; set; }

        /// <summary>
        /// Gets or sets the unweighted standard deviation of the data.
        /// </summary>
        public double UStdDev { get; set; }

        #endregion


        #region Constructor

        public NumericData()
            : base()
        {
            this.MinValue = double.MaxValue;
        }

        #endregion
    }

    public class RespondentCollection : System.Collections.IEnumerable
    {
        public int Count { get; set; }

        public Dictionary<Guid, double[]> Values { get; set; }

        public Guid[] Keys { get { return this.Values.Keys.ToArray(); } }

        public Dictionary<byte, RespondentItem> Items { get; set; }

        public RespondentCollection()
        {
            this.Values = new Dictionary<Guid, double[]>();
            this.Items = new Dictionary<byte, RespondentItem>();
        }

        public void Add(Guid idRespondent, double[] values)
        {
            byte[] data = idRespondent.ToByteArray();

            if (!this.Items.ContainsKey(data[0]))
                this.Items.Add(data[0], new RespondentItem());

            this.Items[data[0]].Add(data, 1);

            this.Count++;
            this.Values.Add(idRespondent, values);
        }

        public void Remove(Guid idRespondent)
        {
            byte[] data = idRespondent.ToByteArray();

            this.Items[data[0]].Remove(data, 1);

            this.Values.Remove(idRespondent);
        }

        public bool ContainsKey(Guid idRespondent)
        {
            byte[] data = idRespondent.ToByteArray();

            if (this.Items.ContainsKey(data[0]))
            {
                if (this.Items[data[0]].ContainsKey(data, 1))
                    return true;
            }

            return false;
        }


        public double[] this[Guid idRespondent]
        {
            get
            {
                return this.Values[idRespondent];
            }
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }
    }

    public class RespondentItem
    {
        public Dictionary<byte, RespondentItem> Items { get; set; }

        public RespondentItem()
        {
            this.Items = new Dictionary<byte, RespondentItem>();
        }

        public bool ContainsKey(byte[] data, int index)
        {
            if (this.Items.ContainsKey(data[index]))
            {
                if (index == (data.Length - 1))
                {
                    return true;
                }

                if (this.Items[data[index]].ContainsKey(data, index + 1))
                    return true;
            }

            return false;
        }

        public void Add(byte[] data, int index)
        {
            if (data.Length <= index)
                return;

            if (!this.Items.ContainsKey(data[index]))
                this.Items.Add(data[index], new RespondentItem());

            this.Items[data[index]].Add(data, index + 1);
        }

        public void Remove(byte[] data, int index)
        {
            if (!this.Items.ContainsKey(data[index]))
                return;

            if (this.Items[data[index]].Items.Count <= 1)
            {
                this.Items.Remove(data[index]);
                return;
            }

            this.Items[data[index]].Remove(data, index + 1);
        }
    }
}
