using UnityEngine;

public class GrassInteraction : MonoBehaviour
{
    [SerializeField] float radius = 2f;

    void Update()
    {
        Shader.SetGlobalVector("_PlayerPos", transform.position);
        Shader.SetGlobalFloat("_PlayerRadius", radius);
    }
}
