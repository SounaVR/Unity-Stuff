using UnityEngine;
using UnityEngine.Events;

public class LevelGeneration : MonoBehaviour
{
    public UnityEvent onGenerationDone;

    public Transform[] startingPositions;
    public GameObject[] rooms; // index 0 --> LR, index 1 --> LRB, index 2 --> LRT, index 3 --> LRTB

    private int direction;
    public float moveAmount;

    private float timeBetweenRoom;
    public float startTimeBetweenRoom = .25f;

    public float minX;
    public float maxX;
    public float minY;
    public bool stopGeneration;

    public LayerMask room;

    private int downCounter;
    
    private void Start()
    {
        int randStartingPos = Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randStartingPos].position;
        Instantiate(rooms[0], transform.position, Quaternion.identity);

        direction = Random.Range(1, 6);
    }

    private void Update()
    {
        if (timeBetweenRoom <= 0 && !stopGeneration) {
            Move();
            timeBetweenRoom = startTimeBetweenRoom;
            Debug.Log(direction);
        } else {
            timeBetweenRoom -= Time.deltaTime;
        }
    }

    private void Move()
    {   
        // Move RIGHT
        if (direction == 1 || direction == 2) 
        {
            if (transform.position.x < maxX) {
                downCounter = 0;

                transform.position = new Vector2(transform.position.x + moveAmount, transform.position.y);

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(1, 6);
                if (direction == 3) {
                    direction = 2;
                } else if (direction == 4) {
                    direction = 5;
                }
            } else direction = 5;
        }
        // Move LEFT 
        else if (direction == 3 || direction == 4)
        {
            if (transform.position.x > minX) {
                downCounter = 0;

                transform.position = new Vector2(transform.position.x - moveAmount, transform.position.y);

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(3, 6);
            } else direction = 5;
        }
        // Move Down
        else if (direction == 5)
        {
            downCounter++;

            if (transform.position.y > minY) {
                Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, room);
                int roomType = roomDetection.GetComponent<RoomType>().type;

                if (roomType != 1 && roomType != 3 ) {
                    if (downCounter >= 2) {
                        roomDetection.GetComponent<RoomType>().RoomDestruction();
                        Instantiate(rooms[3], transform.position, Quaternion.identity);
                    } else {
                        roomDetection.GetComponent<RoomType>().RoomDestruction();

                        int randBottomRoom = Random.Range(1, 4);
                        if (randBottomRoom == 2) {
                            randBottomRoom = 1;
                        }
                        Instantiate(rooms[randBottomRoom], transform.position, Quaternion.identity);
                    }   
                }

                transform.position = new Vector2(transform.position.x, transform.position.y - moveAmount);

                int rand = Random.Range(2, 3);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(1, 6);
            } else {
                stopGeneration = true;
                onGenerationDone.Invoke(); // Spawn the player when the level generation is done
            }
        }
    }
}
