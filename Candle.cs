using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

namespace ChartExplorer
{
    public enum Interval
    {
        Daily,
    }
    public enum CandleType
    {
        ctBull,
        ctHarami,
        ctBear,
    }
    public class Candle
    {
        public DateTime CandleDate;
        public double dOpen;
        public double dClose;
        public double dMax;
        public double dMin;
        public CandleType CandleType;
        public Interval Interval;


        public Candle(DateTime candleDate, double dOpen, double dClose, double dMax, double dMin, Interval interval = Interval.Daily)
        {
            this.Interval = interval;
            this.CandleDate = candleDate;
            this.dOpen = dOpen;
            this.dClose = dClose;
            this.dMax = dMax;
            this.dMin = dMin;
            SetCandleType();
            Interval = interval;    
        }
        public void SetCandleType()
        {
            if (this.dOpen > this.dClose)
            {
                this.CandleType = CandleType.ctBear;
            }
            else if (this.dClose > this.dOpen)
            {
                this.CandleType = CandleType.ctBull;
            }
            else
            {
               this.CandleType = CandleType.ctHarami;
            }
        }
    }
    
    
}
