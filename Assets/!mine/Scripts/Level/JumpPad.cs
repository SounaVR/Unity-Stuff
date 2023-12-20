using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private float bounce = 20f;
    [SerializeField] private AudioSource jumpSFX;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            anim.SetBool("Jump", true);
            jumpSFX.Play();
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
        }
    }
    
    private void BackToIdle()
    {
        anim.SetBool("Jump", false);
    }
}
