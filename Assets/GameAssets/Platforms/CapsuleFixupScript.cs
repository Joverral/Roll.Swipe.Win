using UnityEngine;
using System.Collections;

public class CapsuleFixupScript : MonoBehaviour {

    [SerializeField]
    BoxCollider2D modelBox2d;

    [SerializeField]
    BoxCollider2D box2d;

    [SerializeField]
    CircleCollider2D leftCap;

    [SerializeField]
    CircleCollider2D rightCap;

    float initialRadius = 0.5f;
    public void Awake()
    {
        initialRadius = 0.5f; // / transform.localScale.x;
    }

    public void OnEnable()
    {
        leftCap.radius = initialRadius / transform.localScale.x;
        rightCap.radius = initialRadius / transform.localScale.x;

        box2d.size = new Vector2( 1.0f - 2.0f * leftCap.radius, 1.0f);

        leftCap.offset = new Vector2(-box2d.size.x / 2.0f, 0.0f);
        rightCap.offset = new Vector2(box2d.size.x / 2.0f, 0.0f);
    }
}
