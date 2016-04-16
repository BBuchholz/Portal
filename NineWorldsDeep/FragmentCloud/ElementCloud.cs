using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace NineWorldsDeep.FragmentCloud
{
    public class ElementCloud : Grid
    {
        private readonly RotateTransform3D rotateTransform;

        private bool isRunRotation;
        private double slowDownCounter;
        private List<ElementCloudItem> elementsCollection;
        private RotationType rotationType;
        private Point rotateDirection;
        private Canvas canvas;

        private double scaleRatio;
        private double opacityRatio;

        public ElementCloud()
        {
            this.Background = Brushes.Transparent;

            canvas = new Canvas()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            this.Children.Add(canvas);
            rotateTransform = new RotateTransform3D();

            SizeChanged += OnPageSizeChanged;

            rotationType = RotationType.Mouse;
            rotateDirection = new Point(100, 0);
            slowDownCounter = 500;
            elementsCollection = new List<ElementCloudItem>();
            scaleRatio = 0.09;
            opacityRatio = 1.3;
        }

        #region Properties

        #region RotateDirectionProperty

        public static DependencyProperty RotateDirectionProperty =
            DependencyProperty.Register("RotateDirection", typeof(Point),
            typeof(ElementCloud), new FrameworkPropertyMetadata(new Point(100, 0),
            new PropertyChangedCallback(OnRotateDirectionChanged)));

        private static void OnRotateDirectionChanged(DependencyObject sender,
                            DependencyPropertyChangedEventArgs e)
        {
            ElementCloud ElementsCloud = (ElementCloud)sender;
            ElementsCloud.rotateDirection = (Point)e.NewValue;
        }

        /// <summary>
        /// Defines the direction of rotation
        /// </summary>
        public Point RotateDirection
        {
            get { return (Point)GetValue(RotateDirectionProperty); }
            set
            {
                SetValue(RotateDirectionProperty, value);
                SetRotateTransform(value);
            }
        }

        #endregion

        #region ScaleRatioProperty

        public static DependencyProperty ScaleRatioProperty =
         DependencyProperty.Register("ScaleRatio", typeof(double),
         typeof(ElementCloud), new FrameworkPropertyMetadata(0.09,
         new PropertyChangedCallback(OnScaleRatioChanged)));

        private static void OnScaleRatioChanged(DependencyObject sender,
                            DependencyPropertyChangedEventArgs e)
        {
            ElementCloud elementsCloud = (ElementCloud)sender;
            elementsCloud.scaleRatio = (double)e.NewValue;
        }

        /// <summary>
        /// Defines a scaling of ElementsCloudItems
        /// when they stays further than other elements
        /// </summary>
        public double ScaleRatio
        {
            get { return (double)GetValue(ScaleRatioProperty); }
            set { SetValue(ScaleRatioProperty, value); }
        }

        #endregion

        #region OpacityRatioProperty

        public static DependencyProperty OpacityRatioProperty =
            DependencyProperty.Register("OpacityRatio", typeof(double),
              typeof(ElementCloud),
              new FrameworkPropertyMetadata(1.3,
              new PropertyChangedCallback(OnOpacityRatioChanged)));

        private static void OnOpacityRatioChanged(DependencyObject sender,
                DependencyPropertyChangedEventArgs e)
        {
            ElementCloud elementsCloud = (ElementCloud)sender;
            elementsCloud.opacityRatio = (double)e.NewValue;
        }

        /// <summary>
        /// Defines a strength of opacity when
        /// ElementsCloudItem stays behind other elements
        /// </summary>
        public double OpacityRatio
        {
            get { return (double)GetValue(OpacityRatioProperty); }
            set { SetValue(OpacityRatioProperty, value); }
        }

        #endregion

        #region Other properties

        /// <summary>
        /// Allow to switch between manual or mouse rotation
        /// </summary>
        public RotationType RotationType
        {
            get { return rotationType; }
            set { rotationType = value; }
        }


        /// <summary>
        /// Collection of elements
        /// </summary>
        public List<ElementCloudItem> ElementsCollection
        {
            get { return elementsCollection; }
            set { elementsCollection = value; }
        }
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Stop rotation
        /// </summary>
        public void Stop()
        {
            if (isRunRotation == true)
            {
                CompositionTarget.Rendering -= OnCompositionTargetRendering;
                this.MouseEnter -= OnGridMouseEnter;
                this.MouseLeave -= OnGridMouseLeave;
                this.MouseMove -= OnGridMouseMove;
                slowDownCounter = 500.0;
                isRunRotation = false;

                rotateTransform.Rotation =
                  new AxisAngleRotation3D(new Vector3D(0, 0, 0), 0);
            }

        }

        /// <summary>
        /// Start rotation
        /// </summary>
        public void Run()
        {
            if (isRunRotation == false)
            {
                CompositionTarget.Rendering += OnCompositionTargetRendering;
                this.MouseEnter += OnGridMouseEnter;
                this.MouseLeave += OnGridMouseLeave;
                this.MouseMove += OnGridMouseMove;
                slowDownCounter = 500.0;
                isRunRotation = true;

                rotateTransform.Rotation =
                   new AxisAngleRotation3D(new Vector3D(0.8, 0.6, 0), 0.5);

                SetRotateTransform(rotateDirection);
                RedrawElements();
            }
        }

        /// <summary>
        /// Configure rotate transformation
        /// </summary>
        ///<param name="position">Defines the direction of rotation</param>
        private void SetRotateTransform(Point position)
        {
            ElementCloudItemSize size = GetElementsSize();

            double x = (position.X - size.XOffset) / size.XRadius;
            double y = (position.Y - size.YOffset) / size.YRadius;
            double angle = Math.Sqrt(x * x + y * y);
            rotateTransform.Rotation =
               new AxisAngleRotation3D(new Vector3D(-y, -x, 0.0), angle);
        }

        /// <summary>
        /// Redraw all elements in Canvas
        /// </summary>
        private void RedrawElements()
        {
            canvas.Children.Clear();

            int length = elementsCollection.Count;
            for (int i = 0; i < length; i++)
            {
                double a = Math.Acos(-1.0 + (2.0 * i) / length);
                double d = Math.Sqrt(length * Math.PI) * a;
                double x = Math.Cos(d) * Math.Sin(a);
                double y = Math.Sin(d) * Math.Sin(a);
                double z = Math.Cos(a);

                elementsCollection[i].CenterPoint = new Point3D(x, y, z);
                canvas.Children.Add(elementsCollection[i]);
            }
        }

        /// <summary>
        /// Rotate blocks
        /// </summary>
        private void RotateBlocks()
        {
            ElementCloudItemSize size = GetElementsSize();

            foreach (ElementCloudItem ElementsCloudItem in elementsCollection)
            {
                Point3D point3D;
                if (rotateTransform.TryTransform(
                        ElementsCloudItem.CenterPoint, out point3D))
                {
                    ElementsCloudItem.CenterPoint = point3D;
                    ElementsCloudItem.Redraw(size, scaleRatio, opacityRatio);
                }
            }
        }

        /// <summary>
        /// Get new size for all elements depending of screen resolution
        /// </summary>
        private ElementCloudItemSize GetElementsSize()
        {
            return new ElementCloudItemSize
            {
                XOffset = canvas.ActualWidth / 2.0,
                YOffset = canvas.ActualHeight / 2.0
            };
        }

        #endregion

        #region Events

        /// <summary>
        /// Redraw buttons with new size
        /// </summary>
        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (elementsCollection != null)
            {
                ElementCloudItemSize size = GetElementsSize();

                foreach (ElementCloudItem eci in elementsCollection)
                {
                    eci.Redraw(size, scaleRatio, opacityRatio);
                }
            }
        }

        /// <summary>
        /// Rendering
        /// </summary>
        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            if (!(isRunRotation || (slowDownCounter <= 0)))
            {
                AxisAngleRotation3D axis =
                    (AxisAngleRotation3D)rotateTransform.Rotation;
                axis.Angle *= slowDownCounter / 500;
                rotateTransform.Rotation = axis;
                slowDownCounter--;
            }
            if (((AxisAngleRotation3D)rotateTransform.Rotation).Angle > 0.05)
            {
                RotateBlocks();
            }
        }

        /// <summary>
        /// Attach new event to grid when mouse enter to grid
        /// </summary>
        private void OnGridMouseEnter(object sender, MouseEventArgs e)
        {
            if (rotationType == RotationType.Mouse && isRunRotation == false)
            {
                this.MouseMove += OnGridMouseMove;
                isRunRotation = true;
                slowDownCounter = 500.0;
            }
        }

        /// <summary>
        /// Detach event when mouse leave grid
        /// </summary>
        private void OnGridMouseLeave(object sender, MouseEventArgs e)
        {
            if (rotationType == RotationType.Mouse && isRunRotation == true)
            {
                this.MouseMove -= OnGridMouseMove;
                isRunRotation = false;
                GC.Collect();
            }
        }

        /// <summary>
        /// Move and rotate buttons when mouse position changed
        /// </summary>
        private void OnGridMouseMove(object sender, MouseEventArgs e)
        {
            if (rotationType == RotationType.Mouse)
                rotateDirection = e.GetPosition(canvas);
            SetRotateTransform(rotateDirection);
        }
        #endregion
    }
}
