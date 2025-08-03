using UnityEngine;

public class HitGlow : MonoBehaviour
{
    Material mat;
    void Start()
    {
        mat = GetComponent<SkinnedMeshRenderer>().material;
    }

    public void Hit()
    {
        mat.SetFloat("_CurrentTime", Time.time);
    }
}
