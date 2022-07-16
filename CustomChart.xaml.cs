using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChartExplorer
{

    public partial class CustomChart : Window
    {

        private GraphicManager _graphicManager;

        public CustomChart()
        {
            InitializeComponent();
            this.Cursor = Cursors.Arrow;
            _graphicManager = new GraphicManager(GoT, this);
        }
    }
    public class GoTCanvas : Canvas
    {
        public List<DrawingVisual> drawingVisuals;
        private DrawingVisual? _priceScale;
        private DrawingVisual? _priceNavigator;
        private DrawingVisual? _timeScale;
        private DrawingVisual? _price;
        private DrawingVisual? _average;
        public int VisualsCount()
        {
            return drawingVisuals.Count();
        }
        public DrawingVisual? GetDrawingVisual(int nIndex)
        {
            if (nIndex < drawingVisuals.Count())
            {
                return (drawingVisuals[nIndex]);
            }
            else
            {
                return null;
            }
        }
        public GoTCanvas() : base()
        {
            drawingVisuals = new List<DrawingVisual>();
            ClipToBounds = true;

        }
        public DrawingVisual GetNavigatorDrawingVisual()
        {
            return _priceNavigator;
        }
        public DrawingVisual GetPriceScale()
        {
            return _priceScale;

        }
        protected override void OnRender(DrawingContext dc)
        {
            if (_priceScale is not null)
            {
                
               ((PriceScaleVisual)_priceScale).DrawPriceScale(this);
        
            }
            if (_timeScale is not null)
            {
                
                ((TimeScaleVisual)_timeScale).DrawTimeScale(this);
               
            }
            if (_price is not null)
            {

                ((PriceVisual)_price).DrawCandles(this);

            }
            if (_average is not null)
            {
                ((MovingAverageVisual)_average).Draw(this);
            }

            base.OnRender(dc);
        }

        public void AddPriceScale(PriceScaleVisual priceScale)
        {

            drawingVisuals.Add(priceScale);
            this._priceScale = priceScale;
            AddVisualChild(this._priceScale);
        }
        public void AddTimeScale(TimeScaleVisual timeScale)
        {
            drawingVisuals.Add(timeScale);
            this._timeScale = timeScale;
            AddVisualChild(timeScale);
        }
        public void RemovePriceNavigator()
        {
            if (this._priceNavigator != null)
            {
                drawingVisuals.Remove(this._priceNavigator);
                RemoveVisualChild(_priceNavigator);
                this._priceNavigator = null;
            }
        }
        public DrawingVisual GetPriceIndicator()
        {
            return _price;
        }
        public void AddPriceNavigator()
        {
            if ((this._priceNavigator is null) && (this._priceScale is not null))
            {
                if ((this._priceScale is PriceScaleVisual))
                {

                    {
                        drawingVisuals.Add(((PriceScaleVisual)this._priceScale).PriceScaleNavigator);
                        this._priceNavigator = ((PriceScaleVisual)this._priceScale).PriceScaleNavigator;
                        AddVisualChild(this._priceNavigator);

                    }
                }
            }
        }

        public void AddPrice(PriceVisual priceVisual)
        {
            drawingVisuals.Add(priceVisual);
            this._price = priceVisual;
            AddVisualChild(_price);

        }

        public void AddMovingAverage(MovingAverageVisual meanVisual)
        {
            drawingVisuals.Add(meanVisual);
            this._average = meanVisual;
            AddVisualChild(_average);   
        }
        protected override Visual GetVisualChild(int index)
        {
            return drawingVisuals[index];
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return drawingVisuals.Count;
            }
        }


        public HitTestResultBehavior HitTestCallback(HitTestResult result)
        {
            if (result is not null)
            {

                if ((result.VisualHit.GetType() == typeof(PriceVisual)) || (result.VisualHit.GetType() == typeof(MovingAverageVisual)))
                {
                 
                   ((DrawingVisual)result.VisualHit).Opacity = 0.4;
                
                  
                }
                else
                {
                    ((DrawingVisual)result.VisualHit).Opacity = 1.0;
                }
            }
            else
            {
                this._price.Opacity = 1.0;
                this._average.Opacity = 1.0;
            }
            return HitTestResultBehavior.Stop;
        }

    }
}
