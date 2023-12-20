using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 targetPosition;
    private float speed = 3f;

    private void Awake() {
        targetPosition = PlayerController.Instance.GetPlayerTransform();
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (transform.position == targetPosition) {
            Destroy(gameObject);
        }
    }
}