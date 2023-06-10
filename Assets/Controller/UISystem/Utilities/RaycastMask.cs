using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public sealed class RaycastMask : MonoBehaviour, ICanvasRaycastFilter
    {
        private Image m_Image;
        private Sprite m_Sprite;


        #region Public Methods
        public bool IsRaycastLocationValid(Vector2 screenPosition, Camera eventCamera)
        {
            // Set sprite
            m_Sprite = m_Image.sprite;

            // SetRectTransform
            RectTransform rectTransform = (RectTransform)transform;

            // GetLocalPosition relative to pivot point
            Vector2 localPositionPivotRelative;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, screenPosition, eventCamera, out localPositionPivotRelative);

            // convert to bottom-left origin coordinates
            Vector2 localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x * rectTransform.rect.width,
                localPositionPivotRelative.y + rectTransform.pivot.y * rectTransform.rect.height);

            Rect spriteRect = m_Sprite.textureRect;
            Rect maskRect = rectTransform.rect;

            Vector2Int pixelPosition = new Vector2Int(0, 0);
            Vector2 ratioPosition = new Vector2(localPosition.x / maskRect.width, localPosition.y / maskRect.height);

            bool isValid = false;

            switch (m_Image.type)
            {
                case Image.Type.Filled:
                    {
                        pixelPosition = new Vector2Int(Mathf.FloorToInt(spriteRect.x + spriteRect.width * ratioPosition.x), Mathf.FloorToInt(spriteRect.y + spriteRect.height * ratioPosition.y));
                        if (m_Image.fillMethod == Image.FillMethod.Vertical || m_Image.fillMethod == Image.FillMethod.Horizontal)
                        {
                            float position = 0;
                            switch (m_Image.fillMethod)
                            {
                                case Image.FillMethod.Horizontal:
                                    position = ratioPosition.x;
                                    break;
                                case Image.FillMethod.Vertical:
                                    position = ratioPosition.y;
                                    break;
                            }
                            isValid = (m_Image.fillOrigin == 0 && position <= m_Image.fillAmount) || (m_Image.fillOrigin == 1 && position >= (1 - m_Image.fillAmount));
                        }
                        else
                        {
                            Vector2 ratioRelativeToCenter = new Vector2();
                            float positionAngle = 0, startFillAngle = 0, variableFillAngle = 0;
                            switch (m_Image.fillMethod)
                            {
                                case Image.FillMethod.Radial90:
                                    variableFillAngle = Mathf.PI / 2;
                                    switch ((Image.Origin90)m_Image.fillOrigin)
                                    {
                                        case Image.Origin90.BottomLeft:
                                            ratioRelativeToCenter = ratioPosition;
                                            startFillAngle = 0;
                                            break;
                                        case Image.Origin90.BottomRight:
                                            ratioRelativeToCenter = ratioPosition + Vector2.left;
                                            startFillAngle = Mathf.PI / 2;
                                            break;
                                        case Image.Origin90.TopLeft:
                                            ratioRelativeToCenter = ratioPosition + Vector2.down;
                                            startFillAngle = -Mathf.PI / 2;
                                            break;
                                        case Image.Origin90.TopRight:
                                            ratioRelativeToCenter = ratioPosition - Vector2.one;
                                            startFillAngle = -Mathf.PI;
                                            break;
                                    }
                                    break;
                                case Image.FillMethod.Radial180:
                                    variableFillAngle = Mathf.PI;
                                    switch ((Image.Origin180)m_Image.fillOrigin)
                                    {
                                        case Image.Origin180.Bottom:
                                            ratioRelativeToCenter = new Vector2(2 * ratioPosition.x - 1, ratioPosition.y);
                                            startFillAngle = 0;
                                            break;
                                        case Image.Origin180.Right:
                                            ratioRelativeToCenter = new Vector2(ratioPosition.x - 1, 2 * ratioPosition.y - 1);
                                            startFillAngle = Mathf.PI / 2;
                                            break;
                                        case Image.Origin180.Left:
                                            ratioRelativeToCenter = new Vector2(ratioPosition.x, 2 * ratioPosition.y - 1);
                                            startFillAngle = -Mathf.PI / 2;
                                            break;
                                        case Image.Origin180.Top:
                                            ratioRelativeToCenter = new Vector2(2 * ratioPosition.x - 1, ratioPosition.y - 1);
                                            startFillAngle = -Mathf.PI;
                                            break;
                                    }
                                    break;
                                case Image.FillMethod.Radial360:
                                    ratioRelativeToCenter = new Vector2(2 * ratioPosition.x - 1, 2 * ratioPosition.y - 1);
                                    variableFillAngle = 2 * Mathf.PI;
                                    switch ((Image.Origin360)m_Image.fillOrigin)
                                    {
                                        case Image.Origin360.Bottom:
                                            startFillAngle = -Mathf.PI / 2;
                                            break;
                                        case Image.Origin360.Top:
                                            startFillAngle = Mathf.PI / 2;

                                            break;
                                        case Image.Origin360.Left:
                                            startFillAngle = -Mathf.PI;
                                            break;
                                        case Image.Origin360.Right:
                                            startFillAngle = 0;
                                            break;
                                    }
                                    break;
                            }
                            positionAngle = Mathf.Atan2(ratioRelativeToCenter.y, ratioRelativeToCenter.x);
                            if (positionAngle < startFillAngle) positionAngle += 2 * Mathf.PI;
                            isValid = (m_Image.fillClockwise && positionAngle >= startFillAngle + variableFillAngle * (1 - m_Image.fillAmount)) || (!m_Image.fillClockwise && positionAngle <= startFillAngle + variableFillAngle * m_Image.fillAmount);
                        }
                    }
                    break;
                case Image.Type.Sliced:
                    {
                        Vector4 border = m_Sprite.border;
                        isValid = true;

                        // x slicing
                        if (localPosition.x < border.x) pixelPosition.x = Mathf.FloorToInt(spriteRect.x + localPosition.x);
                        else if (localPosition.x > maskRect.width - border.z) pixelPosition.x = Mathf.FloorToInt(spriteRect.x + spriteRect.width - (maskRect.width - localPosition.x));
                        else pixelPosition.x = Mathf.FloorToInt(spriteRect.x + border.x + ((localPosition.x - border.x) / (maskRect.width - border.x - border.z)) * (spriteRect.width - border.x - border.z));

                        // y slicing
                        if (localPosition.y < border.y) pixelPosition.y = Mathf.FloorToInt(spriteRect.y + localPosition.y);
                        else if (localPosition.y > maskRect.height - border.w) pixelPosition.y = Mathf.FloorToInt(spriteRect.y + spriteRect.height - (maskRect.height - localPosition.y));
                        else pixelPosition.y = Mathf.FloorToInt(spriteRect.y + border.y + ((localPosition.y - border.y) / (maskRect.height - border.y - border.w)) * (spriteRect.height - border.y - border.w));
                    }
                    break;
                case Image.Type.Simple:
                default:
                    {
                        isValid = true;
                        pixelPosition = new Vector2Int(Mathf.FloorToInt(spriteRect.x + spriteRect.width * ratioPosition.x), Mathf.FloorToInt(spriteRect.y + spriteRect.height * ratioPosition.y));
                    }
                    break;
            }

            try
            {
                isValid &= m_Sprite.texture.GetPixel(pixelPosition.x, pixelPosition.y).a > 0;
                return isValid;
            }
            catch
            {
                Debug.LogError("Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
                return false;
            }
        }
        #endregion

        #region Private Methods
        private void Start()
        {
            m_Image = GetComponent<Image>();
        }
        #endregion
    }
}
