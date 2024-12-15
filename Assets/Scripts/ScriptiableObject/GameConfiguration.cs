using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigurationObject", menuName = "GameConfig")]
public class GameConfiguration : ScriptableObject
{
    public List<BaseFish> PondFishes;
}
