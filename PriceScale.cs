using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;


namespace ChartExplorer
{

    public class PriceScale
    {
        public double _dMax;
        public double _dMin;
        public double _dIncrement;
        public double _dTick;
        public double dSpan;
        public Price Price;
        public PriceScaleVisual PriceScaleVisual;



        public PriceScale(Price price)
        {
            _dMax = price._dMax;
            _dMin = price._dMin;
            this.Price = price;
            this.Price.PriceScale = this;

            _dIncrement = 0.05;
            _dTick = 0.01;
            dSpan = _dMax - _dMin;
            PriceScaleVisual = new PriceScaleVisual(this);
        }

    }
    public class PriceScaleVisual : DrawingVisual
    {
        public PriceScale PriceScale { get; set; }
        public DrawingVisual PriceScaleNavigator { get; set; }
        public double TopPrice { get; set; }
        public double DSpan { get; set; }
        public double BottomPrice { get; set; }
        public const int c_PriceScaleWOffset = 40;
        public const int c_PriceScaleHOffset = 60;

        public PriceScaleVisual(PriceScale priceScale) : base()
        {
            this.PriceScale = priceScale;
            PriceScaleNavigator = new DrawingVisual();
            UpdateExtremePrices();
        }

        public void UpdateExtremePrices()
        {
            TopPrice    = PriceScale._dMax;
            BottomPrice = PriceScale._dMin;
            DSpan       = TopPrice - BottomPrice;
        }
        public void UpdateSpan()
        {
            DSpan = TopPrice - BottomPrice;
        }
        public void DrawPriceTag(double Y, Canvas canvas)
        {
            const int RectBorder = 3;
            DrawingContext dc    = PriceScaleNavigator.RenderOpen();


            Point point       = new Point(canvas.ActualWidth - c_PriceScaleWOffset, Y);
            Typeface typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.DemiBold, FontStretches.Normal) ;
            Brush brush       = new SolidColorBrush(Colors.Black);
            Brush rectBrush   = new SolidColorBrush(Colors.LightCyan);
            Pen pen           = new Pen(rectBrush, 0);
            Rect rect         = new Rect();
            FormattedText formattedText = new FormattedText(PriceAtY(Y, canvas).ToString("F2"), 
                                                            System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, 
                                                            typeface, 12, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);


            point.Y = point.Y - formattedText.Height / 2;
            rect.Y      = point.Y - RectBorder / 2;
            rect.X      = canvas.ActualWidth - c_PriceScaleWOffset - RectBorder;
            rect.Height = formattedText.Height + RectBorder;
            rect.Width  = formattedText.Width + (2 * RectBorder);


            dc.DrawRoundedRectangle(rectBrush, pen, rect, 5, 5);

            dc.DrawText(formattedText, point);
            DrawPriceGuide(dc, (point.Y + rect.Height / 2), canvas);
            dc.Close();
        }
        public void DrawPriceGuide(DrawingContext dc, double Y, Canvas canvas)
        {

            Point point1  = new Point(0, Y);
            Point point2  = new Point(canvas.ActualWidth - c_PriceScaleWOffset, Y);
            Pen pen       = new Pen(Brushes.LightCyan, 1);
            pen.DashStyle = DashStyles.Dash;
            dc.DrawLine(pen, point1, point2);


        }
        public void DrawPriceScale(Canvas canvas)
        {
            Point point = new Point(canvas.ActualWidth - c_PriceScaleWOffset, 0);
            Point DrawP = new Point(canvas.ActualWidth - c_PriceScaleWOffset, 0);
            Typeface typeface = new Typeface("Arial");
            Brush brush = new SolidColorBrush(Colors.Black);
            FormattedText sample = new FormattedText("0", System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, 12, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);

            
            double dTotalSpace = canvas.ActualHeight - c_PriceScaleHOffset;
            double dTextHeight = sample.Height;
            double dSpacing    = sample.Height;
            int nTags = 10;
            double dUsedSpace = (nTags * dTextHeight) + ((nTags - 1) * dSpacing);
            double dAvaibleSpace = dTotalSpace - dUsedSpace;
 
            
            if (dAvaibleSpace > 0)
            {
                while (dAvaibleSpace >= 0)
                {
                    nTags++;
                    dUsedSpace    = nTags * dTextHeight + (nTags - 1) * dSpacing;
                    dAvaibleSpace = dTotalSpace - dUsedSpace;
                }
            }
            else
            {
                while ((dAvaibleSpace < 0) && (nTags > 1)){
                    nTags--;    
                    dUsedSpace    = nTags * dTextHeight + (nTags - 1) * dSpacing;
                    dAvaibleSpace = dTotalSpace - dUsedSpace;

                }
            }

            DrawingContext dc = RenderOpen();
            for (int i = 0; i < nTags; i++)
            {
                FormattedText formattedText = new FormattedText(PriceAtY(point.Y, canvas).ToString("F2"), System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, 12, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                
                
                DrawP.Y = point.Y - dTextHeight / 2;
                
                dc.DrawText(formattedText, DrawP);
                DrawGridLine(dc, point.Y, canvas.ActualWidth - c_PriceScaleWOffset, canvas);
                point.Y =  point.Y + dSpacing + dTextHeight;
            }
            dc.Close();


          
        }


        public void DrawGridLine(DrawingContext dc, double Y,double X, Canvas canvas)
        {
            Pen pen = new Pen(Brushes.LightCyan, 1);
            Point point1 = new Point(0, Y); 
            Point point2 = new Point(X, Y); 
            dc.PushOpacity(0.1);
            dc.DrawLine(pen, point1, point2);
            dc.Pop();
        }
        public double PriceAtY(double Y, Canvas canvas)
        {
            return ((canvas.ActualHeight - c_PriceScaleHOffset - Y) * DSpan / (canvas.ActualHeight - c_PriceScaleHOffset) + BottomPrice);
        }
        public double YAtPrice(double price, Canvas canvas)
        {
            return -(((price - BottomPrice) * (canvas.ActualHeight - c_PriceScaleHOffset)) / DSpan) + (canvas.ActualHeight - c_PriceScaleHOffset);

        }
    }
}
