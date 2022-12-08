using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;


public class SnakePlayer : MonoBehaviour
{
    [SerializeField] private Vector3 target;
    [SerializeField] public Vector3 saveDir;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject gameMusic;
    
    //audio sources:
    [SerializeField] AudioClip shotSound;
    [SerializeField] AudioClip changeDirectionSound;

    private AudioSource audioSource;
    
    public GameObject snakeBody;

    private List<GameObject> segments;
    public GameObject shot;

    public float attackTimer = 0.35f;
    private float _currentAttackTimer;
    private bool _canAttack;
    
    public static SnakePlayer player1;
    public static SnakePlayer player2;

    //saves current pressed key, to avoid redundant actions
    private Vector3 curDir;
    
    private static String PLAYER1_TAG = "Player1";
    private static String PLAYER2_TAG = "Player2";
    private static string BITE_TAG = "Bite";

    // added for general code
    public int speed = 100;
    public TMP_Text pscoreBoard;
    public TMP_Text pInstructions;
    public TMP_Text pReady;
    public TMP_Text goMssg;
    public TMP_Text finalMSG;
    public bool ready = false;
    private bool playing = false;
    public GameObject p2 = null;
    private int numPoints = 0;
    private GameObject bite = null;
    private bool calledOnce = true;
    private int bodyDist = 25;
    // <=====
    
    private float maxX = 440;
    private float minX = -440;
    private float maxY = 220;
    private float minY = -220;



    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        
        if (this.tag == "Player1") player1 = this;
        if (this.tag == "Player2") player2 = this;

        
        segments = new List<GameObject>();
        segments.Add(this.GameObject());
        target = transform.position;
        saveDir = Vector3.zero;

        // added for general code
        playing = false;

