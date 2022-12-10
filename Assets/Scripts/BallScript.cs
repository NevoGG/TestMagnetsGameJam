using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public float speed = 15f;
    public float deactivateTimer = 1f;
    private Vector3 direction;
    
    //to avoid collision on shot instantiation, gives it a period of time until active
    private bool isActive;
    private float countUntilActive =0f;
    [SerializeField] float timeUntilActive = 0.01f;

    private static string PLAYER1_TAG = "Player1";
    private static string PLAYER2_TAG = "Player2";
    private static string PLAYER1BODY_TAG = "Player1Body";
    private static string PLAYER2BODY_TAG = "Player2Body";
    private const string SHOT1_TAG = "Shot1";
    private const string SHOT2_TAG = "Shot2";

    // Start is called before the first frame update
    void Start()
    {
        
        Vector3 dir;
        if (CompareTag(SHOT1_TAG)) direction = SnakePlayer.player1.saveDir;
        else if (CompareTag(SHOT2_TAG)) direction = SnakePlayer.player2.saveDir;
        Invoke("DeactivateGameObject", deactivateTimer);
    }

    
    // Update is called once per frame
    void Update()
    {
        Debug.Log(isActive);
        if (!isActive)
        {
            countUntilActive += Time.deltaTime;
            if (countUntilActive >= timeUntilActive)
            {
                isActive = true;
            }
        }
        Move();
    }

    void Move()
    {
        Vector3 temp = transform.position;
        if (direction.x > 0 ) temp.x += speed * Time.deltaTime;
        else if (direction.x < 0) temp.x -= speed * Time.deltaTime;
        else if (direction.y > 0) temp.y += speed * Time.deltaTime;
        else if (direction.y < 0) temp.y -= speed * Time.deltaTime;
        
        
        transform.position = temp;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareTag(SHOT2_TAG) && collision.gameObject.CompareTag(PLAYER1_TAG))
        {
            SnakePlayer.player1.TerminateGame(PLAYER2_TAG);
        }
        else if (CompareTag(SHOT1_TAG) && collision.gameObject.CompareTag(PLAYER2_TAG))
        {
            SnakePlayer.player1.TerminateGame(PLAYER1_TAG);
        }
        //if shot your own head, dont set active to false- to avoid bugs
        if (CompareTag(SHOT2_TAG) && collision.gameObject.CompareTag(PLAYER2_TAG) ||
            CompareTag(SHOT1_TAG) && collision.gameObject.CompareTag(PLAYER1_TAG)) return;
        
        gameObject.SetActive(false);
    }

    public bool GetIsShotActive() { return isActive; }


    void DeactivateGameObject()
    {
        Destroy(this.gameObject); 
    }
}