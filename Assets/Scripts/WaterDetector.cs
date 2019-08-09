using UnityEngine;
using System.Collections;


//using UnityEditor;
//using UnityEngine;
//using System;

public class WaterDetector : MonoBehaviour {

    [SerializeField]
    float fudgeFactor = 10.0f;

    void OnTriggerEnter2D(Collider2D Hit)
    {
        if (Hit.GetComponent<Rigidbody2D>() != null)
        {
            float mass = Hit.GetComponent<Rigidbody2D>().mass;
            // TODO: when if/unity 5.4f, do .Cast on the collider to get point of impact
            transform.GetComponentInParent<Water>().
            Splash(Hit.transform.position.x, Hit.GetComponent<Rigidbody2D>().velocity.y * mass / (mass*fudgeFactor));
        }
    }

    //void OnTriggerExit2D(Collider2D Hit)
    //{
    //    if (Hit.GetComponent<Rigidbody2D>() != null)
    //    {
    //        // TODO: when if/unity 5.4f, do .Cast on the collider to get point of impact
    //        transform.GetComponentInParent<Water>().
    //        Splash(Hit.transform.position.x, Hit.GetComponent<Rigidbody2D>().velocity.y * Hit.GetComponent<Rigidbody2D>().mass / fudgeFactor);
    //    }
    //}

    /*void OnTriggerStay2D(Collider2D Hit)
    {
        //print(Hit.name);
        if (Hit.rigidbody2D != null)
        {
            int points = Mathf.RoundToInt(Hit.transform.localScale.x * 15f);
            for (int i = 0; i < points; i++)
            {
                transform.parent.GetComponent<Water>().Splish(Hit.transform.position.x - Hit.transform.localScale.x + i * 2 * Hit.transform.localScale.x / points, Hit.rigidbody2D.mass * Hit.rigidbody2D.velocity.x / 10f / points * 2f);
            }
        }
    }*/

}



//public class EdgeCollider2DEditor : EditorWindow
//{

//    [MenuItem("Window/EdgeCollider2D Snap")]
//    public static void ShowWindow()
//    {
//        EditorWindow.GetWindow(typeof(EdgeCollider2DEditor));
//    }

//    EdgeCollider2D edge;
//    Vector2[] vertices = new Vector2[0];

//    void OnGUI()
//    {
//        GUILayout.Label("EdgeCollider2D point editor", EditorStyles.boldLabel);
//        edge = (EdgeCollider2D)EditorGUILayout.ObjectField("EdgeCollider2D to edit", edge, typeof(EdgeCollider2D), true);
//        if (vertices.Length != 0)
//        {
//            for (int i = 0; i < vertices.Length; ++i)
//            {
//                vertices[i] = (Vector2)EditorGUILayout.Vector2Field("Element " + i, vertices[i]);
//            }
//        }

//        if (GUILayout.Button("Retrieve"))
//        {
//            vertices = edge.points;
//        }

//        if (GUILayout.Button("Set"))
//        {
//            edge.points = vertices;
//        }
//    }

//    /*
//        void OnSelectionChange() {
//            if (Selection.gameObjects.Length == 1) {
//                EdgeCollider2D aux = Selection.gameObjects[0].GetComponent<EdgeCollider2D>();

//                if (aux) {
//                    edge = aux;
//                    vertices = edge.points;
//                }
//            }
//        }
//    */
//}