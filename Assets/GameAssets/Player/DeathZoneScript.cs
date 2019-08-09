using UnityEngine;
using System.Collections;

public class DeathZoneScript : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        // we only have one trigger right now...
        //Debug.Log("Death Zone Trigger entered");
        other.gameObject.SendMessage("OnEnterDeathZone", SendMessageOptions.RequireReceiver);
    }


    void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("Death Zone Trigger Stay");
        other.gameObject.SendMessage("OnEnterDeathZone", SendMessageOptions.RequireReceiver);
    }
}
