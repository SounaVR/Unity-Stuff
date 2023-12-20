using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform target;
    [SerializeField] private float retreatDistance;
    [SerializeField] private float followMinimumDistance;

    [SerializeField] GameObject projectile;
    [SerializeField] private float timeBetweenShots;
    private float nextShotTime;

    private void Update()
    {
        if (Time.time > nextShotTime) {
            Instantiate(projectile, transform.position, Quaternion.identity);
            nextShotTime = Time.time + timeBetweenShots;
        }
        if (Vector2.Distance(transform.position, target.position) < retreatDistance) {
            transform.position = Vector2.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);
        }
        if (Vector2.Distance(transform.position, target.position) > followMinimumDistance) {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}
