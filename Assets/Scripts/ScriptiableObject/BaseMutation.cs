using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mutation", menuName = "Game/Fish/Mutation")]
public class BaseMutation : ScriptableObject
{
    public  string id;
    public string Description;
    public float OneIn;
}
