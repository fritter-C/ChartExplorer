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
            CreateGraphicManger();
        }
        private void CreateGraphicManger()
        {
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
                using (DrawingContext dcScale = this._priceScale.RenderOpen())
                {
                    (_priceScale as PriceScaleVisual).DrawPriceScale(dcScale, this);
                }
            }


            if (_timeScale is not null)
            {
                using (DrawingContext dcScale = this._timeScale.RenderOpen())
                {
                    (_timeScale as TimeScaleVisual).DrawTimeScale(dcScale, this);
                }

            }
            if (_price is not null)
            {
              
               (_price as PriceVisual).DrawCandles(this);
        
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
                        drawingVisuals.Add((this._priceScale as PriceScaleVisual).PriceScaleNavigator);
                        this._priceNavigator = (this._priceScale as PriceScaleVisual).PriceScaleNavigator;
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

    }
}
