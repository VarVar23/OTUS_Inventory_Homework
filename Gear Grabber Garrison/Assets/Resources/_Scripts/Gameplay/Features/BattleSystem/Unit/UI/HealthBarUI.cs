using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Features.Battlesystem
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private GameObject container;
        
        public void SetFill(float fill)
        {
            if (fillImage != null)
                fillImage.fillAmount = fill;
        }
        
        public void Show()
        {
            if (container != null)
                container.SetActive(true);
        }
        
        public void Hide()
        {
            if (container != null)
                container.SetActive(false);
        }
        private void LateUpdate()
        {
            // Вариант А: строго вертикально
            // transform.localRotation = Quaternion.identity;
            
            // Вариант Б: всегда смотрит в камеру (биллбординг)
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}