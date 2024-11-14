using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mutation", menuName = "Mutation")]
public class BaseMutation : ScriptableObject
{
    public string Description;
    public float OneIn;
}
