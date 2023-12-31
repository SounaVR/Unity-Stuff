using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = .25f;
    private Vector3 velocity = Vector3.zero;
    private Transform target;

    private void Update()
    {
        target = PlayerController.Instance.GetPlayerTransform().transform;
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
