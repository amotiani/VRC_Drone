using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class CreateInverted : EditorWindow
{
    #region Standard Fields
    //Object Seletion
    public int objectType = 0;
    string[] names = new string[] {"Select Object", "Cube", "Sphere", "Capsule", "Cylinder"};
    int[] sizes = new int[] {0, 1, 2, 3, 4};
    PrimitiveType objectPrimitive;

    //Object Size
    public float objectSize = 1;


    //Booleans
    public bool showBasicObject = false;
    public bool showCustomObject = false;
    public bool objectUndefinded = true;

    //String Messages
    string selectionBox = "No Selection found! Please select one of the fields above.";

    //String
    public string objectName;

    //Material
    Material objectMaterial;
    Material defaultMaterial;
    #endregion


    #region Custom Object Fields
    public float customObjectSize = 1;
    public Material customObjectMaterial;
    public GameObject customObject;
    #endregion



    [MenuItem("Window/Labgames/Tools/Inverted GameObjects")] //Creates an Menu item
    public static void OpenEditorWindow() //Opens Editor Window
    {
        EditorWindow.GetWindow(typeof(CreateInverted));
    }

    public void OnGUI() //Handlse the visuals on the Editor Window
    {
        EditorGUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Create Basic Object"))
        {
            showBasicObject = true;
            showCustomObject = false;
        }

        GUILayout.Space(15);
        if(GUILayout.Button("Create Custom Object"))
        {
            showBasicObject = false;
            showCustomObject = true;
        }
        
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(15);

        //Basic Object Elements
        if(showBasicObject == true && showCustomObject == false)
        {
            GUILayout.Box("Create Basic Object", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            objectType = EditorGUILayout.IntPopup("Object Type", objectType, names, sizes);
            EditorGUILayout.Space(5);

            objectSize = EditorGUILayout.FloatField("Object Size", objectSize);
            EditorGUILayout.Space(5);

            objectMaterial = (Material)EditorGUILayout.ObjectField("Object Material", objectMaterial, typeof(Material), true);
            EditorGUILayout.Space(5);

            if(GUILayout.Button("Create Object"))
            {
                GetObjectType(objectType);
                if(objectUndefinded != true)
                {
                    CreateBasicObject(objectPrimitive, objectSize, objectName, objectMaterial);
                }
                else
                {
                    Debug.LogError("No Object Type selected!");
                }
            }
        }



        //Custom Object Elements
        if(showCustomObject == true && showBasicObject == false)
        {
            GUILayout.Box("Create Custom Object", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            customObjectSize = EditorGUILayout.FloatField("Object Size", customObjectSize);
            EditorGUILayout.Space(5);

            customObject= (GameObject)EditorGUILayout.ObjectField("Object Mesh", customObject, typeof(GameObject), true);
            EditorGUILayout.Space(5);

            customObjectMaterial = (Material)EditorGUILayout.ObjectField("Object Material", customObjectMaterial, typeof(Material), true);
            EditorGUILayout.Space(5);

            if(GUILayout.Button("Create Object"))
            {
                if(customObject != null)
                {
                    CreateCustomObject(customObjectSize, customObjectMaterial, customObject);
                }
                else if(customObject == null)
                {
                    Debug.LogError("No custom GameObject selected! Please assign one.");
                }
            }
        }


        //Warning Box
        if(showBasicObject == false && showCustomObject == false)
        {
            EditorGUILayout.HelpBox(selectionBox, MessageType.Warning);
        }

        //Website Button
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("Support Website")) //Leads User to support Website
        {
            Application.OpenURL("https://labgamesstudio.com/");
        }
    }

    void GetObjectType(int objectType) //Sets the object type
    {
        if(objectType == 1)
        {
            objectPrimitive = PrimitiveType.Cube;
            objectUndefinded = false;
            objectName = "InvertedCube";
        }
        else if(objectType == 2)
        {
            objectPrimitive = PrimitiveType.Sphere;
            objectUndefinded = false;
            objectName = "InvertedSphere";
        }
        else if(objectType == 3)
        {
            objectPrimitive = PrimitiveType.Capsule;
            objectUndefinded = false;
            objectName = "InvertedCapsule";
        }
        else if(objectType == 4)
        {
            objectPrimitive = PrimitiveType.Cylinder;
            objectUndefinded = false;
            objectName = "InvertedCylinder";
        }
        else
        {
            objectUndefinded = true;
            objectName = "NONE";
        }
    }

    void LoadResources() //Loads the resources
    {
        defaultMaterial = (Material)Resources.Load("defaultMaterial");
    }

    void CreateBasicObject(PrimitiveType objectType, float objectSize, string name, Material objectMaterial) //Creates an basic GameObject
    {

        //Create the Object and get the mesh and the meshFilter
        GameObject basicObject = GameObject.CreatePrimitive(objectType);
        MeshFilter meshFilter = basicObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;

        //Destroy an not wanted Instance
        DestroyImmediate(basicObject);

        //Create the new Object
        GameObject newBasicObject = new GameObject();
        newBasicObject.name = name;
        MeshFilter newMeshFilter = newBasicObject.AddComponent<MeshFilter>();
        newMeshFilter.sharedMesh = new Mesh();

        //Scaling the new GameObject
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i] * objectSize;
        }
        newMeshFilter.sharedMesh.vertices = vertices;

        //Inverting the GameObject by reversing the triangles and the normals
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int t = triangles[i];
            triangles[i] = triangles[i+2];
            triangles[i+2] = t;
        }
        newMeshFilter.sharedMesh.triangles = triangles;

        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        newMeshFilter.sharedMesh.normals = normals;

        //Set the new UV´s and recalvulate the mesh bounds
        newMeshFilter.sharedMesh.uv = mesh.uv;
        newMeshFilter.sharedMesh.uv2 = mesh.uv2;
        newMeshFilter.sharedMesh.RecalculateBounds();

        //Create mesh renderer and add the transparent Material
        MeshRenderer meshRenderer = newBasicObject.AddComponent<MeshRenderer>();

        //Set the new Material
        if(newBasicObject != null)
        {
            if(objectMaterial != null)
            {
                meshRenderer.material = objectMaterial;
            }
            else
            {
                meshRenderer.material = defaultMaterial;
            }
        }
        
        //Set the new position for the object
        newBasicObject.transform.position = new Vector3(0, 0, 0);
    }

    void CreateCustomObject(float objectSize, Material objectMaterial, GameObject gameObject) //Creates an custom GameObject
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;

        string name;
        name = gameObject.name;

        GameObject newCustomObject = new GameObject(name);
        MeshFilter newMeshFilter = newCustomObject.AddComponent<MeshFilter>();
        newMeshFilter.sharedMesh = new Mesh();

        //Scaling the new GameObject
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i] * objectSize;
        }
        newMeshFilter.sharedMesh.vertices = vertices;

        //Inverting the GameObject by reversing the triangles and the normals
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int t = triangles[i];
            triangles[i] = triangles[i+2];
            triangles[i+2] = t;
        }
        newMeshFilter.sharedMesh.triangles = triangles;

        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        newMeshFilter.sharedMesh.normals = normals;

        //Set the new UV´s and recalculate the mesh bounds
        newMeshFilter.sharedMesh.uv = mesh.uv;
        newMeshFilter.sharedMesh.uv2 = mesh.uv2;
        newMeshFilter.sharedMesh.RecalculateBounds();

        //Create mesh renderer and add a Material
        MeshRenderer meshRenderer = newCustomObject.AddComponent<MeshRenderer>();

        //Set the new Material
        if(newCustomObject != null)
        {
            if(objectMaterial != null)
            {
                newCustomObject.GetComponent<Renderer>().material = objectMaterial;
            }
            else
            {
                newCustomObject.GetComponent<Renderer>().material = defaultMaterial;
            }
        }

        //Set Instance location
        newCustomObject.transform.position = new Vector3(0, 0, 0);

        //Set Instance rotation
        newCustomObject.transform.rotation = Quaternion.identity;
    }

    void Awake() //Loads the resources at opening of the Window
    {
        LoadResources();
    }
}
