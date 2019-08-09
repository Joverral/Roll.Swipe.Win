using UnityEngine;
using System.Collections;

public class AlwaysSpawnTop : MonoBehaviour
{
    [SerializeField]
    float yOffset = 0.0f;

    void OnEnable()
    {
        var top = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, 0.0f));
        transform.position = new Vector3(transform.position.x, top.y + yOffset, transform.position.z);
    }
}
