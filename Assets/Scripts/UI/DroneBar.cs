
using UnityEngine;
using UnityEngine.UI;

namespace AcidRain.UI
{
    public class DroneBar : MonoBehaviour
    {
        [SerializeField] private Color DefaultFill;
        [SerializeField] private Gradient _gradient;
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _fill;

        public float? Level
        {
            set
            {
                if (value == null)
                {
                    _slider.normalizedValue = 1f;
                    _fill.color = DefaultFill;
                }
                else
                {
                    _slider.normalizedValue = value.Value;
                    _fill.color = _gradient.Evaluate(_slider.normalizedValue);
                }
            }
        }

        private void Awake()
        {
            Level = null;
        }
    }
}