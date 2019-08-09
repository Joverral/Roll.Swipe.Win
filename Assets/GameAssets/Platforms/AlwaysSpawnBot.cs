using UnityEngine;
using System.Collections;

public class AlwaysSpawnBot : MonoBehaviour {

    [SerializeField]
    float yOffset = 0.0f;

    void OnEnable()
    {
        var bot = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
        transform.position = new Vector3(transform.position.x, bot.y + yOffset, transform.position.z);
    }
}
