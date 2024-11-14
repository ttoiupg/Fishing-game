using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    public Transform playerTransform;
    public PlayerControl playerControl;
    public bool CanFish = true;
    public bool IsFishing = false;
    public Transform ZoneContainer;
    public BaseZone CurrentZone;
    private ZoneDisplayer[] Zones;
    // Start is called before the first frame update
    private bool IsInside(Vector2 A,Vector2 SizeA,Vector2 B)
    {
        bool result = false;
        if (A.x - SizeA.x/2f <= B.x && A.x + SizeA.x/2f >= B.x && A.y - SizeA.y/2f <= B.y && A.y + SizeA.y/2f >= B.y)
        {
            result = true;
        }
        return result;
    }
    private void Start()
    {
        playerTransform = gameObject.GetComponent<Transform>();
        playerControl = gameObject.GetComponent<PlayerControl>();
    }
    // Update is called once per frame
    void Update()
    {
        Zones = ZoneContainer.GetComponentsInChildren<ZoneDisplayer>();
        for(int i=0;i< Zones.Length; i++)
        {
            if (IsInside(Zones[i].zone.position, Zones[i].zone.size, new Vector2(playerTransform.position.x, playerTransform.position.z))){
                CurrentZone = Zones[i].zone;
                break;
            }
            else
            {
                CurrentZone = null;
            };
        };
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PendingFishing();
        }
    }


    public void PendingFishing()
    {
        if (IsFishing == false && CanFish)
        {
            IsFishing = true;
            ThrowFishingRod();
        }
        else if (IsFishing)
        {
            IsFishing = false;
            RetrackFishingRod();
        }
    }
    public void ThrowFishingRod()
    {
        playerControl.playerSpeed = 0;
        if (CurrentZone == null)
        {
            Debug.Log("Fishing in no zone");
        }
        else
        {
            Debug.Log($"Lets go fishing at {CurrentZone.name}");
        }
    }
    public void RetrackFishingRod()
    {
        playerControl.playerSpeed = 7.0f;
        Debug.Log("Stoped fishing");
    }
}