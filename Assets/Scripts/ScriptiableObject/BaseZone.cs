using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Zone", menuName = "Zone")]
public class BaseZone : ScriptableObject
{
    public string Description = "This is the default zone.";
    public Vector2 position;
    public Vector2 size;
    public BaseFish[] FeaturedFish = {};
}
