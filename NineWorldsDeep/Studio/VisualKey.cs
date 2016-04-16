using System.Windows.Shapes;

namespace NineWorldsDeep.Studio
{
    public class VisualKey
    {
        private Ellipse _keyDot;
        private Rectangle _key;
        private bool _isHighlighted;

        public VisualKey(Ellipse keyDot, Rectangle key, bool isHighlighted)
        {
            this._keyDot = keyDot;
            this._key = key;
            this._isHighlighted = isHighlighted;
        }

        public Rectangle KeyRectangle
        {
            get { return _key; }
        }

        public Ellipse KeyDot
        {
            get { return _keyDot; }
        }

        public bool IsHighlighted
        {
            get { return _isHighlighted; }

            set
            {
                _isHighlighted = value;
                UpdateDisplay();
            }
        }

        public void ToggleHighlight()
        {
            IsHighlighted = !IsHighlighted;
        }

        public void ClearHighlight()
        {
            IsHighlighted = false;
        }

        private void UpdateDisplay()
        {
            if (_isHighlighted)
            {
                this._keyDot.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this._keyDot.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}