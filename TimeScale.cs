using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

namespace ChartExplorer
{
    public class TimeScale
    {
        private Interval Interval { get; set; }
        private DateTime FirstDate { get; set; }
        private DateTime LastDate { get; set; }

        private Price Price { get; set; }
        private LinkedList<DateTime> dateTimes { get; set; }
        public TimeScaleVisual TimeScaleVisual { get; set; }

        public TimeScale(Price price)
        {
            this.Price = price;
            this.Price.TimeScale = this;
            this.Interval = price.Interval;
            this.FirstDate = price._firstDate;
            this.LastDate = price._lastDate;
            dateTimes = new LinkedList<DateTime>();
            PopulateTimeDates();
            this.TimeScaleVisual = new TimeScaleVisual(this);

        }
        public void PopulateTimeDates()

        {

            foreach (Candle cd in this.Price.candles)
            {
                dateTimes.AddLast(cd.CandleDate);
            };
        }
        public LinkedList<DateTime> GetDateTimes()
        {
            return dateTimes;
        }

    }
    public class TimeScaleVisual : DrawingVisual
    {
        private TimeScale TimeScale { get; set; }
        public List<DateTime> DrawTimes { get; set; }
        private double DSpancing { get; set; }
        private double DTextWidth { get; set; }

        public int Zoom { get; set; }
        public int ZoomTopOffset { get; set; }
        public int ZoomBottomOffset { get;  set; }

        public int StarterZoom;

        public TimeScaleVisual(TimeScale timeScale)
        {
            this.TimeScale = timeScale;
            this.DSpancing = 0;
            DrawTimes = new List<DateTime>();
            foreach (DateTime dt in timeScale.GetDateTimes())
            {
                DrawTimes.Add(dt);
            }
            DrawTimes.Sort();
            this.Zoom = DrawTimes.Count - 10;
            if (this.Zoom < 0)
            {
                this.Zoom = 0;
            }
            this.StarterZoom = this.Zoom;
            this.ZoomTopOffset = 0;
            this.ZoomBottomOffset = 0;
        }
        public void DrawTimeScale(DrawingContext dc, Canvas canvas)
        {
            DateTime dateTime;
            int nEntrys;
            double dAvaiableLenght;
            double dSpaceNeeded;

            if (DrawTimes != null)
            {
                nEntrys = DrawTimes.Count;
                Typeface typeface = new Typeface("Arial");
                Brush brush = new SolidColorBrush(Colors.Black);
                FormattedText sampleText = new FormattedText("01/12/99", System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, 12, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                this.DTextWidth = sampleText.Width;
                dAvaiableLenght = canvas.ActualWidth - 30 - 2 * this.DTextWidth;
                dSpaceNeeded = (nEntrys - Zoom) * this.DTextWidth;
                this.DSpancing = (dAvaiableLenght - dSpaceNeeded) / (DrawTimes.Count - 1 - Zoom);


                for (int i = (Zoom - ZoomTopOffset); i < (nEntrys - ZoomTopOffset) ; i++)
                {
                    dateTime = DrawTimes[i];

                    Point point = new Point(this.DTextWidth + (i -(Zoom - ZoomTopOffset)) * (this.DSpancing + this.DTextWidth), canvas.ActualHeight - 40);

                    FormattedText formattedText = new FormattedText(dateTime.ToString("dd/MM/yy"), System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, 12, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    dc.DrawText(formattedText, point);

                }
            }
        }

        public double XAtDate(DateTime dateTime)
        {


            for (int i = (Zoom - ZoomTopOffset); i < (DrawTimes.Count - ZoomTopOffset); i++)
            {
                if (DrawTimes[i] == dateTime)
                {
                    return this.DTextWidth + (i - (Zoom - ZoomTopOffset)) * (this.DSpancing + this.DTextWidth) + this.DTextWidth / 2;
                }
            }
            return -1;
        }
    }


}
