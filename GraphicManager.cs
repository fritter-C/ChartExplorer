using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace ChartExplorer
{

    internal class GraphicManager
    {

        private GoTCanvas Canvas { get; set; }
        private PriceScale _priceScale { get; set; }
        private TimeScale TimeScale { get; set; }
        private MovingAverage movingAverage { get; set; }
        private Price Price { get; set; }
        private object Owner { get; set; }

        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool bShow);

        public GraphicManager(GoTCanvas canvas, object owner)

        {
            this.Owner = owner;
            this.Canvas = canvas;
            this.Canvas.MouseMove   += _canvas_MouseMove;
            this.Canvas.SizeChanged += _canvas_SizeChanged;
            this.Canvas.MouseLeave  += _canvas_MouseLeave;
            this.Canvas.MouseEnter  += _canvas_MouseEnter;
            this.Canvas.MouseWheel  += Canvas_MouseWheel;
            this.Canvas.KeyDown     += Canvas_KeyDown;
            this.Canvas.MouseUp     += Canvas_MouseUp;

            CreatePrice();
            CreatePriceScale();
            CreateTimeScale();
            CreateMovingAverage();




        }

        private void CreateMovingAverage()
        {
            movingAverage = new MovingAverage(Price, 20);
            Canvas.AddMovingAverage(movingAverage.meanVisual);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Canvas.Focus();
            e.Handled = true;
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                _priceScale.PriceScaleVisual.TopPrice+= _priceScale.PriceScaleVisual.DSpan / 10;
                _priceScale.PriceScaleVisual.BottomPrice+= _priceScale.PriceScaleVisual.DSpan / 10;
                Canvas.InvalidateVisual();
            }
            else if (e.Key == Key.Down)
            {
                _priceScale.PriceScaleVisual.TopPrice -=_priceScale.PriceScaleVisual.DSpan / 10;
                _priceScale.PriceScaleVisual.BottomPrice-=_priceScale.PriceScaleVisual.DSpan / 10; 
                Canvas.InvalidateVisual();
            }
            else if (e.Key == Key.Right)
            {
                if (TimeScale.TimeScaleVisual.ZoomTopOffset > 0) 
                {
                    TimeScale.TimeScaleVisual.ZoomTopOffset--;
                    Canvas.InvalidateVisual();
                }
            }
            else if (e.Key == Key.Left)
            {
                if (TimeScale.TimeScaleVisual.ZoomTopOffset < TimeScale.TimeScaleVisual.DrawTimes.Count - 2)
                {
                    TimeScale.TimeScaleVisual.ZoomTopOffset++;
                    Canvas.InvalidateVisual();
                }
            }
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
           if (e.MiddleButton == MouseButtonState.Pressed)
            {
                TimeScale.TimeScaleVisual.Zoom = TimeScale.TimeScaleVisual.StarterZoom;
            }
            if (!(e.RightButton == MouseButtonState.Pressed))
            {
                if (e.Delta > 0)
                {
                    if (_priceScale.PriceScaleVisual.DSpan > 0)
                    {
                        _priceScale.PriceScaleVisual.TopPrice--;
                        _priceScale.PriceScaleVisual.BottomPrice++;
                        _priceScale.PriceScaleVisual.UpdateSpan();
                        if (_priceScale.PriceScaleVisual.DSpan < 0)
                        {
                            _priceScale.PriceScaleVisual.TopPrice++;
                            _priceScale.PriceScaleVisual.BottomPrice--;
                            _priceScale.PriceScaleVisual.UpdateSpan();
                        }
                        Canvas.InvalidateVisual();
                    }


                }
                else if (e.Delta < 0)
                {
                    if (_priceScale.PriceScaleVisual.DSpan > 0)
                    {
                        _priceScale.PriceScaleVisual.TopPrice++;
                        _priceScale.PriceScaleVisual.BottomPrice--;
                        _priceScale.PriceScaleVisual.UpdateSpan();
                        if (_priceScale.PriceScaleVisual.DSpan < 0)
                        {
                            _priceScale.PriceScaleVisual.TopPrice--;
                            _priceScale.PriceScaleVisual.BottomPrice++;
                            _priceScale.PriceScaleVisual.UpdateSpan();
                        }
                        Canvas.InvalidateVisual();
                    }


                }
            }
            else
            {
                if ((e.Delta > 0) &&(TimeScale.TimeScaleVisual.Zoom < (TimeScale.TimeScaleVisual.DrawTimes.Count - 1)))
                {

                    TimeScale.TimeScaleVisual.Zoom = TimeScale.TimeScaleVisual.Zoom + 2;
                    Canvas.InvalidateVisual();
                }
                else if ((e.Delta < 0) && (TimeScale.TimeScaleVisual.Zoom > 0) && (TimeScale.TimeScaleVisual.Zoom > (TimeScale.TimeScaleVisual.DrawTimes.Count - 400)))
                {

                    TimeScale.TimeScaleVisual.Zoom = TimeScale.TimeScaleVisual.Zoom - 2;
                    Canvas.InvalidateVisual();
                }
            }
            e.Handled = true;
        }

        private void _canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            (Canvas as GoTCanvas).AddPriceNavigator();
            e.Handled = true;
        }

        private void _canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            (Canvas as GoTCanvas).RemovePriceNavigator();
            e.Handled = true;
        }

        private void _canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void _canvas_MouseMove(object sender, MouseEventArgs e)
        {
            DrawingVisual? vs;
            GoTCanvas? aux;
            double Y;
            double X;
            Point point;
            aux = (e.OriginalSource as GoTCanvas);
            Y = (e.GetPosition(Canvas)).Y;
            X = (e.GetPosition(Canvas)).X;
            point = new Point(X, Y);
           
           
            
            // Redraws pricetag
            if (_priceScale.PriceScaleVisual != null)
            {
                if (Y <= Canvas.ActualHeight - PriceScaleVisual.c_PriceScaleHOffset)
                {
                    _priceScale.PriceScaleVisual.DrawPriceTag(Y, Canvas);
                }
            }


            // Hightlight candle in case is over
            if (aux != null)
            {

                for (int i = 0; i < aux.VisualsCount() - 1; ++i)
                {
                    vs = aux.GetDrawingVisual(i);

                    if (vs != null)
                    {

                        HitTestResult result = vs.HitTest(point);
                        aux.HitTestCallback(result);
                        if (result != null) // Achou o objeto sai do loop
                        {
                            
                            break;
                        }
                    }
                }
            }

            // Hide the cursor in case is over PriceScale
            if (X > (Canvas.ActualWidth - PriceScaleVisual.c_PriceScaleWOffset))
            {
                if (Owner is Window)
                {

                    Window window = (Window)Owner;
                    if ((window.Cursor != null) && (window.Cursor.Equals(Cursors.Arrow)))
                    {
                        window.Cursor = Cursors.None;
                    }


                }
            }
            else
            {
                if (Owner is Window)
                {
                    Window window = (Window)Owner;
                    if ((window.Cursor != null) && (window.Cursor.Equals(Cursors.None)))
                    {
                        window.Cursor = Cursors.Arrow;
                    }

                }
            }

        }

        public void Paint()
        {

        }
        public void CreatePriceScale()
        {
            _priceScale = new PriceScale(Price);
            this.Canvas.AddPriceScale(_priceScale.PriceScaleVisual);
        }
        public void CreateTimeScale()
        {
            TimeScale = new TimeScale(Price);
            this.Canvas.AddTimeScale(TimeScale.TimeScaleVisual);
        }
        public void CreatePrice()
        {
            Price = new Price(); //Create Test
            this.Canvas.AddPrice(Price.PriceVisual);
        }


    }
}
