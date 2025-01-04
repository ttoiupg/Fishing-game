using System.Collections;
using UnityEngine;

public class VisualFXManager : MonoBehaviour
{
    public static VisualFXManager Instance;

    [Header("Bobber")]
    public Object BobberObject;
    public FishLineManager FishLineManager;
    public float ThrowStrength;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private IEnumerator BobberNumerator(Vector3 position, int direction)
    {
        yield return new WaitForSeconds(0.65f);
        GameObject bobber = Instantiate(BobberObject, position, new Quaternion(0, 0, 0, 0)) as GameObject;
        Rigidbody rigidbody = bobber.GetComponent<Rigidbody>();
        Vector3 force = new Vector3(15 * direction * ThrowStrength, 30, 0);
        rigidbody.AddRelativeForce(force, ForceMode.Impulse);
        FishLineManager.LineEnd = bobber.transform;
    }
    public void SpawnBobber(Vector3 position,int direction)
    {
        StartCoroutine(BobberNumerator(position, direction));
    }
    public void DestroyBobber()
    {
        GameObject[] bobbers = GameObject.FindGameObjectsWithTag("Bobber");
        for (int i = 0; i < bobbers.Length; i++)
        {
            Destroy(bobbers[i]);
        }
    }
}
