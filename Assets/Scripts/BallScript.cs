using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
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
        bool shouldDisappear = true;
        if ((CompareTag(SHOT2_TAG) && collision.CompareTag(SHOT2_TAG)) ||
            (CompareTag(SHOT1_TAG) && collision.CompareTag(SHOT1_TAG))) shouldDisappear = false;
        if (CompareTag(SHOT2_TAG) && collision.gameObject.CompareTag(PLAYER1_TAG))
        {
            Debug.Log("Shot terminating game");
            ReadyScript.gameRunner.TerminateGame(SnakePlayer.player2.gameObject, 1);
        }
        else if (CompareTag(SHOT1_TAG) && collision.gameObject.CompareTag(PLAYER2_TAG))
        {
            Debug.Log("Shot terminating game");
            ReadyScript.gameRunner.TerminateGame(SnakePlayer.player1.gameObject, 1);
        }
        //if shot your own head, dont set active to false- to avoid bugs
        if ((CompareTag(SHOT2_TAG) && collision.CompareTag(PLAYER2_TAG)) ||
            (CompareTag(SHOT1_TAG) && collision.CompareTag(PLAYER1_TAG)))
        {
            Debug.Log("Shot own Head");
            shouldDisappear = false;
        }
        else if ((CompareTag(SHOT2_TAG) && collision.CompareTag(PLAYER2BODY_TAG))
            || (CompareTag(SHOT1_TAG) && collision.CompareTag(PLAYER1BODY_TAG)))
        {
            // collision.gameObject.TryGetComponent(out Linkable linkable);
            // if (linkable.getLinkNum() > 3) linkable.WasShot();
            // else shouldDisappear = false;
            shouldDisappear = false;
        }
        Debug.Log(this.tag);
        Debug.Log(collision.tag);

        
        Debug.Log(shouldDisappear);
        
        
        if(shouldDisappear) gameObject.SetActive(false);
    }

    public bool GetIsShotActive() { return isActive; }


    void DeactivateGameObject()
    {
        Destroy(this.gameObject); 
    }
}