using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    public PlayerControl playerControl;
    public bool CanFish = true;
    public bool IsFishing = false;
    // Start is called before the first frame update
    private void Start()
    {
        playerControl = gameObject.GetComponent<PlayerControl>();
    }
    // Update is called once per frame
    void Update()
    {
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
        Debug.Log("Lets go fishing");
    }
    public void RetrackFishingRod()
    {
        playerControl.playerSpeed = 7.0f;
        Debug.Log("Stoped fishing");
    }
}