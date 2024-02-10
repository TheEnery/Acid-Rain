
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace AcidRain.UI
{
    public class DroneBar : MonoBehaviour
    {
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Image _fill;
        [SerializeField] private Gradient _chargeGradient;
        [SerializeField] private bool _invertAlpha;
        [SerializeField] private Slider _slider;

        private float _leftAlpha => _invertAlpha ? 0f : 1f;
        private float _rightAlpha => _invertAlpha ? 1f : 0f;

        public float? Level
        {
            set
            {
                if (value == null)
                {
                    _slider.normalizedValue = 1f;
                    _fill.material.SetColor("_LeftColor", _defaultColor.WithAlpha(_leftAlpha));
                    _fill.material.SetColor("_RightColor", _defaultColor.WithAlpha(_rightAlpha));
                }
                else
                {
                    _slider.normalizedValue = value.Value;
                    var color = _chargeGradient.Evaluate(_slider.normalizedValue);
                    _fill.material.SetColor("_LeftColor", color.WithAlpha(_leftAlpha));
                    _fill.material.SetColor("_RightColor", color.WithAlpha(_rightAlpha));
                }
            }
        }

        private void Awake()
        {
            Level = null;
        }

        private void OnDestroy()
        {
            Level = null;
        }
    }
}