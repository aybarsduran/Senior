using System;
using TMPro;
using UnityEngine;

namespace IdenticalStudios
{
    public sealed class TMP_TextSizer : MonoBehaviour
    {
        [Flags]
        public enum Mode
        {
            None = 0,
            Horizontal = 0x1,
            Vertical = 0x2,
            Both = Horizontal | Vertical
        }

        private float MinX
        {
            get
            {
                if ((ControlAxes & Mode.Horizontal) != 0) return MinSize.x;
                return _selfRectTransform.rect.width - Padding.x;
            }
        }

        private float MinY
        {
            get
            {
                if ((ControlAxes & Mode.Vertical) != 0) return MinSize.y;
                return _selfRectTransform.rect.height - Padding.y;
            }
        }

        private float MaxX
        {
            get
            {
                if ((ControlAxes & Mode.Horizontal) != 0) return MaxSize.x;
                return _selfRectTransform.rect.width - Padding.x;
            }
        }

        private float MaxY
        {
            get
            {
                if ((ControlAxes & Mode.Vertical) != 0) return MaxSize.y;
                return _selfRectTransform.rect.height - Padding.y;
            }
        }

        [SerializeField]
        private TextMeshProUGUI Text;

        [SerializeField]
        private bool ResizeTextObject = true;

        [SerializeField]
        private Vector2 Padding;

        [SerializeField]
        private Vector2 MaxSize = new Vector2(1000, float.PositiveInfinity);

        [SerializeField]
        private Vector2 MinSize;

        [SerializeField]
        private Mode ControlAxes = Mode.Both;

        private string _lastText;
        private Mode _lastControlAxes = Mode.None;
        private Vector2 _lastSize;
        private bool _forceRefresh;
        private bool _isTextNull = true;
        private RectTransform _textRectTransform;
        private RectTransform _selfRectTransform;


        // Forces a size recalculation on next Update
        public void Refresh()
        {
            _forceRefresh = true;

            _isTextNull = Text == null;
            if (Text) _textRectTransform = Text.GetComponent<RectTransform>();
            _selfRectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!_isTextNull && (Text.text != _lastText || _lastSize != _selfRectTransform.rect.size || _forceRefresh || ControlAxes != _lastControlAxes))
            {
                var preferredSize = Text.GetPreferredValues(MaxX, MaxY);
                preferredSize.x = Mathf.Clamp(preferredSize.x, MinX, MaxX);
                preferredSize.y = Mathf.Clamp(preferredSize.y, MinY, MaxY);
                preferredSize += Padding;

                if ((ControlAxes & Mode.Horizontal) != 0)
                {
                    _selfRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredSize.x);
                    if (ResizeTextObject)
                    {
                        _textRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredSize.x);
                    }
                }
                if ((ControlAxes & Mode.Vertical) != 0)
                {
                    _selfRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
                    if (ResizeTextObject)
                    {
                        _textRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
                    }
                }

                _lastText = Text.text;
                _lastSize = _selfRectTransform.rect.size;
                _lastControlAxes = ControlAxes;
                _forceRefresh = false;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Refresh();
        }
#endif
    }
}
