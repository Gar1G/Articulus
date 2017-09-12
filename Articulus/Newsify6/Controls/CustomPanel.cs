using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;

namespace Newsify6.Controls
{
    public class CustomPanel : Panel
    {
        // List of Small Rectangles
        private List<Rect> Cells; 

        internal bool Ready { get; set; } = false;

        //3 Dependency Properties (Orientation, AspectRatio, MaxCols that if changed 
        //require Measure and Arrage overrides to be made invalid
        //and must be recalculated

        //Orientation
        public Orientation orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(CustomPanel), new PropertyMetadata(Orientation.Horizontal, OrientationChanged));

        //If Orientation of grid item flow changes then Measure and Arrange calculations must be recalculated
        private static void OrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var obj = o as CustomPanel;
            obj.InvalidateMeasure(); //InvalidArrange() automatically called and layout asynchronously updated
        }

        //Aspect Ratio
        public double AspectRatio
        {
            get { return (double)GetValue(AspectRatioProperty); }
            set { SetValue(AspectRatioProperty, value); }
        }

        public static readonly DependencyProperty AspectRatioProperty = DependencyProperty.Register("AspectRatio", typeof(double), typeof(CustomPanel), new PropertyMetadata(1.0, AspectRatioChanged));

        private static void AspectRatioChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var obj = o as CustomPanel;
            obj.InvalidateMeasure();
        }

        //MaxCols (or max rows depending on orientation)
        public int MaxCols
        {
            get { return (int)GetValue(MaxColsProperty); }
            set { SetValue(MaxColsProperty, value); }
        }

        private static void MaxColsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as CustomPanel;
            control.InvalidateMeasure();
        }

        public static readonly DependencyProperty MaxColsProperty = DependencyProperty.Register("MaxCols", typeof(int), typeof(CustomPanel), new PropertyMetadata(0, MaxColsChanged));

        //Override for Measure
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Ready && base.Children.Count > 0)
            {
                Cells = new List<Rect>();
                double availableWidth = availableSize.Width;
                double availableHeight = availableSize.Height;

                //In event element size will accomodate any content therefore infinite available width
                //Changed available width and height to limits of the window size
                if (double.IsInfinity(availableWidth))
                {
                    availableWidth = Window.Current.Bounds.Width;
                }
                if (double.IsInfinity(availableHeight))
                {
                    availableHeight = Window.Current.Bounds.Height;
                }
                if(availableWidth>= 1920)
                {
                    MaxCols = 6;
                }
                else if(availableWidth < 720)
                {
                    MaxCols = 2;
                }
                else
                {
                    MaxCols = 4;
                }
                // Split space up into cells depending on number of cols (or rows) allowed
                double cell_width = availableWidth / MaxCols;
                double cell_height = cell_width * AspectRatio;

                //If content flow is vertical then switch around cell width and height calculations
                if (orientation == Orientation.Vertical)
                {
                    cell_height = availableHeight / MaxCols;
                    cell_width = cell_height * AspectRatio;
                }

                //Height is generally in terms of integer pixels
                cell_height = Math.Round(cell_height); //turns into nearest integer height
                cell_width = Math.Round(cell_width);

                //Go through each child of customPanel and calculate ColSpans and RowSpan
                //to obtain new widths and heights for each child
                int n = 0;
                foreach (FrameworkElement child in base.Children)
                {
                    int colspan = 1;
                    int rowspan = 1;
                    ColRowSpan(n, child, ref colspan, ref rowspan, MaxCols);//function to recalculate each items col and row spans
                    double width = cell_width * colspan;
                    double height = cell_height * rowspan;
                    var rect = GetNextPosition(Cells, new Size(cell_width, cell_height), new Size(width, height));
                    child.Measure(new Size(width, height));
                    n++;
                }
                return MeasureSize(Cells); //return size covered by list of rects

            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Ready && base.Children.Count > 0)
            {
                int n = 0;
                foreach (FrameworkElement child in base.Children)
                {
                    var rect = Cells[n++];
                    //Position child object and determine size for UI Element
                    //Parents that implement custom layout will call this method to override implementation
                    //to form recursive layout update
                    child.Arrange(rect);
                }
                return MeasureSize(Cells);
            }
            return base.ArrangeOverride(finalSize);
        }

        private Rect GetNextPosition(List<Rect> cells, Size cellSize, Size itemSize)
        {
            if (orientation == Orientation.Horizontal)
            {
                for (int y = 0; ; y++)
                {
                    for (int x = 0; x < this.MaxCols; x++)
                    {
                        var rect = new Rect(new Point(x * cellSize.Width, y * cellSize.Height), itemSize); //Point indicates top left coordinate of rect
                        //Check that new Rect fits in the space available
                        if (RectFitInCells(rect, cells))
                        {
                            cells.Add(rect);
                            return rect;
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; ; x++)
                {
                    for (int y = 0; y < this.MaxCols; y++)
                    {
                        var rect = new Rect(new Point(x * cellSize.Width, y * cellSize.Height), itemSize); //Point indicates top left coordinate of rect
                        //Check that new Rect fits in the space available
                        if (RectFitInCells(rect, cells))
                        {
                            cells.Add(rect);
                            return rect;
                        }
                    }
                }
            }
        }

        private bool RectFitInCells(Rect rect, List<Rect> cells)
        {
            //Check that the boundaries of passed rect do not 
            //intefer with the boundaries of any existing rect in the list
            //NOTE rect.Bottom is LARGER than rect.Top since initial point is Top-left rather tha Bottom-left!!
            return !cells.Any(r => !(r.Left >= rect.Right || r.Right <= rect.Left || r.Top >= rect.Bottom || r.Bottom <= rect.Top));
        }

        private Size MeasureSize(List<Rect> cells)
        {
            double total_width = cells.Max(r => r.Right);
            double total_height = cells.Max(r => r.Bottom);
            return new Size(total_width, total_height);
        }

        protected virtual void ColRowSpan(int index, UIElement element, ref int colSpan, ref int rowSpan, int MaxCols)
        {
            if (MaxCols == 2)
            {
                if (index % 24 == 0 || index % 24 == 5 || index % 24 == 14 || index % 24 == 15)
                {
                    colSpan = 2;
                    rowSpan = 2;
                }
                else if (index % 24 <= 2 || (index % 24 <= 13 && index % 24 > 11))
                {
                    colSpan = 2;
                    rowSpan = 2;
                }
                else if (index % 24 <= 4 || (index % 24 <= 11 && index % 24 > 7) || (index % 24 <= 17 && index % 24 > 15) || (index % 24 <= 23 && index % 24 > 19))
                {
                    colSpan = 2;
                    rowSpan = 2;
                }
                else
                {
                    colSpan = 2;
                    rowSpan = 1;
                }
            }

            else
            {
                if (index % 24 == 0 || index % 24 == 5 || index % 24 == 14 || index % 24 == 15)
                {
                    colSpan = 2;
                    rowSpan = 2;
                }
                else if (index % 24 <= 2 || (index % 24 <= 13 && index % 24 > 11))
                {
                    colSpan = 1;
                    rowSpan = 2;
                }
                else if (index % 24 <= 4 || (index % 24 <= 11 && index % 24 > 7) || (index % 24 <= 17 && index % 24 > 15) || (index % 24 <= 23 && index % 24 > 19))
                {
                    colSpan = 1;
                    rowSpan = 3;
                }
                else
                {
                    colSpan = 1;
                    rowSpan = 1;
                }

            }
        }
    }
}
