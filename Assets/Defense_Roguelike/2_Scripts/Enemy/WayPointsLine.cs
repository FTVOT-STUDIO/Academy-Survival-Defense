using UnityEngine;

public class WayPointsLine : MonoBehaviour
{
    LineRenderer lineRenderer;

    public Transform[] wayPoints;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        wayPoints = new Transform[transform.childCount];

        for (int i = 0; i < wayPoints.Length; i++)
        {
            wayPoints[i] = transform.GetChild(i);
        }
        SetWayPointLine();
    }

    void SetWayPointLine()
    {
        lineRenderer.positionCount = wayPoints.Length + 1;
        lineRenderer.SetPosition(0, transform.position);

        for (int i = 0; i < wayPoints.Length; i++)
        {
            lineRenderer.SetPosition(i + 1, wayPoints[i].position);
        }
    }

}
