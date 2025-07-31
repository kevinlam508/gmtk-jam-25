using UnityEngine;

public class CharmInstance : MonoBehaviour
{
    private CharmData _data;

    public void AssignData(CharmData data)
    {
        _data = data;
    }

    private void OnTriggerEnter(Collider other)
    {
        _data.CollisionCallback(other);
    }
}
