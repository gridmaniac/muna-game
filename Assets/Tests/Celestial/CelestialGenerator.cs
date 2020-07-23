using UnityEngine;
using System.Collections;
//using TreeEditor;

namespace Celestial
{
    public class CelestialGenerator : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateCelestial(GameObject obj, CelestialSeed seed)
        {
            obj.GetComponent<MeshFilter>().sharedMesh = GeneratePolygon(seed.resolution, seed.radius, seed.topoFreq, seed.topoAmp, seed.details);
        }

        public GameObject GenerateCelestial(CelestialSeed seed)
        {
            GameObject obj = new GameObject("Celestial " + Time.time);
            obj.AddComponent<MeshRenderer>();
            obj.AddComponent<PolygonCollider2D>();
            obj.AddComponent<MeshFilter>().sharedMesh = GeneratePolygon(seed.resolution, seed.radius, seed.topoFreq, seed.topoAmp, seed.details);
            return obj;
        }

        public void ClearCollider(GameObject obj)
        {
            PolygonCollider2D collider = obj.GetComponent<PolygonCollider2D>();
            collider = new PolygonCollider2D();
        }

        public void GenerateCollider(GameObject obj)
        {
            DestroyImmediate(obj.GetComponent<PolygonCollider2D>());
            PolygonCollider2D collider = obj.AddComponent<PolygonCollider2D>();
            Vector3[] vertices = obj.GetComponent<MeshFilter>().sharedMesh.vertices;


            int[] tris = obj.GetComponent<MeshFilter>().sharedMesh.triangles;

            Vector2[] points = new Vector2[tris.Length];


            /*for (var i = 1; i < vertices.Length; i++)
            {
                points[i].x = vertices[i].x;
                points[i].y = vertices[i].y;
            }*/
            int c = 0;
            for (var i = 0; i < tris.Length - 2; i += 3)
            {
                points[c] = vertices[tris[i]];
                points[c + 1] = vertices[tris[i + 1]];
                points[c + 2] = vertices[tris[i + 2]];
                //collider.SetPath(i / 3, points);
                c += 3;
            }

            collider.SetPath(0, points);


            //DestroyImmediate(obj.GetComponent<PolygonCollider2D>());
            //PolygonCollider2D collider = obj.AddComponent<PolygonCollider2D>();
            //collider = new PolygonCollider2D();
            //int[] tris = obj.GetComponent<MeshFilter>().sharedMesh.triangles;
            //Vector3[] vertices = obj.GetComponent<MeshFilter>().sharedMesh.vertices;

            //Vector2[] points = new Vector2[3];

            /*collider.pathCount = tris.Length;
            for (var i = 0; i < tris.Length - 2; i += 3)
            {
                points[0] = vertices[tris[i]];
                points[1] = vertices[tris[i + 1]];
                points[2] = vertices[tris[i + 2]];
                collider.SetPath(i / 3, points);
            }*/
        }



        private Mesh GeneratePolygon(int resolution, float radius, float topoFreq, float topoAmp, float details)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[resolution + 1];

            vertices[0] = Vector3.zero;

            float step = 360.0f / (float)resolution;
            float vX = 0.0f;
            float vY = 0.0f;

            var tx = Random.Range(0.0f, 10.0f);
            var ty = Random.Range(0.0f, 10.0f);

            var bx = Random.Range(0.0f, 10.0f);
            var by = Random.Range(0.0f, 10.0f);

            float bCoef = resolution / 512.0f;

            float sRad = 0.0f;
            for (var i = 0; i < resolution; i++)
            {
                sRad = radius + Mathf.PerlinNoise(bx / bCoef, by / bCoef) * topoAmp;
                vX = sRad * Mathf.Sin(i * step * Mathf.Deg2Rad) + vertices[0].x;
                vY = sRad * Mathf.Cos(i * step * Mathf.Deg2Rad) + vertices[0].y;

                bx += topoFreq;
                by += topoFreq;

                vX += Mathf.PerlinNoise(vX + tx, vY + tx) * details - details / 2;
                vY += Mathf.PerlinNoise(vX + ty, vY + ty) * details - details / 2;
                vertices[i + 1] = new Vector3(vX, vY, 0.0f);


            }
            mesh.vertices = vertices;

            int[] tri = new int[resolution * 3 + 3];

            int vC = 1;

            for (var i = 0; i < resolution * 3 - 3; i += 3)
            {
                tri[i] = 0;
                tri[i + 1] = vC;
                tri[i + 2] = vC + 1;
                vC++;
            }
            tri[resolution * 3] = 0;
            tri[resolution * 3 + 1] = vC;
            tri[resolution * 3 + 2] = 1;

            mesh.triangles = tri;

            return mesh;
        }
    }
}