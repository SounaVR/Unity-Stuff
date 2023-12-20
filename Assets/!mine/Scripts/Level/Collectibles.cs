using UnityEngine;

public class Collectibles : MonoBehaviour
{
    public static Collectibles Instance;
    [SerializeField] InventoryManager.AllItems itemType;
    [SerializeField] private AudioSource lootSFX;

    private bool follow = false;
    [SerializeField] private Transform player;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Update() {
        if (follow) {
            float minimumDistance = 2f;
            if (Vector2.Distance(transform.position, player.position) > minimumDistance) {
                transform.position = Vector2.MoveTowards(transform.position, player.position, 50 * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!follow) {
                lootSFX.Play();
                InventoryManager.Instance.AddItem(itemType);

                if (itemType == InventoryManager.AllItems.Key) {
                    follow = true;
                    gameObject.GetComponent<CircleCollider2D>().enabled = false;
                } else {
                    DestroyFollowingKey();
                }
            }
        }
    }

    public void DestroyFollowingKey()
    {
        Destroy(gameObject);
    }
}
