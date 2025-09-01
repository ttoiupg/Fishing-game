using System;
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
        zone.position = new Vector2(DisplayerTransform.position.x, DisplayerTransform.position.y);
        zone.size = new Vector2(DisplayerTransform.localScale.x, DisplayerTransform.localScale.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            player.currentZone = zone;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            player.currentZone = null;
        }
    }
}
