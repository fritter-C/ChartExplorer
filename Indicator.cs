using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartExplorer
{
    public struct SingleSizedArray
    {
        public double dValue;
        public DateTime datetime;
        public bool isValid;

        public SingleSizedArray(double dValue, DateTime datetime, bool isValid) : this()
        {
            this.dValue = dValue;
            this.datetime = datetime;
            this.isValid = isValid;
        }
    }
    public class Indicator
    {
        public List<SingleSizedArray> Values;
        public Interval Interval;
        public Price Price;


        public Indicator(Price Price, Interval Interval = Interval.Daily)
        {
            Values = new List<SingleSizedArray>();  
            this.Interval = Interval;  
            this.Price = Price; 
        }

        public virtual void CalcValues()
        {

        }
    }
}
