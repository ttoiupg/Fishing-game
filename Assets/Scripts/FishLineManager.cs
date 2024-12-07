using UnityEngine;

public class FishLineManager : MonoBehaviour
{
    public Transform LineEnd;
    private Transform LineStart;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        LineStart = GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (LineEnd != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, LineStart.position);
            lineRenderer.SetPosition(1, LineEnd.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
