using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "Bool Variable", menuName = "Variable/Bool")]
public class BoolVariable : Variable<bool>
{
    public void Toggle()
    {
        Value = !Value;
    }
}