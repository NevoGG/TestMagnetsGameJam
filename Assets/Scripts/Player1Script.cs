using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// using DG.Tweening;   TODO: HOW TO IMPORT WITHOUT DOWNLOADING AGAIN?

public class Player1Script : MonoBehaviour
{
    public int speed = 100;
    public TMP_Text p1scoreBoard;
    public TMP_Text p1Instructions;
    public TMP_Text p1Ready;
    public TMP_Text goMssg;
    public bool ready = false;
    public GameObject p2 = null;


    private Rigidbody2D player1RB = null;
    private int xVelocity = 0;
    private int yVelocity = 0;
    private int numPoints = 0;
    private bool playing = false;
    private float boardHight = -0.1f;
    private GameObject bite = null;
    private bool calledOnce = true;

    // Start is called before the first frame update
    void Start()
    {
        player1RB = gameObject.GetComponent<Rigidbody2D>();
        playing = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!playing)
        {
            if (Input.GetKey(KeyCode.RightShift))
            {
                p1Instructions.gameObject.SetActive(false);
                ready = true;
                p1Ready.gameObject.SetActive(true);
            }
            if (ready && p2.gameObject.GetComponent<Player2Script>().ready && calledOnce)
            {
                Debug.Log("Marked as Ready");
                calledOnce = false;
                StartCoroutine(StartGame());
            }
        }
        else
        {
            xVelocity = 0;
            yVelocity = 0;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                xVelocity = -speed;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                xVelocity = speed;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                yVelocity = speed;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                yVelocity = -speed;
            }
            player1RB.velocity = new Vector2(xVelocity, yVelocity);

            if (Input.GetKey(KeyCode.RightShift))
            {
                ShootBullet(Vector2.right);
            }


        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Player1 - Collided!");

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player1 - hit trigger");
        if (other.gameObject.CompareTag("Bite"))
        {
            numPoints += 1;
            p1scoreBoard.text = "Score: " + numPoints.ToString();
            Destroy(other.gameObject);

        }
    }

    IEnumerator RespawnBites()
    {
        Debug.Log("Called respawn");
        while (!playing) {}
        while (playing)
        {
            Debug.Log("Creating bite");
            float xPlacement = Random.Range(1, 450);
            float yPlacement = Random.Range(1, 250);
            if (Random.value < 0.5f)
            {
                xPlacement *= -1;
            }
            if (Random.value < 0.5f)
            {
                yPlacement *= -1;
            }
            bite = Instantiate(Resources.Load("Bite")) as GameObject;
            bite.transform.position = (new Vector3(xPlacement, yPlacement, boardHight));
            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator ShootBullet(Vector2 direction)
    {
        yield return null;
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        p1Ready.gameObject.SetActive(false);
        p2.gameObject.GetComponent<Player2Script>().p2Ready.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        goMssg.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        goMssg.gameObject.SetActive(false);
        playing = true;
        p2.gameObject.GetComponent<Player2Script>().playing = true;
        StartCoroutine(RespawnBites());

    }
}
