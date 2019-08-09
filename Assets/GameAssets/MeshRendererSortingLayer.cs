using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshRendererSortingLayer : MonoBehaviour
{
    [SerializeField]
	string sortingLayerName;     // The name of the sorting layer the particles should be set to.
    [SerializeField]
    int sortingOrder;

    void Start()
    {
        // Set the sorting layer of the mesh renderer.
        GetComponent<MeshRenderer>().GetComponent<Renderer>().sortingLayerName = sortingLayerName;
        GetComponent<MeshRenderer>().GetComponent<Renderer>().sortingOrder = sortingOrder;
    }
}
