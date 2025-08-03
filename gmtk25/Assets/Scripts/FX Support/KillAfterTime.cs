    using UnityEngine;

public class KillAfterTime : MonoBehaviour
{
    [SerializeField] private float _killTime;

    private float _timePassed;

    // Update is called once per frame
    void Update()
    {
        if (_killTime < _timePassed)
        {
            Destroy(gameObject);
        }

        _timePassed += Time.deltaTime;
    }
}
