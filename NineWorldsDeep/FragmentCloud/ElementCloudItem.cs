using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace NineWorldsDeep.FragmentCloud
{
    // This code is from, with only minor adaptations (comments and variable names):
    //TODO: LICENSE NOTES
    // http://www.codeproject.com/Articles/191026/WPF-Elements-Cloud
    // license at: http://www.codeproject.com/info/cpol10.aspx

    // I am following the tutorial and planning to adapt it to 
    // NwdFragmentCloud, which will be similiar but custom tailored 
    // to the needs of the NineWorldsDeep ecosystem :)

    // NOTE TO SELF: 
    // this link:
    // http://stackoverflow.com/questions/7437004/how-to-make-a-user-control-property-of-type-collectionmyclass-editable-in-form
    // maybe able to solve the issue where the tutorial says:    
    //
    //   "...ElementCloud extends Grid but not UserControl. You should 
    //       not use UserControl class if you want to make a control 
    //       which has a collection of some elements..."
    //
    // which sounds suspect to me, and I would like to try to create 
    // a new version inheriting from UserControl but for now will just 
    // follow the tutorial verbatim

    /// <summary>
    /// ElementsCloudItem represents a grid able to contain
    /// any UIElement object
    /// </summary>
    public class ElementCloudItem : Grid
    {
        private ScaleTransform itemScaling;

        public ElementCloudItem(Point3D point3D, UIElement element)
        {
            CenterPoint = point3D;
            itemScaling = new ScaleTransform();
            this.LayoutTransform = itemScaling;
            Children.Add(element);
            itemScaling.CenterX = CenterPoint.X;
            itemScaling.CenterY = CenterPoint.Y;
        }

        public ElementCloudItem()
        {
            CenterPoint = new Point3D();
            itemScaling = new ScaleTransform();
            this.LayoutTransform = itemScaling;
            itemScaling.CenterX = CenterPoint.X;
            itemScaling.CenterY = CenterPoint.Y;
        }

        #region CenterPoint property

        public static DependencyProperty CenterPointProperty =
            DependencyProperty.Register("CenterPoint", typeof(Point3D),
                typeof(ElementCloudItem), new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(OnCenterPointChanged)));

        private static void OnCenterPointChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            ElementCloudItem eci = (ElementCloudItem)sender;
            eci.CenterPoint = (Point3D)e.NewValue;
        }

        /// <summary>
        /// CenterPoint of the item
        /// </summary>
        public Point3D CenterPoint
        {
            get { return (Point3D)GetValue(CenterPointProperty); }
            set { SetValue(CenterPointProperty, value); }
        }

        #endregion

        public void Redraw(ElementCloudItemSize size,
                           double scaleRatio,
                           double opacityRatio)
        {
            itemScaling.ScaleX = itemScaling.ScaleY =
                Math.Abs((16 + CenterPoint.Z * 4) * scaleRatio);
            Opacity = CenterPoint.Z + opacityRatio;
            Canvas.SetLeft(this,
                (size.XOffset + CenterPoint.X * size.XRadius)
                - (ActualHeight / 2.0));
            Canvas.SetTop(this,
                (size.YOffset - CenterPoint.Y * size.YRadius)
                - (ActualHeight / 2.0));
            Canvas.SetZIndex(this,
                (int)(CenterPoint.Z * Math.Min(size.XRadius, size.YRadius)));
        }
    }
}