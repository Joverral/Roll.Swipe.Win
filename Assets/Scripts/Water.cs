using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//using UnityEditor;

public class Water : MonoBehaviour {

    //Our renderer that'll make the top of the water visible
    LineRenderer WaterLine;

    [SerializeField]
    bool UseHighQualityWater = false;

    [SerializeField]
    EdgeCollider2D WaterEdge;
    Vector2[] edgePoints;

    //Our physics arrays
    float[] velocities;
    float[] accelerations;
    float[] leftDeltas;
    float[] rightDeltas;

    //Our meshes and colliders
    GameObject[] meshobjects;
    Mesh[] meshes;

    //Our particle system
    public GameObject splash;

    //The material we're using for the top of the water
    public Material mat;

    //The GameObject we're using for a mesh
    public GameObject watermesh;

    //All our constants
    //const float springconstant = 0.02f;
    //const float damping = 0.04f;
    //const float spread = 0.05f;

    [SerializeField]
    float springconstant = 0.02f;
    [SerializeField]
    float damping = 0.04f;
    [SerializeField]
    float spread = 0.05f;

    //The properties of our water  (WTF, why aren't these public?)
    float baseheight;
    float left;
    float bottom;

    Vector2 bottomLeft;
    Vector2 bottomRight;

    int screenNodeCount; //


    // we don't need as fine of granularity for collision as we do rendering
    int collisionPolygonPointRatio = 10;

    bool waterReady = false;

    [SerializeField]
    float TimeBetweenWaves = 0.5f;
    [SerializeField]
    float WaveStrength = 10.0f;


