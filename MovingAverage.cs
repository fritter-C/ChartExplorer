using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;


namespace ChartExplorer
{
    public class MovingAverage : Indicator
    {
        public int Period;
        public MovingAverageVisual meanVisual;
        public MovingAverage(Price Price, int Period = 20) : base(Price)
        {
            this.Period = Period;
            CalcValues();
            meanVisual = new MovingAverageVisual(this);
        }

        public override void CalcValues()

        {
            List<Candle> candles;
            int index = 0;
            candles = Price.candles;
            SingleSizedArray ar;
            double sum = 0;

            if (candles.Count >= Period)
            {
                while (index < candles.Count)
                {
                    if (index < Period - 1)
                    {
                        ar = new(0, candles[index].CandleDate, false);
                        Values.Add(ar);
                    }
                    else
                    {
                        for (int i = 0; i < Period; i++)
                        {
                            sum = sum + candles[index - i].dClose;
                        }
                        sum = sum / Period;
                        ar = new(sum, candles[index].CandleDate, true);
                        sum = 0;
                        Values.Add(ar);
                    }
                    index++;
                }
            }
        }
    }
    public class MovingAverageVisual : DrawingVisual
    {

        public MovingAverage movingAverage;
        public MovingAverageVisual(MovingAverage movingAverage)
        {
            this.movingAverage = movingAverage;
        }

        public void Draw(Canvas canvas)
        {
            int c = movingAverage.Values.Count;
            SingleSizedArray ar;
            SingleSizedArray arNext;
            DrawingContext dc;
            dc = RenderOpen();
            for (int i = 0; i < (c - 1); i++)
            {
                ar = movingAverage.Values[i];
                arNext = movingAverage.Values[i + 1];

                if (ar.isValid)
                {
                    if (movingAverage.Price.TimeScale.TimeScaleVisual.XAtDate(arNext.datetime) < 0)
                    {
                        continue;
                    }
                    double X1 = movingAverage.Price.TimeScale.TimeScaleVisual.XAtDate(ar.datetime);
                    double Y1 = movingAverage.Price.PriceScale.PriceScaleVisual.YAtPrice(ar.dValue, canvas);
                    double X2 = movingAverage.Price.TimeScale.TimeScaleVisual.XAtDate(arNext.datetime);
                    double Y2 = movingAverage.Price.PriceScale.PriceScaleVisual.YAtPrice(arNext.dValue, canvas);

                    if ((X1 < 0) && (X2 > 0))
                    {
                        X1 = X2;
                    }
                    if (X2 > 0)
                    {
                        Point point1 = new(X1, Y1);
                        Point point2 = new(X2, Y2);
                        Pen pen = new Pen(Brushes.Blue, 2);

                        dc.DrawLine(pen, point1, point2);
                    }

                }
            }
            dc.Close();
        }


    }
}
