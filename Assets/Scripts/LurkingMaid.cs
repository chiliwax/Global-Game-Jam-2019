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
    private Vector3 oldPos;
    private Vector3 oldDir;

    private void Awake()
    {
        sightRange = GetComponent<CircleCollider2D>() as CircleCollider2D;
        sightRange.isTrigger = true;
        playerInRange = false;
        oldDir = -transform.up;
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
        float angleToPlayer = Vector3.Angle(oldDir * sightMaxDistance, playerPos - transform.position);

        //  Is player in sight ?
        if (playerInRange && angleToPlayer < fieldOfView / 2)
        {
            Vector2 toPlayer = new Vector2(playerPos.x - transform.position.x, playerPos.y - transform.position.y);

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, toPlayer);
            int index = 0;
            if (hits.Length != 0)
                while (index < hits.Length && hits[index].transform.tag == tag)
                    ++index;
            Debug.DrawRay(transform.position, hits[index].transform.position - transform.position, Color.red);

            // Is player uncovered ?
            if (hits[index].transform.tag == "Player")
            {
                Debug.Log("I see you");
                hits[index].transform.GetComponent<SpriteRenderer>().color = Color.red;
                hits[index].transform.GetComponent<Player>().Seen(gameObject);
                return ;
            }
        }
        if (player.GetComponent<Player>().seenBy == gameObject)
            player.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void drawSightCone()
    {
        // Compute direction
        Vector2 direction = new Vector2(transform.position.x - oldPos.x, transform.position.y - oldPos.y);
        if (direction.magnitude != 0)
            direction = direction / direction.magnitude;
        else
            direction = oldDir;
        oldPos = transform.position;
        oldDir = direction;

        // Place points for FOV drawin
        var points = new List<Vector2>();
        Vector3 localPosition = transform.InverseTransformPoint(transform.position);
        points.Add(new Vector2(localPosition.x, localPosition.y));
        for (float i = -fieldOfView / 2; i < fieldOfView / 2; ++i)
        {
            Vector3 worldAngle = Quaternion.AngleAxis(i, transform.forward) * direction;
            Vector3 localAngle = Quaternion.AngleAxis(i, transform.forward) * direction;//transform.up;

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, localAngle * sightMaxDistance, sightMaxDistance);
            Vector2 point;
            int index = 0;
            if (hits.Length != 0)
                while (index < hits.Length && hits[index].transform.tag == tag)
                    ++index;
 
            if (hits.Length > index)
            {
                Vector2 toPoint = hits[index].point - (Vector2)transform.position;
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

        Mesh msh = new Mesh
        {
            vertices = vertices,
            triangles = indices
        };
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        MeshFilter filter = GetComponentInChildren<MeshFilter>() as MeshFilter;
        filter.mesh = msh;
    }
}
