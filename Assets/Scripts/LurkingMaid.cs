using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class LurkingMaid : MonoBehaviour
{
    [Range(0.0f, 180.0f)] public float fieldOfView = 70.0f;
    [Range(0.0f, 10.0f)] public float viewDistance = 5.0f;

    [SerializeField] private GameObject player = null;

    private CircleCollider2D cc;
    private Vector3 playerPos;
    private bool playerInRange;

    private void Start()
    {
        cc = GetComponent<CircleCollider2D>() as CircleCollider2D;
        playerInRange = false;

        // Create Vector2 vertices
        Vector2[] vertices2D = new Vector2[] {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,0),
        };
    }

    private void Update()
    {
        cc.radius = viewDistance;
        Vector3 begFov = Quaternion.AngleAxis(fieldOfView / 2, Vector3.forward) * transform.up;
        Vector3 endFov = Quaternion.AngleAxis(-fieldOfView / 2, Vector3.forward) * transform.up;
        Debug.DrawLine(transform.position,  begFov * viewDistance, Color.red);
        Debug.DrawLine(transform.position, endFov * viewDistance, Color.red);
        if (playerInRange &&
            Vector3.Angle(transform.up * viewDistance, playerPos - transform.position) < fieldOfView / 2 )
        {
            Vector2 vec = new Vector2(playerPos.x - transform.position.x, playerPos.y - transform.position.y);
            Vector2 dir = vec / vec.magnitude;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, vec);
            Debug.DrawRay(transform.position, hits[1].transform.position - transform.position, Color.red);
            if (hits[1].transform.tag == "Player")
            {
                hits[1].transform.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                Debug.Log("See you not");
                player.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
        else
        {
            Debug.Log("See you not");
            player.GetComponent<SpriteRenderer>().color = Color.green;
        }
        drawCone(); 
    }
    
    private void drawCone()
    {
        var points = new List<Vector2>();
        for (float i = -fieldOfView / 2; i < fieldOfView / 2; ++i)
        {
            Vector3 angle = Quaternion.AngleAxis(i, transform.forward) * Vector3.up;
            Vector3 angleRay = Quaternion.AngleAxis(i, transform.forward) * transform.up;
            //Debug.DrawRay(transform.position, angle * viewDistance, Color.green);
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, angleRay * viewDistance);
            Vector2 point;
            if (hits.Length > 1)
            {
                Vector2 vec = hits[1].point - (Vector2)transform.position;
                float distance = vec.magnitude;
                Debug.DrawRay(transform.position, angleRay * distance, Color.green);
                point = (Vector2)angle * distance;// + Vector2.up * .5f;//hits[1].point + Vector2.up * .5f;
            }
            else
            {
                Debug.DrawRay(transform.position, angleRay * viewDistance, Color.green);
                point = (Vector2)angle * viewDistance;
            }
            points.Add(point);
        }

        points.Add(new Vector2(transform.position.x, transform.position.y));
        Vector2[] vertices2D = points.ToArray();
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        MeshFilter filter = GetComponentInChildren<MeshFilter>() as MeshFilter;
        filter.mesh = msh;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("found you");
            playerPos = other.gameObject.transform.position;
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("where you at little niggaz");
            playerInRange = false;
        }
    }
}
