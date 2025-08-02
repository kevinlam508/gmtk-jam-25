using UnityEngine;

public class AngleConeVisualizer : MonoBehaviour
{
    public float AngleRange = 135f; 
    public float AngleOffset = 0; 
    public float MaxDistanceForSpawn = 5f;
    public int debugCurveSmoothness = 30;
    private Vector2 direction;

    void OnDrawGizmos()
    {
        float angleRange = Mathf.Deg2Rad * AngleRange;
        float angleOffset = Mathf.Deg2Rad * AngleOffset;

        float angleCombined = angleOffset + angleRange;

        
        Vector3 rangeBeginning = new Vector3(Mathf.Cos(angleOffset), 0, Mathf.Sin(angleOffset));
        Vector3 rangeEnd = new Vector3(Mathf.Cos(angleCombined), 0, Mathf.Sin(angleCombined));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (rangeBeginning * MaxDistanceForSpawn));
        Gizmos.DrawLine(transform.position, transform.position + (rangeEnd * MaxDistanceForSpawn));


        for (int i = 0; i < debugCurveSmoothness; i++)
        {
            float t0 = (angleOffset) + angleRange * (i / debugCurveSmoothness);//currentPoint
            float t1 = (angleOffset) + angleRange * ((i + 1) / debugCurveSmoothness);//nextPoint this creates a drawing between the two
            Vector3 p0 = new Vector3(Mathf.Cos(t0), 0, Mathf.Sin(t0)) * MaxDistanceForSpawn;
            Vector3 p1 = new Vector3(Mathf.Cos(t1), 0, Mathf.Sin(t1)) * MaxDistanceForSpawn;
            Gizmos.DrawLine(transform.position + p0, transform.position + p1);
        }

        direction = GetDirection(angleRange, angleOffset) * MaxDistanceForSpawn;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(direction.x, 0, direction.y));
    }

    Vector2 GetDirection(float angle, float angleMin)
    {
        float random = Random.value * angle + angleMin;
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }
}
