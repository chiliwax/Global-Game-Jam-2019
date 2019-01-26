using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class LurkingMaid : MonoBehaviour
{
    [Range(0.0f, 180.0f)] public float fieldOfView = 70.0f;
    [Range(0.0f, 10.0f)] public float sightMaxDistance = 5.0f;

    [SerializeField] private GameObject player = null;

    private CircleCollider2D sightRange;
    private Vector3 playerPos;
    private bool playerInRange;

    private void Awake()
    {
        sightRange = GetComponent<CircleCollider2D>() as CircleCollider2D;
        playerInRange = false;
    }

    private void Update()
    {
        sightRange.radius = sightMaxDistance;
        searchPlayer();
        drawSightCone(); 
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Someone here ?");
            playerPos = other.gameObject.transform.position;
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Probably a rat");
            playerInRange = false;
        }
    }

    private void searchPlayer()
    {
        float angleToPlayer = Vector3.Angle(transform.up * sightMaxDistance, playerPos - transform.position);

        //  Is player in sight ?
        if (playerInRange && angleToPlayer < fieldOfView / 2)
        {
            Vector2 toPlayer = new Vector2(playerPos.x - transform.position.x, playerPos.y - transform.position.y);

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, toPlayer);
            Debug.DrawRay(transform.position, hits[1].transform.position - transform.position, Color.red);

            // Is player uncovered ?
            if (hits[1].transform.tag == "Player")
            {
                Debug.Log("I see you");
                hits[1].transform.GetComponent<SpriteRenderer>().color = Color.red;
                return ;
            }
        }

        player.GetComponent<SpriteRenderer>().color = Color.green;
    }

    private void drawSightCone()
    {
        // Draw FOV in Scene View
        Vector3 begFOV = Quaternion.AngleAxis(fieldOfView / 2, Vector3.forward) * transform.up;
        Vector3 endFOV = Quaternion.AngleAxis(-fieldOfView / 2, Vector3.forward) * transform.up;
        Debug.DrawLine(transform.position, begFOV * sightMaxDistance, Color.red);
        Debug.DrawLine(transform.position, endFOV * sightMaxDistance, Color.red);

        // Place points for FOV drawin
        var points = new List<Vector2>();
        points.Add(new Vector2(transform.position.x, transform.position.y));
        for (float i = -fieldOfView / 2; i < fieldOfView / 2; ++i)
        {
            Vector3 worldAngle = Quaternion.AngleAxis(i, transform.forward) * Vector3.up;
            Vector3 localAngle = Quaternion.AngleAxis(i, transform.forward) * transform.up;

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, localAngle * sightMaxDistance);

            Vector2 point;
            if (hits.Length > 1)
            {
                Vector2 toPoint = hits[1].point - (Vector2)transform.position;
                float distanceToPoint = toPoint.magnitude;
                Debug.DrawRay(transform.position, localAngle * distanceToPoint, Color.green);
                point = (Vector2)worldAngle * distanceToPoint;
            }
            else
            {
                Debug.DrawRay(transform.position, localAngle * sightMaxDistance, Color.green);
                point = (Vector2)worldAngle * sightMaxDistance;
            }
            points.Add(point);
        }
   
        // Draw FOV
        Vector2[] vertices2D = points.ToArray();
        Vector3[] vertices = new Vector3[vertices2D.Length];

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);

        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        MeshFilter filter = GetComponentInChildren<MeshFilter>() as MeshFilter;
        filter.mesh = msh;
    }
}
