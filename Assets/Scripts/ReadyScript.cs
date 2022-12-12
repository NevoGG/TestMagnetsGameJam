using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ReadyScript : MonoBehaviour
{
    public static ReadyScript gameRunner;

    // SCREENS
    public GameObject rNoneScreen = null;
    public GameObject rplayer1Screen = null;
    public GameObject rplayer2Screen = null;
    public GameObject rBothScreen = null;
    public GameObject Countdown = null;


    public GameObject doneTieScreen = null;
    public GameObject winP1CrashScreen = null;
    public GameObject winP1hitScreen = null;
    public GameObject winP2CrashScreen = null;
    public GameObject winP2hitScreen = null;

    public GameObject replayScreen = null;

    public GameObject player1 = null;
    public GameObject player2 = null;

    // GENERAL PRIVATE VARS

    private List<GameObject> bites;

    private float maxX = 440;
    private float maxY = 220;
    private GameObject bite = null;


    private bool p1Ready;
    private bool p2Ready;
    private bool playing;
    private bool showingWinner;
    private bool firstTimePlaying;
    private GameObject curScreen = null;

    // Start is called before the first frame update
    void Start()
    {
        gameRunner = this;
        
        rNoneScreen.GetComponent<SpriteRenderer>().enabled = true;
        p1Ready = false;
        p2Ready = false;
        playing = false;
        showingWinner = false;
        firstTimePlaying = true;
        bites = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playing  && !showingWinner)
        {
            Debug.Log("Waiting to start");
            if (Input.GetKey(KeyCode.RightShift))
            {
                if (!p2Ready)
                {
                    p2Ready = true;
                    if (p1Ready)
                    {
                        rplayer1Screen.GetComponent<SpriteRenderer>().enabled = false;
                        rBothScreen.GetComponent<SpriteRenderer>().enabled = true;
                        StartCoroutine(Wait2Start());
                    }
                    else
                    {
                        rNoneScreen.GetComponent<SpriteRenderer>().enabled = false;
                        rplayer2Screen.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!p1Ready)
                {
                    p1Ready = true;
                    if (p2Ready)
                    {
                        rplayer2Screen.GetComponent<SpriteRenderer>().enabled = false;
                        rBothScreen.GetComponent<SpriteRenderer>().enabled = true;
                        StartCoroutine(Wait2Start());
                    }
                    else
                    {
                        rNoneScreen.GetComponent<SpriteRenderer>().enabled = false;
                        rplayer1Screen.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
        }   
        else if(showingWinner)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                showingWinner = false;
                StopCoroutine("FlashReplayScreen");
                curScreen.GetComponent<SpriteRenderer>().enabled = false;
                rNoneScreen.GetComponent<SpriteRenderer>().enabled = true;
            }
            else if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("WelcomeScene");
            }
        }
    }

    IEnumerator Wait2Start()  // only player 1 can call this
    {
        yield return new WaitForSeconds(0.5f);
        rBothScreen.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Animator>().enabled = true;
        Countdown.SetActive(true);
        
        yield return new WaitForSeconds(3.9f);
        Countdown.SetActive(false);
        gameObject.GetComponent<Animator>().enabled = false;


        playing = true;
        player1.SetActive(true);
        player2.SetActive(true);
        if (!firstTimePlaying)
        {
            player1.GetComponent<SnakePlayer>().startState();
            player2.GetComponent<SnakePlayer>().startState();
        } else { firstTimePlaying = false; }

        StartCoroutine(RespawnBites());
    }

    IEnumerator RespawnBites()
    {
        while (playing)
        {
            Debug.Log("Creating bite");
            float xPlacement = UnityEngine.Random.Range(1, maxX);
            float yPlacement = UnityEngine.Random.Range(1, maxY);
            if (UnityEngine.Random.value < 0.5f)
            {
                xPlacement *= -1;
            }
            if (UnityEngine.Random.value < 0.5f)
            {
                yPlacement *= -1;
            }
            bite = Instantiate(Resources.Load("Bite")) as GameObject;
            bite.transform.position = (new Vector3(xPlacement, yPlacement, -0.1f));
            bites.Add(bite);
            yield return new WaitForSeconds(3);
        }
    }


    public void TerminateGame(GameObject winner, int winType)
    {
        Debug.Log("Terminator called!!!!");
        playing = false;
        player1.GetComponent<SnakePlayer>().DestroyLeftovers();
        player2.GetComponent<SnakePlayer>().DestroyLeftovers();
        player1.SetActive(false);
        player2.SetActive(false);
        p1Ready = false;
        p2Ready = false;
        StopCoroutine("RespawnBites");
        for(int i = 0; i < bites.Count; i++) { Destroy(bites[i]);}
        showingWinner = true;
        ShowWinner(winner, winType);
        StartCoroutine("FlashReplayScreen");


    }

    public void ShowWinner(GameObject winner, int winType)
    {
        Debug.Log("Showing winner!");
        if(winner == null)
        {
            doneTieScreen.GetComponent<SpriteRenderer>().enabled = true;
            curScreen = doneTieScreen;
        }
        else if(winner == player1)
        {
            if(winType == 0){winP1CrashScreen.GetComponent<SpriteRenderer>().enabled = true; curScreen = winP1CrashScreen; }
            else{winP1hitScreen.GetComponent<SpriteRenderer>().enabled = true; curScreen = winP1hitScreen; }
        }
        else
        {
            if (winType == 0) { winP2CrashScreen.GetComponent<SpriteRenderer>().enabled = true; curScreen = winP2CrashScreen; }
            else { winP2hitScreen.GetComponent<SpriteRenderer>().enabled = true; curScreen = winP2hitScreen; }
        }
        StartCoroutine(FlashReplayScreen());
    }

    IEnumerator FlashReplayScreen()  // only player 1 can call this
    {
        yield return new WaitForSeconds(2);

        while (showingWinner)
        {
            yield return new WaitForSeconds(0.5f);
            if (!showingWinner) break;
            replayScreen.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(1);
            replayScreen.GetComponent<SpriteRenderer>().enabled = false;
        }
    }




}
