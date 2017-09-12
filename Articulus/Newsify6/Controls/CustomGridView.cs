
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.Foundation;

namespace Newsify6.Controls
{
    public class CustomGridView : ListViewBase //inherits from same class as GridView and ListView
    {
        //Encapsulates custom panel and adds on a scroll bar
        private ScrollViewer Scroll = null;
        private CustomPanel Panel = null;
        private bool Initialised = false;

        public CustomGridView()
        {
            this.DefaultStyleKey = typeof(CustomGridView); //sets style of control to the customGridView
            this.LayoutUpdated += layout_updated;
        }

        // 5 Dependency Properties that will affect layout of this gridView
        // Margin, Padding, MaxCols (or rows), AspectRatio, Orientation
        //Margin
        public Thickness ItemMargin
        {
            get { return (Thickness)GetValue(ItemMarginProperty); }
            set { SetValue(ItemMarginProperty, value); }
        }

        public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register("ItemMargin", typeof(Thickness), typeof(CustomGridView), new PropertyMetadata(new Thickness(2)));

        public Thickness ItemPadding
        {
            get { return (Thickness)GetValue(ItemPaddingProperty); }
            set { SetValue(ItemPaddingProperty, value); }
        }

        public static readonly DependencyProperty ItemPaddingProperty = DependencyProperty.Register("ItemPadding", typeof(Thickness), typeof(CustomGridView), new PropertyMetadata(new Thickness(2)));

        //Orientation
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(CustomPanel), new PropertyMetadata(Orientation.Horizontal, OrientationChanged));

        //If Orientation of grid item flow changes then Measure and Arrange calculations must be recalculated
        private static void OrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var obj = o as CustomGridView;
            obj.SetOrientation((Orientation)e.NewValue);
            //InvalidArrange() automatically called and layout asynchronously updated
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
            var obj = o as CustomGridView;
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
            var obj = o as CustomGridView;
            obj.InvalidateMeasure();
        }

        public static readonly DependencyProperty MaxColsProperty = DependencyProperty.Register("MaxCols", typeof(int), typeof(CustomPanel), new PropertyMetadata(0, MaxColsChanged));


        private void SetOrientation(Orientation Orientation)
        {
            if (Initialised && Scroll != null)
            {
                //if content laid out horizontally then scrolling must be vertical
                if (Orientation == Orientation.Horizontal)
                {
                    Scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    Scroll.HorizontalScrollMode = ScrollMode.Disabled;
                    Scroll.VerticalScrollBarVisibility = (ScrollBarVisibility)this.GetValue(ScrollViewer.VerticalScrollBarVisibilityProperty);
                    Scroll.VerticalScrollMode = ScrollMode.Auto;

                }
                else
                {
                    Scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    Scroll.VerticalScrollMode = ScrollMode.Disabled;
                    if ((ScrollBarVisibility)this.GetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty) == ScrollBarVisibility.Disabled)
                    {
                        Scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    }
                    else
                    {
                        Scroll.HorizontalScrollBarVisibility = (ScrollBarVisibility)this.GetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty);
                    }
                    Scroll.HorizontalScrollMode = ScrollMode.Auto;
                }
            }
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListViewItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {

            var Container = element as ListViewItem;
            Container.Margin = this.ItemMargin;
            Container.Padding = this.ItemPadding;
            base.PrepareContainerForItemOverride(element, item);

        }

        protected override void OnApplyTemplate()
        {
            Scroll = base.GetTemplateChild("scrollViewer") as ScrollViewer;
            Initialised = true;
            SetOrientation(this.Orientation);
            base.OnApplyTemplate();
        }

        private void layout_updated(object sender, object o)
        {
            if (Panel == null)
            {
                Panel = base.ItemsPanelRoot as CustomPanel;
                if (Panel != null)
                {
                    Panel.Ready = true;
                    Panel.SetBinding(CustomPanel.OrientationProperty, new Binding { Source = this, Path = new PropertyPath("Orientation") });
                    Panel.SetBinding(CustomPanel.AspectRatioProperty, new Binding { Source = this, Path = new PropertyPath("AspectRatio") });
                    Panel.SetBinding(CustomPanel.MaxColsProperty, new Binding { Source = this, Path = new PropertyPath("MaxCols") });
                }
            }
        }

    }
}
