using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDisplayer : MonoBehaviour
{
    public BaseZone zone;
    private Transform DisplayerTransform;
    // Start is called before the first frame update
    void Start()
    {
        DisplayerTransform = gameObject.GetComponent<Transform>();
        zone.position = new Vector2(DisplayerTransform.position.x, DisplayerTransform.position.z);
        zone.size = new Vector2(DisplayerTransform.localScale.x, DisplayerTransform.localScale.z);
    }
}