        // <=====

    }


    // Update is called once per frame
    void Update()
    {
        if (!playing)   // added for general code
        {
            if (CompareTag(PLAYER1_TAG))
            {
                if (Input.GetKey(KeyCode.RightShift))
                {
                    pInstructions.gameObject.SetActive(false);
                    ready = true;
                    pReady.gameObject.SetActive(true);
                }
                if (ready && p2.GetComponent<SnakePlayer>().ready && calledOnce)
                {
                    Debug.Log("Marked as Ready");
                    calledOnce = false;
                    StartCoroutine(StartGame());            
                }
            }     
            else if (CompareTag(PLAYER2_TAG))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    pInstructions.gameObject.SetActive(false);
                    ready = true;
                    pReady.gameObject.SetActive(true);
                }
            }


        }  // <=====
        else
        {
            Move();
            Attack();
        }

        
        
    }

    void Move()
    {
        Vector3 dir = Vector3.zero;
        if (CompareTag(PLAYER1_TAG))
        {
            if (Input.GetKey(KeyCode.UpArrow)) dir = Vector3.up;
            else if (Input.GetKey(KeyCode.DownArrow)) dir = Vector3.down;
            else if (Input.GetKey(KeyCode.RightArrow)) dir = Vector3.right;
            else if (Input.GetKey(KeyCode.LeftArrow)) dir = Vector3.left;
        }
        else if (CompareTag(PLAYER2_TAG))
        {
            if (Input.GetKey(KeyCode.W)) dir = Vector3.up;
            else if (Input.GetKey(KeyCode.S)) dir = Vector3.down;
            else if (Input.GetKey(KeyCode.D)) dir = Vector3.right;
            else if (Input.GetKey(KeyCode.A)) dir = Vector3.left;
        }

        if (dir != Vector3.zero && curDir != Vector3.zero)
        {
            audioSource.PlayOneShot(changeDirectionSound); 
        }

        // ADDED for boarders
        // dir = checkBoundries(dir);

        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].transform.position = segments[i - 1].transform.position - (saveDir * bodyDist);
        }

        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

        if (dir.x != 0) saveDir = Vector3.right * dir.x;
        if (dir.y != 0) saveDir = Vector3.up * dir.y;
        if (transform.position == target) target += saveDir;
        curDir = dir;
    }

    private Vector3 checkBoundries(Vector3 dir)
    {
        if (transform.position.x >= maxX || transform.position.x <= minX)
        {
            if ((saveDir == Vector3.right && transform.position.x >= maxX) ||
                (saveDir == Vector3.left && transform.position.x <= minX))
            {
                if (UnityEngine.Random.value < 0.5f)
                {
                    dir = new Vector3(0, 1, 0);
                }
                else
                {
                    dir = new Vector3(0, -1, 0);
                }

                if (transform.position.x >= maxX)
                {
                    transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(minX, transform.position.y, transform.position.z);
                }
            }
        }
        return dir;
    }

    void Grow()
    {
        Debug.Log("Growing");
        GameObject bodyLink;
        if (CompareTag(PLAYER1_TAG)){bodyLink = Instantiate(Resources.Load("Player1Body")) as GameObject;}
        else{bodyLink = Instantiate(Resources.Load("Player2Body")) as GameObject;}
        // BodyLink linkScript = bodyLink.GetComponent<BodyLink>();
        if(!bodyLink.TryGetComponent(out Linkable linkable)) Debug.Log("Linkable failed");;
        linkable.setSnakeParent(this);
        linkable.setLinkNum(segments.Count);
        //
        bodyLink.transform.position = this.transform.position;
        segments.Add(bodyLink);
        //body.transform.parent = this.transform;
        
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(BITE_TAG))
        {
            // added for general code
            numPoints += 1;
            pscoreBoard.text = "Score: " + numPoints.ToString();
            Destroy(col.gameObject);
            // <=====
            Grow();
        }

        // if (CompareTag(PLAYER1_TAG) && col.tag == "Shot2") Destroy(this.gameObject);
        // else if (this.tag == PLAYER2_TAG && col.tag == "Shot1") Destroy(this.gameObject);

        
    }

    public void DestroyTail(int fromIdx)
    {
        for (int i = fromIdx; i < segments.Count; i++)
        {
            Destroy(segments[i]);
        }   
    }

    public void TerminateGame(String mssg)
    {
        pReady.gameObject.SetActive(false);
        SnakePlayer.player1.playing = false;
        finalMSG.gameObject.SetActive(true);
        SnakePlayer.player1.gameObject.SetActive(false);
        SnakePlayer.player2.gameObject.SetActive(false);
        finalMSG.text = "GAME OVER! \n\n" + mssg + " WINS!";

    }


    void Attack()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer > _currentAttackTimer) _canAttack = true;
        if (Input.GetKeyDown(KeyCode.RightShift) && this.tag == "Player1"  && segments.Count>1) CanAttack();
        else if (Input.GetKeyDown(KeyCode.LeftShift) && this.tag == "Player2" && segments.Count > 1) CanAttack();
    }

    void CanAttack()
    {
        Debug.Log("Can Attack");
        if (_canAttack)
        {
            //play shot sound:
            audioSource.PlayOneShot(shotSound);
            
            Debug.Log("Attacking");
            _canAttack = false;
            attackTimer = 0f;
            if (saveDir.y > 0) shot.transform.eulerAngles = new Vector3(0,0,90);
            else if (saveDir.y < 0) shot.transform.eulerAngles = new Vector3(0,0,-90);
            else if (saveDir.x < 0) shot.transform.eulerAngles = new Vector3(0,0,180);
            else if (saveDir.x > 0) shot.transform.eulerAngles = new Vector3(0,0,0);
            GameObject shotObj = null;
            if (this.tag == "Player1") {
                shotObj =  Instantiate(Resources.Load("Player1Shot"), spawnPoint.position, shot.transform.rotation) as GameObject;
            }
            else {
                shotObj = Instantiate(Resources.Load("Player2Shot"), spawnPoint.position, shot.transform.rotation) as GameObject; 
            }
            Debug.Log("removing last");
            segments[segments.Count - 1].gameObject.SetActive(false);
            Destroy(segments[segments.Count - 1].gameObject);
            segments.RemoveAt(segments.Count - 1);
            numPoints -= 1;
            pscoreBoard.text = "Score: " + numPoints.ToString();

        }
        
    }

    // added for general code
    IEnumerator StartGame()  // only player 1 can call this
    {
        // amplify game music
        AudioSource music = gameMusic.GetComponent<AudioSource>();
        music.volume = 1;

        yield return new WaitForSeconds(1);
        pReady.gameObject.SetActive(false);
        p2.gameObject.GetComponent<SnakePlayer>().pReady.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        goMssg.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        goMssg.gameObject.SetActive(false);
        playing = true;
        p2.gameObject.GetComponent<SnakePlayer>().playing = true;
        StartCoroutine(RespawnBites());
    }

    IEnumerator RespawnBites()
    {
        Debug.Log("Called respawn");
        while (!playing) { }
        while (playing)
        {
            Debug.Log("Creating bite");
            float xPlacement = UnityEngine.Random.Range(1, 450);
            float yPlacement = UnityEngine.Random.Range(1, 250);
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
            yield return new WaitForSeconds(3);
        }
    }
        // <=====

    }
