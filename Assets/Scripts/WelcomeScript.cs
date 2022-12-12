using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WelcomeScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject welcomeScreen = null;
    public GameObject t1Screen = null;
    public GameObject t2Screen = null;
    public GameObject t3Screen = null;
    public GameObject t4Screen = null;
    public GameObject t5Screen = null;
    public GameObject t6Screen = null;
    public GameObject t7Screen = null;

    //public GameObject rNoneScreen = null;
    //public GameObject rplayer1Screen = null;
    //public GameObject rplayer2Screen = null;
    //public GameObject rBothScreen = null;


    private int stage;
    private bool justChanged = false;

    void Start()
    {
        welcomeScreen.GetComponent<SpriteRenderer>().enabled = true;
        welcomeScreen.GetComponent<Animator>().enabled = true;
        stage = 0;
    }

    //SceneManager.LoadScene("LightScene");

    // Update is called once per frame
    void Update()
    {
        if (stage == 0 && !justChanged && (Input.GetKey(KeyCode.RightShift)|| Input.GetKey(KeyCode.LeftShift)))
        {
            stage += 1;
            moveStageForward();
            justChanged = true;
            StartCoroutine(WaitLittle());
        }
        else if (stage >= 1 && stage <= 8)
        {
            if (!justChanged && Input.GetKey(KeyCode.RightShift))
            {
                stage += 1;
                moveStageForward();
                justChanged = true;
                StartCoroutine(WaitLittle());
            }
            if (!justChanged && Input.GetKey(KeyCode.LeftShift))
            {
                moveStageBackwards();
                stage -= 1;
                justChanged = true;
                StartCoroutine(WaitLittle());
            }
        }
    }

    IEnumerator WaitLittle()  // only player 1 can call this
    {
        yield return new WaitForSeconds(0.5f);
        justChanged = false;
    }

    private void moveStageForward()
    {

        if (stage == 1)
        {
            welcomeScreen.GetComponent<SpriteRenderer>().enabled = false;
            welcomeScreen.GetComponent<Animator>().enabled = false;
            t1Screen.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (stage == 2)
        {
            t1Screen.GetComponent<SpriteRenderer>().enabled = false;
            t2Screen.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (stage == 3)
        {
            t2Screen.GetComponent<SpriteRenderer>().enabled = false;
            t3Screen.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (stage == 4)
        {
            t3Screen.GetComponent<SpriteRenderer>().enabled = false;
            t4Screen.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (stage == 5)
        {
            t4Screen.GetComponent<SpriteRenderer>().enabled = false;
            t5Screen.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (stage == 6)
        {
            t5Screen.GetComponent<SpriteRenderer>().enabled = false;
            t6Screen.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (stage == 7)
        {
            t6Screen.GetComponent<SpriteRenderer>().enabled = false;
            t7Screen.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (stage >= 8)
        {
            StopCoroutine(WaitLittle());
            SceneManager.LoadScene("LightScene");
        }
    }

    private void moveStageBackwards()
    {

        if (stage == 1)
        {
            welcomeScreen.GetComponent<SpriteRenderer>().enabled = true;
            welcomeScreen.GetComponent<Animator>().enabled = true;
            t1Screen.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (stage == 2)
        {
            t1Screen.GetComponent<SpriteRenderer>().enabled = true;
            t2Screen.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (stage == 3)
        {
            t2Screen.GetComponent<SpriteRenderer>().enabled = true;
            t3Screen.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (stage == 4)
        {
            t3Screen.GetComponent<SpriteRenderer>().enabled = true;
            t4Screen.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (stage == 5)
        {
            t4Screen.GetComponent<SpriteRenderer>().enabled = true;
            t5Screen.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (stage == 6)
        {
            t5Screen.GetComponent<SpriteRenderer>().enabled = true;
            t6Screen.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (stage == 7)
        {
            t6Screen.GetComponent<SpriteRenderer>().enabled = true;
            t7Screen.GetComponent<SpriteRenderer>().enabled = false;
        }
    }



}