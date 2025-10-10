using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyMLP.Controls
{
    public class ZoomPanControl : ContentControl
    {
        private Point _panStartPoint;
        private bool _isPanning;
        private ScaleTransform _zoomTransform;
        private TranslateTransform _panTransform;
        private Grid _containerGrid;

        static ZoomPanControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomPanControl), new FrameworkPropertyMetadata(typeof(ZoomPanControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _containerGrid = GetTemplateChild("PART_Container") as Grid;
            var contentPresenter = GetTemplateChild("PART_Content") as ContentPresenter;

            if (contentPresenter != null)
            {
                _zoomTransform = new ScaleTransform();
                _panTransform = new TranslateTransform();
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(_zoomTransform);
                transformGroup.Children.Add(_panTransform);
                contentPresenter.RenderTransform = transformGroup;
            }

            if (_containerGrid != null)
            {
                _containerGrid.MouseWheel += OnMouseWheel;
                _containerGrid.MouseLeftButtonDown += OnMouseLeftButtonDown;
                _containerGrid.MouseLeftButtonUp += OnMouseLeftButtonUp;
                _containerGrid.MouseMove += OnMouseMove;
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_zoomTransform == null) return;

            var position = e.GetPosition(_containerGrid);
            var scale = e.Delta > 0 ? 1.1 : 1.0 / 1.1;

            var currentScale = _zoomTransform.ScaleX;
            var newScale = currentScale * scale;

            if (newScale < 0.1) newScale = 0.1;
            if (newScale > 5) newScale = 5;

            _zoomTransform.ScaleX = newScale;
            _zoomTransform.ScaleY = newScale;

            _panTransform.X = position.X - (position.X - _panTransform.X) * scale;
            _panTransform.Y = position.Y - (position.Y - _panTransform.Y) * scale;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_containerGrid.CaptureMouse())
            {
                _panStartPoint = e.GetPosition(_containerGrid);
                _isPanning = true;
                Cursor = Cursors.Hand;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                var currentPoint = e.GetPosition(_containerGrid);
                var delta = currentPoint - _panStartPoint;

                _panTransform.X += delta.X;
                _panTransform.Y += delta.Y;

                _panStartPoint = currentPoint;
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                _containerGrid.ReleaseMouseCapture();
                _isPanning = false;
                Cursor = Cursors.Arrow;
            }
        }
    }
}