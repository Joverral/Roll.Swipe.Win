using UnityEngine;
using System.Collections;

public class BasicTriggerMessageScript : MonoBehaviour {

    [SerializeField]
    string messageString;

    [SerializeField]
    bool requireResponse = true;

    [SerializeField]
    bool setInactiveAfter = false;

    [SerializeField]
    bool sendSelfAsParam = false;

    void OnTriggered(GameObject other)
    {
        // HACK:
        // Stupid Unity Layers don't work with Triggers...wtf?
        if (other.tag == "Player")
        {
            var msgOption = requireResponse ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver;
            if(sendSelfAsParam)
            {
                other.SendMessage(messageString, this.gameObject, msgOption);
            }
            else
            {
                other.SendMessage(messageString, msgOption);
            }

            if (setInactiveAfter)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggered(other.gameObject);
    }


    // I found that for fast objects, Enter was unreliable, moving to a stay/enter seemed to catch things more reliably
    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggered(other.gameObject);
    }
}
