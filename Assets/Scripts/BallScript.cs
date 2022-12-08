using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public float speed = 15f;
    public float deactivateTimer = 1f;
    private Vector3 direction;
    

    // Start is called before the first frame update
    void Start()
    {
        if (this.tag == "Shot1")
        {
            direction = SnakePlayer.player1.saveDir;
            
        }
        else if (this.tag == "Shot2")
        {
            direction = SnakePlayer.player2.saveDir;
        }
               
        Invoke("DeactivateGameObject", deactivateTimer);
    }

    // Update is called once per frame
    void Update()
    {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.tag == "Shot2" && collision.gameObject.CompareTag("Player1"))
        {
            SnakePlayer.player1.TerminateGame("Player2");
        }
        else if (this.tag == "Shot1" && collision.gameObject.CompareTag("Player2"))
        {
            SnakePlayer.player1.TerminateGame("Player1");
        }
    }


    void DeactivateGameObject()
    {
        Destroy(this.gameObject); 
    }
}