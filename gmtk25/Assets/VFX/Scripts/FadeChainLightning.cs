using UnityEngine;
using System.Collections;

public class FadeChainLightning : MonoBehaviour
{
    private TrailRenderer tr;
    private Material mat;
    public float duration = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tr = GetComponentInChildren<TrailRenderer>();
        mat = tr.material;
    }

    public void StartFade()
    {
        StartCoroutine(FadeEffect(duration));
    }
    IEnumerator FadeEffect(float duration)
    {
        float t = 0f;
        while (t < duration) {
            float value = Mathf.Lerp(0f, 1f, t/duration);
            mat.SetFloat("_Dissolve", value);
            t += Time.deltaTime;
            yield return null;
        }

    }

}