    IEnumerator AddWaveCoRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeBetweenWaves);
            // TODO:  Think about choosing an index for more of a sine wave type feel
            int randomIndex = Random.Range(0, edgePoints.Length);
            velocities[randomIndex] += WaveStrength;
        }
    }

    IEnumerator SpawnWaterCoRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        var temp = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var temp2 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

        //this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 3));

        // Spawning our water
        //SpawnWater(temp.x, temp2.x - temp.x, 0, - (temp2.y / 2.0f));
        SpawnWater(temp.x - 100.0f, 1000, 0, -(temp2.y / 2.0f));
        waterReady = true;

        StartCoroutine(AddWaveCoRoutine());
    }

    void Start()
    {
        StartCoroutine(SpawnWaterCoRoutine());

        //var temp = Camera.main.ScreenToWorldPoint(Vector3.zero);
        //var temp2 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

        ////this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 3));

        //// Spawning our water
        ////SpawnWater(temp.x, temp2.x - temp.x, 0, - (temp2.y / 2.0f));
        //SpawnWater(temp.x - 100.0f, 1000, 0, -(temp2.y / 2.0f));
        //waterReady = true;
    }


    public int WorldXToIndex(float worldX)
    {
        float xpos = worldX; // transform.InverseTransformPoint(new Vector3(worldX, 0.0f)).x;
        //If the position is within the bounds of the water:
        if (xpos >= edgePoints[0].x && xpos <= edgePoints[edgePoints.Length - 1].x)
        {
            //Offset the x position to be the distance from the left side
            xpos -= edgePoints[0].x;

            //Find which spring we're touching
            return Mathf.RoundToInt((edgePoints.Length - 1) * (xpos / (edgePoints[edgePoints.Length - 1].x - edgePoints[0].x)));
        }

        return -1;
    }

    public void Splash(float xpos, float velocity)
    {
        if (!waterReady)
            return;

        //  xpos = this.transform.InverseTransformPoint(new Vector3(xpos, 0.0f)).x;

        int index = WorldXToIndex(xpos);

        if(index == -1)
        {
            return;
        }
            
        //If the position is within the bounds of the water:
      

        //Add the velocity of the falling object to the spring
        velocities[index] += velocity;

        //Set the lifetime of the particle system.
        float lifetime = 0.93f + Mathf.Abs(velocity)*0.07f;

        //Set the splash to be between two values in Shuriken by setting it twice.
        splash.GetComponent<ParticleSystem>().startSpeed = 8 + 2 * Mathf.Pow(Mathf.Abs(velocity),0.5f);
        splash.GetComponent<ParticleSystem>().startSpeed = 9 + 2 * Mathf.Pow(Mathf.Abs(velocity), 0.5f);
        splash.GetComponent<ParticleSystem>().startLifetime = lifetime;

        //Set the correct position of the particle system.
        Vector3 position = new Vector3(edgePoints[index].x, edgePoints[index].y - 0.35f, transform.position.z);

        //This line aims the splash towards the middle. Only use for small bodies of water:
        //Quaternion rotation = Quaternion.LookRotation(new Vector3(xpositions[Mathf.FloorToInt(edgePoints.Length / 2)], baseheight + 8, 5) - position);

        //Create the splash and tell it to destroy itself.
        GameObject splish = GameObjectPooler.Current.GetObject(splash);
        splish.transform.position = position;
        splish.transform.rotation = Quaternion.Euler(0, 0, 180.0f);

        GameObjectPooler.Current.PoolObject(splish, lifetime + 0.3f);
    }

    const int numOffScreenNodes = 5;
    const float magicResolutionMultiplier = 1.5f;
    public void SpawnWater(float Left, float Width, float Top, float Bottom)
    {
        var boxCollider = gameObject.GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(Width, boxCollider.size.y);
        //WaterEdge = gameObject.GetComponent<PolygonCollider2D>();

        // For the Buoyancy trigger/collider
        bottomLeft = new Vector2(Left, (Top + Bottom)); //  + (Vector2)this.transform.position;
        bottomRight = new Vector2(Left + Width, (Top + Bottom)); //  + (Vector2)this.transform.position;

     //   WaterEdge.offset = new Vector2(Left + Width / 2, (Top + Bottom) / 2);

        //Calculating the number of edges and nodes we have
        int edgecount = Mathf.RoundToInt(Width* magicResolutionMultiplier);
        int nodecount = edgecount + 1;
        var temp = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var temp2 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        screenNodeCount = Mathf.RoundToInt( (temp2.x - temp.x) * magicResolutionMultiplier)  + numOffScreenNodes * 2; // + is for slightly offscreen nodes for better smoothness

        //Add our line renderer and set it up:
        // TODO WTF, why is this dynamically generated?  Nothing in it is required
        WaterLine =  gameObject.GetComponent<LineRenderer>();
        WaterLine.material.renderQueue = 4000; //not sure why this is here

        
        WaterLine.SetVertexCount(screenNodeCount);
       
        //WaterLine.SetVertexCount(nodecount);
        //  
        WaterLine.sortingLayerName = "Foreground";
        WaterLine.sortingOrder = 999;
        //WaterEdge.offset =- new Vector2(Left + Width / 2, (Top + Bottom) / 2);

        //Declare our physics arrays
        edgePoints = new Vector2[nodecount];
        velocities = new float[nodecount];
        accelerations = new float[nodecount];

        leftDeltas = new float[edgePoints.Length];
        rightDeltas = new float[edgePoints.Length];

        //Declare our mesh arrays
        meshobjects = new GameObject[edgecount];
        meshes = new Mesh[screenNodeCount - 1];

        //Set our variables
        baseheight = Top;
        bottom = Bottom;
        left = Left;

        //For each node, set the line renderer and our physics arrays
        for (int i = 0; i < nodecount; i++)
        {
            edgePoints[i] = new Vector2(Left + Width * i / edgecount, Top);
            accelerations[i] = 0;
            velocities[i] = 0;
        }

        //Setting the meshes now:
        for (int i = 0; i < screenNodeCount - 1; i++)
        {
            //Make the mesh
            meshes[i] = new Mesh();

            //Create the corners of the mesh
            Vector3[] Vertices = new Vector3[4];
            Vertices[0] = edgePoints[i];
            Vertices[1] = edgePoints[i + 1];
            Vertices[2] = new Vector3(edgePoints[i].x, bottom, 0);
            Vertices[3] = new Vector3(edgePoints[i + 1].x, bottom, 0);

            //Set the UVs of the texture
            Vector2[] UVs = new Vector2[4];
            UVs[0] = new Vector2(0, 1);
            UVs[1] = new Vector2(1, 1);
            UVs[2] = new Vector2(0, 0);
            UVs[3] = new Vector2(1, 0);

            //Set where the triangles should be.
            int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };

            //Add all this data to the mesh.
            meshes[i].vertices = Vertices;
            meshes[i].uv = UVs;
            meshes[i].triangles = tris;

            //Create a holder for the mesh, set it to be the manager's child
            meshobjects[i] = Instantiate(watermesh, Vector3.zero, Quaternion.identity) as GameObject;
            meshobjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
            meshobjects[i].GetComponent<MeshRenderer>().sortingLayerName = "Foreground";
            meshobjects[i].GetComponent<MeshRenderer>().sortingOrder = 1000;

            meshobjects[i].transform.parent = transform;

            meshobjects[i].transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    //Same as the code from in the meshes before, set the new mesh positions
    Vector3[] ScratchVertices = new Vector3[4];
    void UpdateMeshes(int startIndex, int endIndex)
    {
        const float z = 0.0f;
        int q = 0;

        endIndex = Mathf.Min(endIndex, startIndex + meshes.Length);

        for (int i = startIndex; i < endIndex - 1; i++)
        {
            // Note: I really should transform the position, but that's overly expensive
            ScratchVertices[0] = new Vector3(edgePoints[i].x, edgePoints[i].y + this.transform.position.y, z);
            ScratchVertices[1] = new Vector3(edgePoints[i + 1].x, edgePoints[i + 1].y + this.transform.position.y, z);
            ScratchVertices[2] = new Vector3(edgePoints[i].x, bottom + this.transform.position.y, z);
            ScratchVertices[3] = new Vector3(edgePoints[i + 1].x, bottom + this.transform.position.y, z);
           

            meshes[q].vertices = ScratchVertices;
            meshes[q++].RecalculateBounds();
        }
    }

    void Update()
    {
        if (!waterReady)
            return;

        // Determine what water is on screen, only update that
        var upperCorner = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var lowerCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        int startIndex = Mathf.Max(WorldXToIndex(upperCorner.x) - numOffScreenNodes, 0);
        int endIndex = Mathf.Min(WorldXToIndex(lowerCorner.x) + numOffScreenNodes, edgePoints.Length);

        int q = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            float force = springconstant * (edgePoints[i].y - baseheight) + velocities[i] * damping;
            accelerations[i] = -force;
            edgePoints[i].y += velocities[i];
            velocities[i] += accelerations[i];

            if (q < screenNodeCount)
            {
                Vector3 newPos = new Vector3(edgePoints[i].x, edgePoints[i].y, 0.0f) + this.transform.position;
                WaterLine.SetPosition(q++, newPos);
                
            }
        }

        UpdateMeshes(startIndex, endIndex);
    }

    void FixedUpdate()
    {
        if (!waterReady)
            return;


        //Here we use the Euler method to handle all the physics of our springs:
        //We make 8 small passes for fluidity:
        for (int j = 0; j < 8; j++)
        {
            rightDeltas[0] = spread * (edgePoints[0].y - edgePoints[1].y);
            velocities[1] += rightDeltas[0];
            edgePoints[1].y += rightDeltas[0];

            for (int i = 1; i < (edgePoints.Length - 1); i++)
            {

                // We check the heights of the nearby nodes, adjust velocities accordingly, record the height differences
                leftDeltas[i] = spread * (edgePoints[i].y - edgePoints[i - 1].y);
                velocities[i - 1] += leftDeltas[i];

                rightDeltas[i] = spread * (edgePoints[i].y - edgePoints[i + 1].y);
                velocities[i + 1] += rightDeltas[i];

                edgePoints[i - 1].y += leftDeltas[i];
            }

            leftDeltas[edgePoints.Length - 1] = spread * (edgePoints[edgePoints.Length - 1].y - edgePoints[edgePoints.Length - 1 - 1].y);
            velocities[edgePoints.Length - 1 - 1] += leftDeltas[edgePoints.Length - 1];
            edgePoints[edgePoints.Length - 1 - 1].y += leftDeltas[edgePoints.Length - 1];
        }

        if(UseHighQualityWater)
        {
            WaterEdge.points = edgePoints;
        }
        
    }
}

//[CustomEditor(typeof(MeshRenderer))]
//public class MeshRendererSortingLayersEditor : Editor
//{

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        MeshRenderer renderer = target as MeshRenderer;

//        EditorGUILayout.BeginHorizontal();
//        EditorGUI.BeginChangeCheck();
//        string name = EditorGUILayout.TextField("Sorting Layer Name", renderer.sortingLayerName);
//        if (EditorGUI.EndChangeCheck())
//        {
//            renderer.sortingLayerName = name;
//        }
//        EditorGUILayout.EndHorizontal();

//        EditorGUILayout.BeginHorizontal();
//        EditorGUI.BeginChangeCheck();
//        int order = EditorGUILayout.IntField("Sorting Order", renderer.sortingOrder);
//        if (EditorGUI.EndChangeCheck())
//        {
//            renderer.sortingOrder = order;
//        }
//        EditorGUILayout.EndHorizontal();

//    }
//}