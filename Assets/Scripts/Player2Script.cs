using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// using DG.Tweening;   TODO: HOW TO IMPORT WITHOUT DOWNLOADING AGAIN?

public class Player2Script : MonoBehaviour
{

    public int speed = 100;
    public TMP_Text p2scoreBoard;
    public TMP_Text p2Instructions;
    public TMP_Text p2Ready;
    public bool ready = false;
    public bool playing = false;


    private Rigidbody2D player2RB = null;
    private int xVelocity = 0;
    private int yVelocity = 0;
    private int numPoints = 0;


    // Start is called before the first frame update
    void Start()
    {
        player2RB = gameObject.GetComponent<Rigidbody2D>();



    }

    // Update is called once per frame
    void Update()
    {
        if (!playing)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                p2Instructions.gameObject.SetActive(false);
                ready = true;
                p2Ready.gameObject.SetActive(true);
            }
        }
        else
        {
            xVelocity = 0;
            yVelocity = 0;
            if (Input.GetKey(KeyCode.A))
            {
                xVelocity = -speed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                xVelocity = speed;
            }
            if (Input.GetKey(KeyCode.W))
            {
                yVelocity = speed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                yVelocity = -speed;
            }
            player2RB.velocity = new Vector2(xVelocity, yVelocity);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                ShootBullet(Vector2.right);
            }

        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Player2 - Collided!");

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player2 - hit trigger");
        if (other.gameObject.CompareTag("Bite"))
        {
            numPoints += 1;
            p2scoreBoard.text = "Score: " + numPoints.ToString();
            Destroy(other.gameObject);
        }
    }


    IEnumerator ShootBullet(Vector2 direction)
    {
        yield return null;
    }


}
