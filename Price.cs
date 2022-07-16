using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CsvHelper.Configuration;
using System.IO;
using System.Globalization;


namespace ChartExplorer
{
    public class Price
    {
        public List<Candle> candles;
        public double _dMax;
        public double _dMin;
        public DateTime _firstDate;
        public DateTime _lastDate;
        public Interval Interval { get; set; }
        public TimeScale TimeScale { get; set; }
        public PriceScale? PriceScale { get; set; }
        public PriceVisual? PriceVisual { get; set; }

        public Price()
        {
            candles = new List<Candle>();
            Interval = Interval.Daily;
            
            ReadCsv();
            UpdateMaxMin();
            this.PriceVisual = new PriceVisual(this);
        }
        public void GenerateTestSerie()
        {
            Candle candle1 = new Candle(DateTime.Parse("30/06/2022"), 27.65, 27.93, 28.06, 27.04, Interval);
            Candle candle2 = new Candle(DateTime.Parse("01/07/2022"), 28.08, 28.53, 28.85, 27.52, Interval);
            Candle candle3 = new Candle(DateTime.Parse("04/07/2022"), 28.54, 29.14, 29.28, 28.54, Interval);
            Candle candle4 = new Candle(DateTime.Parse("05/07/2022"), 28.9, 28.03, 28.95, 27.55, Interval);
            Candle candle5 = new Candle(DateTime.Parse("06/07/2022"), 28.16, 27.67, 28.48, 26.91, Interval);
            Candle candle6 = new Candle(DateTime.Parse("07/07/2022"), 28, 28.48, 28.92, 28, Interval);
            Candle candle7 = new Candle(DateTime.Parse("08/07/2022"), 28.53, 28.8, 28.97, 28.37, Interval);
            candles.Add(candle1);
            candles.Add(candle2);
            candles.Add(candle3);
            candles.Add(candle4);
            candles.Add(candle5);
            candles.Add(candle6);
            candles.Add(candle7);

        }
        public void ReadCsv()
        {
            string aux;
            string[] subs;
            Candle candle;

            try
            {
                using (var reader = new StreamReader(@"D:\Fernandos\fin\ChartExplorer\PETR4_D.csv"))
                {
                    reader.ReadLine(); // Retira o header;
                    while ((aux = reader.ReadLine()) != null)
                    {
                        subs = aux.Split(';');
                        candle = new Candle(DateTime.Parse(subs[0]), Double.Parse(subs[1]), Double.Parse(subs[4]), Double.Parse(subs[2]), Double.Parse(subs[3]));
                        candles.Add(candle);
                    }
                    candles.Sort((x, y) => DateTime.Compare(x.CandleDate, y.CandleDate));
              
                }
                int c = candles.Count;
            }
            catch
            {

            }


        }

        public void UpdateMaxMin()
        {
            _dMax = 0;
            _dMin = 100;
            foreach (var candle in candles)
            {
                if (candle.dMax > _dMax)
                {
                    _dMax = candle.dMax;

                }
                if (candle.dMin < _dMin)
                {
                    _dMin = candle.dMin;
                }
            }

        }
        public void UpdateDateTime()
        {
            _lastDate = candles.Last().CandleDate;
            _firstDate = candles.First().CandleDate;
        }
    }
    public class PriceVisual : DrawingVisual
    {
        public Price Price { get; set; }
        public readonly List<Candle> Candles;
        private bool highlight;
        private int nCandlesDraw;


        public PriceVisual(Price price)
        {
            this.Price = price;
            this.highlight = false;
            this.Candles = price.candles;
         
        }
        public void DrawCandles(Canvas canvas)
        {
            DrawingContext dc;
            dc = this.RenderOpen(); 

            if ((this.Price.TimeScale != null) && (this.Price.TimeScale != null))
            {
                foreach (var candle in Candles)
                {
                    if (this.Price.TimeScale.TimeScaleVisual.XAtDate(candle.CandleDate) < 0)
                    {
                        continue;
                    }
                    nCandlesDraw = Candles.Count - this.Price.TimeScale.TimeScaleVisual.Zoom;
                    DrawCandleShadow(dc, canvas,
                                     this.Price.PriceScale.PriceScaleVisual.YAtPrice(candle.dMax, canvas),
                                     this.Price.PriceScale.PriceScaleVisual.YAtPrice(candle.dMin, canvas),
                                     this.Price.TimeScale.TimeScaleVisual.XAtDate(candle.CandleDate));

                    DrawCandleBody(dc, canvas,
                                          this.Price.PriceScale.PriceScaleVisual.YAtPrice(candle.dOpen, canvas),
                                          this.Price.PriceScale.PriceScaleVisual.YAtPrice(candle.dClose, canvas),
                                          this.Price.TimeScale.TimeScaleVisual.XAtDate(candle.CandleDate));




                }
            }
            dc.Close();
        }
        public bool SetHightlight(bool value)
        {

            if (value != highlight)
            {
                highlight = value;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void DrawCandleBody(DrawingContext dc, Canvas canvas, double dOpenPos, double dClosePos, double X)
        {
            const int c_CandleSpacing = 6;
            Rect rect = new();
            Pen pen = new();
            Brush brush;
            double dAvaibleSpace = canvas.ActualWidth;
            pen.Brush = new SolidColorBrush(Colors.Black);

            rect.Width = 61;
            while(((this.nCandlesDraw * (rect.Width + c_CandleSpacing)) > dAvaibleSpace) && (rect.Width > 1))
            {
                rect.Width--;
            }
            rect.Height = Math.Abs(dOpenPos - dClosePos);
            rect.X = X - (rect.Width / 2);

            if (dOpenPos < dClosePos)
            {
                rect.Y = dOpenPos;
                brush = new SolidColorBrush(Colors.Red);
            }
            else if (dOpenPos > dClosePos)
            {
                rect.Y = dClosePos;
                brush = new SolidColorBrush(Colors.Green);
            }
            else
            {

                rect.Y = dOpenPos;
                brush = new SolidColorBrush(Colors.Black);
            }

            //Não desenha o corpo caso a largura seja menor que 3, apenas as sombras para evitar desenhos desnecessários
            if (!(rect.Width < 3))
            {
                dc.DrawRectangle(brush, pen, rect);
            }
        }

        public void DrawCandleShadow(DrawingContext dc, Canvas canvas, double dMaxPos, double dMinPos, double X)
        {
            Pen pen = new Pen();
            pen.Brush = new SolidColorBrush(Colors.Black);
            pen.Thickness = 1;
            Point point1 = new Point(X, dMaxPos);
            Point point2 = new Point(X, dMinPos);
            dc.DrawLine(pen, point1, point2);
        }
    }
}
