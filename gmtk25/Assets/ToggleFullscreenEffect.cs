using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ToggleFullscreenEffect : MonoBehaviour
{
    public ScriptableRendererFeature screenFX;
    public Material screenFXMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // screenFX.SetActive(false);
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         Hit();
    //     }
    // }

    public void Hit() {
        screenFXMaterial.SetFloat("_CurrentTimestamp", Time.time);
    }
}
