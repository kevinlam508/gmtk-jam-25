using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    public void OnHealthChange(int current, int total)
    {
        _slider.value = (float)current / total;
    }
}
