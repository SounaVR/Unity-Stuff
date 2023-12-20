using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = .5f;

    private Collider2D coll;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpModifier = .3f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private Rigidbody2D targetrb;

    private Path path;
    private int currentWaypoint = 0;
    private Animator anim;

    Seeker seeker;
    Rigidbody2D rb;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled) {
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone()) {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null) return;

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count) return;

        // See if colliding with anything
        RaycastHit2D isGrounded = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0, Vector2.down, .1f, jumpableGround);

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        // Vector2 force = speed * Time.deltaTime * direction;

        // Jump
        if (jumpEnabled && isGrounded) {
            if (target.position.y -1f > rb.transform.position.y && targetrb.velocity.y == 0 && path.path.Count < 20) {
                rb.AddForce(jumpModifier * speed * Vector2.up);
            }
        }

        // Movement
        rb.AddForce(Vector2.right * direction, ForceMode2D.Impulse);

        if (rb.velocity.x > speed) {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        } else if (rb.velocity.x < speed * (-1)) {
            rb.velocity = new Vector2(speed * (-1), rb.velocity.y);
        }

        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) currentWaypoint++;

        // Direction Graphics Handling
        if (directionLookEnabled) {
            if (rb.velocity.x >= .1f) {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            } else if (rb.velocity.x <= -.1f) {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }
}
