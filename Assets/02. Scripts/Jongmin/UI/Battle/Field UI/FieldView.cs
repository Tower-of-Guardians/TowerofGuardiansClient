using UnityEngine;
using UnityEngine.UI;

namespace Jongmin
{
    public class FieldView : MonoBehaviour
    {
        [SerializeField] private Transform cardRoot;
        [SerializeField] private Image disableImage;
        [SerializeField] private PreviewCard previewCard;

        public Transform CardRoot => cardRoot;

        public void TogglePreview(bool isActive)
        {
            previewCard.gameObject.SetActive(isActive);
            previewCard.transform.SetAsFirstSibling();
        }

        public void UpdatePreviewPosition(Vector2 position)
        {
            previewCard.RectTransform.anchoredPosition = position;
        }
    }
}