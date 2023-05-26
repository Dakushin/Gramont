using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Déformationmaillage : MonoBehaviour
{

    public GameObject PickObj;
    Mesh m;
    Vector3[] p_vertices;
    Dictionary<int, List<int>> les_po = new Dictionary<int, List<int>>();
   
    // Start is called before the first frame update
    void Start()
    {
        m = GetComponent<MeshFilter>().mesh;
        
        Vector3[] p_vertices = m.vertices;
        const float SEUIL_DISTANCE_VERTICES_SIMILAIRES = 0.01f;

        bool[] bool_vert = new bool[p_vertices.Length];

        for (int i = 0; i < bool_vert.Length; i++) bool_vert[i] = false;

        int indes_vert = 0;
        int nb_pickingObjects = 0;
        while(indes_vert < p_vertices.Length)
        {
            if(!bool_vert[indes_vert])
            {
                bool_vert[indes_vert] = true;
                GameObject po = Instantiate(PickObj);
                po.AddComponent<DragObject>();
                po.name = "po" + nb_pickingObjects;
                nb_pickingObjects++;
                po.transform.position = transform.TransformPoint(p_vertices[indes_vert]);
                les_po.Add(po.GetInstanceID(), new List<int> { indes_vert });

                for(int i = indes_vert; i < p_vertices.Length; i++)
                {
                    if(Vector3.Distance(p_vertices[i], p_vertices[indes_vert]) < SEUIL_DISTANCE_VERTICES_SIMILAIRES)
                    {
                        bool_vert[i] = true;
                        les_po[po.GetInstanceID()].Add(i);
                    }
                }
                
            }
            indes_vert++;
        }
    }

   
    // Update is called once per frame
    void Update()
    {
        GameObject[] lst = (GameObject[])FindObjectsOfType(typeof(GameObject));
        p_vertices = m.vertices;
        foreach (int i in les_po.Keys)
        {
            foreach (GameObject o in lst)
            {
                foreach (int x in les_po[i])
                {
                    if (o.GetInstanceID() == i)
                    {
                        p_vertices[x] = o.transform.position;
                        m.normals[x] = NormalAtVertex(x);
                    }
                }
            }
        }
        m.vertices = p_vertices;
    }

    private Vector3 NormalAtVertex(int indiceVertex)
    {
        Vector3 normal = Vector3.zero;
        Vector3 sommeNormal = Vector3.zero;
        int nb = 0;
        normal = m.normals[indiceVertex];
        for (int i = 0; i < m.triangles.Length; i+= 3)
        {
            if ((m.triangles[i] == indiceVertex) || (m.triangles[i + 1] == indiceVertex) || (m.triangles[i + 2] == indiceVertex))
            {
                sommeNormal = Vector3.Cross((p_vertices[m.triangles[i]] - p_vertices[m.triangles[i + 1]]), (p_vertices[m.triangles[i + 2]] - p_vertices[m.triangles[i]])).normalized;
                nb++;
            }
        }
        

        // à quels triangles appartient ce vertex  
        //  recherche dans la liste des triplets de sommets du tableau triangles
        // calculer puis normaliser la moyenne des normales des nb triangles concernés
        normal = sommeNormal / nb;
        return normal.normalized;
    }
}


