using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public static PlayerDeath Instance;

    public bool IsDead = false;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [SerializeField] private AudioSource deathSFX;

    private void Awake()
    {
        Instance = this;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap")) {
            Die();
        }
    }

    public bool Die()
    {
        deathSFX.Play();
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
        IsDead = true;

        return IsDead;
    }
}
