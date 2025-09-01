using UnityEngine;

public class GrassInteraction : MonoBehaviour
{
    [SerializeField] float radius = 2f;
    [SerializeField] Vector3 offset;

    void Update()
    {
        Shader.SetGlobalVector("_PlayerPos", transform.position + offset);
        Shader.SetGlobalFloat("_PlayerRadius", radius);
    }
}
