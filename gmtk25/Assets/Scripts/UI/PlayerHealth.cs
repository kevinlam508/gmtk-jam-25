using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Color _mediumHurt;
    [SerializeField] private Color _hurt;
    [SerializeField] private Image _fill;
    private Color _defaultColor;

    void Start()
    {
        _defaultColor = _fill.color;
    }

    public void OnHealthChange(int current, int total)
    {
        _slider.value = (float)current / total;

        if (_slider.value <= 0.7f)
        {
            _fill.color = _mediumHurt;
            if (_slider.value <= 0.3f) _fill.color = _hurt;
        }
        else _fill.color = _defaultColor;
    }

    public void OnDamageTaken()
    {
        _slider.transform.DOShakePosition(.3f, 10);
    }
}
