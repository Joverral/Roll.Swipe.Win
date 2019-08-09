using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShapeController : MonoBehaviour {

    [SerializeField]
    GameObject[] ShapePrefabs;
    List<GameObject> ShapeGhosts;

    [SerializeField]
    Material GhostMaterial;

    List<GameObject> ShapeInstances = new List<GameObject>();

    int nextShapeIdx;

    void MakeGhosts()
    {
        Debug.Assert(ShapeGhosts == null);
        ShapeGhosts = new List<GameObject>(ShapePrefabs.Length);

        for(int i = 0; i < ShapePrefabs.Length; i++)
        {
            ShapeGhosts.Add(GameObject.Instantiate(ShapePrefabs[i]) as GameObject);

            // make it a ghost
            var meshRenderer = ShapeGhosts[i].GetComponent<MeshRenderer>();

            if(meshRenderer != null)
            {
                meshRenderer.material = GhostMaterial;
            }
            else
            {
                var spriteRenderer = ShapeGhosts[i].GetComponent<SpriteRenderer>();
                spriteRenderer.material = GhostMaterial;
            }

            ShapeGhosts[i].GetComponent<Collider2D>().enabled = false;

            // default to inactive
            ShapeGhosts[i].SetActive(false);
        }

        
    }

	// Use this for initialization
	void Start () {
	
        nextShapeIdx = Random.Range(0, ShapePrefabs.Length);
        MakeGhosts();
	}
	
	// Update is called once per frame
	void Update () {
	
          Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0.0f;

        if(Input.GetMouseButtonUp(0))
        {
            ShapeInstances.Add(GameObject.Instantiate(ShapePrefabs[nextShapeIdx], mouseWorld, ShapePrefabs[nextShapeIdx].transform.rotation) as GameObject);

            ShapeGhosts[nextShapeIdx].SetActive(false);

            nextShapeIdx = Random.Range(0, ShapePrefabs.Length);

        }

        ShapeGhosts[nextShapeIdx].SetActive(true);
        ShapeGhosts[nextShapeIdx].transform.position = mouseWorld;
        
	}

    public void ClearAllShapes()
    {
        for(int i = 0; i < ShapeInstances.Count; i++)
        {
            Destroy(ShapeInstances[i]);
        }

        ShapeInstances.Clear();
    }
}
