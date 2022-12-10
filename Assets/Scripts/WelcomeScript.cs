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

    //public GameObject rNoneScreen = null;
    //public GameObject rplayer1Screen = null;
    //public GameObject rplayer2Screen = null;
    //public GameObject rBothScreen = null;


    private bool tstage = true;
    private int stage = 0;
    private bool justChanged = false;

    void Start()
    {
        welcomeScreen.GetComponent<SpriteRenderer>().enabled = true;
        welcomeScreen.GetComponent<Animator>().enabled = true;
    }

    //SceneManager.LoadScene("LightScene");

    // Update is called once per frame
    void Update()
    {
        if (tstage == true)
        {
            if (stage >= 0 && stage <= 7)
            {
                if (!justChanged && Input.GetKey(KeyCode.Space))
                {
                    stage += 1;
                    updateStage();
                    justChanged = true;
                    StartCoroutine(WaitLittle());
                }
            }
            else { tstage = false; }
        }
    }

    IEnumerator WaitLittle()  // only player 1 can call this
    {
        yield return new WaitForSeconds(0.5f);
        justChanged = false;
    }

    private void updateStage()
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
            StopCoroutine(WaitLittle());
            tstage = true;
            SceneManager.LoadScene("LightScene");
        }


    }





}