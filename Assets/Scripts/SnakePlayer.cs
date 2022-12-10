using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

//minor change
public class SnakePlayer : MonoBehaviour
{
    [SerializeField] private Vector3 target;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject gameMusic;
    [SerializeField] int GAP = 40;
    [SerializeField] int electrocutionTime = 5;
    
    //audio sources:
    [SerializeField] AudioClip shotSound;
    [SerializeField] AudioClip changeDirectionSound;

    private AudioSource audioSource;
    
    public GameObject snakeBody;

    private List<GameObject> segments;
    private List<KeyValuePair<Vector3, Direction>> positionHistory = new List<KeyValuePair<Vector3, Direction>>();
    
    public GameObject shot;

    public float attackTimer = 0.35f;
    private float _currentAttackTimer;
    private bool _canAttack;
    
    public static SnakePlayer player1;
    public static SnakePlayer player2;

    //saves current pressed key, to avoid redundant actions
    private Vector3 curDir;
    public Vector3 saveDir;
    
    private static string PLAYER1_TAG = "Player1";
    private static string PLAYER2_TAG = "Player2";
    private static String PLAYER1BODY_TAG = "Player1Body";
    private static String PLAYER2BODY_TAG = "Player2Body";
    private static string BITE_TAG = "Bite";
    private const string TIE = "Tie";
    private const string SHOT1_TAG = "Shot1";
    private const string SHOT2_TAG = "Shot2";
    private const float SHOT_DIST_FROM_HEAD = 1f;
    
    //electrocution by borders parameters:

    private bool isElectrocuted = false;
    private float curTimeElectrocution = 0;
    


    // added for general code
    public int speed = 200;
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

        if (CompareTag(PLAYER1_TAG))
        {
            player1 = this;
        }
        if (CompareTag(PLAYER2_TAG))
        {
            player2 = this;

        }


        segments = new List<GameObject>();
        segments.Add(this.GameObject());

        startState();


    }

    public void startState()
    {
        if (CompareTag(PLAYER1_TAG))
        {
            transform.position = new Vector3(-200, 0, 0);
        }
        if (CompareTag(PLAYER2_TAG))
        {
            transform.position = new Vector3(200, 0, 0);
        }
        target = transform.position;
        saveDir = Vector3.zero;

        // amplify game music
        AudioSource music = gameMusic.GetComponent<AudioSource>();
        music.volume = 1;
        numPoints = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        Move();
        Attack();
        
        //check if snake is electrocuted and if it can move
        if (isElectrocuted)
        {
            curTimeElectrocution += Time.deltaTime;
            if (curTimeElectrocution > electrocutionTime)
            {
                UnelectrocuteSnake();
            }
        }
    }

    private void StartWhenReady()
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
    }

    void Move()
    {
        var dir = DetermineSnakeDirection();
        if (dir != Vector3.zero && curDir != Vector3.zero)
        {
            audioSource.PlayOneShot(changeDirectionSound);
            saveDir = dir;
        }

        // ADDED for boarders
        //dir = checkBoundries(dir);

        //for (int i = segments.Count - 1; i > 0; i--)
        int index = 0;
        if (!IsInBoundaries()) ElectrocuteSnake();
        if(!isElectrocuted)
        {
            transform.position += saveDir * (speed * Time.deltaTime); //change head position
            positionHistory.Insert(0, new KeyValuePair<Vector3, Direction>(transform.position, vecToDir(saveDir)));
            for (int i = 1; i < segments.Count; i++)
            {
                Vector3 point = positionHistory[Math.Min(i * GAP, positionHistory.Count - 1)].Key;
                Direction linkDirection = positionHistory[Math.Min(i * GAP, positionHistory.Count - 1)].Value;
                segments[i].transform.position = point;
                if(!segments[i].TryGetComponent(out Linkable linkable)) Debug.Log("linkable failed in move");;
                linkable.SetDirection(linkDirection);
                index++;
            }
        }
        curDir = dir;
    }

    // ADDED for boarders
    //   private Vector3 checkBoundries(Vector3 dir)
    //   {
    //       if (transform.position.x >= maxX || transform.position.x <= minX)
    //       {
    //           if ((saveDir == Vector3.right && transform.position.x >= maxX) || (saveDir == Vector3.left && transform.position.x <= minX))
    //         {
    //           if (UnityEngine.Random.value < 0.5f) { dir = new Vector3(0, 1, 0); }
    //               else { dir = new Vector3(0, -1, 0); }
    //               if (transform.position.x >= maxX)
    //               {
    //                   transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
    //               }
    //                else
    //                {
    //                   transform.position = new Vector3(minX, transform.position.y, transform.position.z);
    //               }
    //           }
    //       }
    //
    //      if (transform.position.y >= maxY || transform.position.y <= minY)
    //       {
    //           if ((saveDir == Vector3.up && transform.position.y >= maxY) || (saveDir == Vector3.down && transform.position.y <= minY))
    //           {
    //               if (UnityEngine.Random.value < 0.5f) { dir = Vector3.right; }
    //               else { dir = Vector3.left; }
    //
    //                if (transform.position.y >= maxY)
    //                {
    //                    transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
    //               }
    //               else
    //               {
    //                   transform.position = new Vector3(transform.position.x, minY, transform.position.z);
    //               }
    //           }
    //       }
    //       return dir;
    
    public Direction vecToDir(Vector3 vec)
    {
        if (vec == Vector3.up) return Direction.Up;
        if (vec == Vector3.down) return Direction.Down;
        if (vec == Vector3.left) return Direction.Left;
        if (vec == Vector3.right) return Direction.Right;
        return Direction.None;
    }
    

    private Vector3 DetermineSnakeDirection()
    {
        Vector3 dir = Vector3.zero;
        if (CompareTag(PLAYER1_TAG))
        {
            if (Input.GetKey(KeyCode.W) && saveDir != Vector3.down) dir = Vector3.up;
            else if (Input.GetKey(KeyCode.S) && saveDir != Vector3.up) dir = Vector3.down;
            else if (Input.GetKey(KeyCode.D) && saveDir != Vector3.left) dir = Vector3.right;
            else if (Input.GetKey(KeyCode.A) && saveDir != Vector3.right) dir = Vector3.left;
        }
        else if (CompareTag(PLAYER2_TAG))
        {
            if (Input.GetKey(KeyCode.UpArrow) && saveDir != Vector3.down) dir = Vector3.up;
            else if (Input.GetKey(KeyCode.DownArrow) && saveDir != Vector3.up) dir = Vector3.down;
            else if (Input.GetKey(KeyCode.RightArrow) && saveDir != Vector3.left) dir = Vector3.right;
            else if (Input.GetKey(KeyCode.LeftArrow) && saveDir != Vector3.right) dir = Vector3.left;
        }

        return dir;
    }


    private bool IsInBoundaries()
    {
        if (transform.position.x >= maxX || transform.position.x <= minX)
        {
            float xValue = minX + 1;
            if (transform.position.x >= maxX) xValue = maxX - 1;
            transform.position = new Vector3(xValue, transform.position.y, transform.position.z);
            return false;
        }
        if (transform.position.y >= maxY || transform.position.y <= minY)
        {
            float yValue = minY + 1;
            if (transform.position.y >= maxY) yValue = maxY - 1;
            transform.position = new Vector3(transform.position.x, yValue, transform.position.z);
            return false;
        }
        return true;
    }

    void Grow()
    {
        GameObject bodyLink;
        
        //instantiate right type according to player
        if (CompareTag(PLAYER1_TAG)){bodyLink = Instantiate(Resources.Load("Player1Body")) as GameObject;}
        else{bodyLink = Instantiate(Resources.Load("Player2Body")) as GameObject;}
        
        //set link fields
        if(!bodyLink.TryGetComponent(out Linkable linkable)) Debug.Log("Linkable failed"); //check if is linkable
        linkable.setSnakeParent(this);
        linkable.setLinkNum(segments.Count);
        //

        GameObject lastLink = segments[segments.Count - 1];
        
        if(!lastLink.TryGetComponent(out Linkable last)) Debug.Log("linkable failed in grow");
        segments.Add(bodyLink);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(BITE_TAG))
        {
            numPoints += 1;
            pscoreBoard.text = "Score: " + numPoints.ToString();
            col.gameObject.SetActive(false);
            Grow();
            return;
        }
        
    }

    public void DestroyTail(int fromIdx, string tag)
    {
        //changeable if we just want it to disappear
        StonifyTail(fromIdx, tag);
        
        // if (CompareTag(tag)) TerminateGame(tag); //if hit head, terminate game
        //     int toDestroy = segments.Count - fromIdx;
        // for (int i = 0; i < toDestroy; i++)
        // {
        //     segments[segments.Count - 1].gameObject.SetActive(false);
        //     Destroy(segments[segments.Count - 1].gameObject);
        //     segments.RemoveAt(segments.Count - 1);
        //     numPoints -= 1;
        //     pscoreBoard.text = "Score: " + numPoints.ToString();
        // }
    }
    
    public void StonifyTail(int fromIdx, string tag)
    {
        if (CompareTag(tag))
        {
            Debug.Log("Term - OnTrigger  stonify tail");
            ReadyScript.gameRunner.TerminateGame(SnakePlayer.player2.gameObject, 0);
            //TerminateGame(tag); //if hit head, terminate game
        }
        int toDestroy = segments.Count - fromIdx;
        for (int i = 0; i < toDestroy; i++)
        {
            GameObject bodyLink = segments[segments.Count - 1].gameObject;
            if(!bodyLink.TryGetComponent(out Linkable linkable)) Debug.Log("Linkable failed on stonify");
            linkable.SetDestroyed();
            segments.RemoveAt(segments.Count - 1);
            numPoints -= 1;
            pscoreBoard.text = "Score: " + numPoints.ToString();
        }
    }
    
    private void ElectrocuteSnake()
    {
        isElectrocuted = true;
        curTimeElectrocution = 0f;
        // todo: start electrocution sound
        for (int i = 1; i < segments.Count; i++)
        {
            if(!segments[i].TryGetComponent(out Linkable linkable)) Debug.Log("linkable failed in electrocute");;
            linkable.SetElectrocutedAnim();
        }
    }
    
    private void UnelectrocuteSnake()
    {
        isElectrocuted = false;
        curTimeElectrocution = 0f;
        // todo: start electrocution sound
        for (int i = 1; i < segments.Count; i++)
        {
            if(!segments[i].TryGetComponent(out Linkable linkable)) Debug.Log("linkable failed in electrocute");;
            linkable.SetElectrocutedAnim();
        }
    }

    public void TerminateGame(string mssg)
    {
        Debug.Log("Terminator called!!!!");
        
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && CompareTag(PLAYER1_TAG)  && segments.Count>1) CanAttack();
        else if (Input.GetKeyDown(KeyCode.RightShift) && CompareTag(PLAYER2_TAG) && segments.Count > 1) CanAttack();
    }

    void CanAttack()
    {
        if (_canAttack)
        {
            //play shot sound:
            audioSource.PlayOneShot(shotSound);
            
            _canAttack = false;
            attackTimer = 0f;
            if (saveDir.y > 0) shot.transform.eulerAngles = new Vector3(0,0,90); //todo: make vectors not new
            else if (saveDir.y < 0) shot.transform.eulerAngles = new Vector3(0,0,-90);
            else if (saveDir.x < 0) shot.transform.eulerAngles = new Vector3(0,0,180);
            else if (saveDir.x > 0) shot.transform.eulerAngles = new Vector3(0,0,0);
            GameObject shotObj = null;
            if (CompareTag(PLAYER1_TAG)) {
                shotObj =  Instantiate(Resources.Load("Player1Shot"),
                    spawnPoint.position + saveDir * SHOT_DIST_FROM_HEAD, shot.transform.rotation) as GameObject;
            }
            else {
                shotObj = Instantiate(Resources.Load("Player2Shot"),
                    spawnPoint.position+ saveDir * SHOT_DIST_FROM_HEAD, shot.transform.rotation) as GameObject; 
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

        // ADDED for borders
        transform.position = new Vector3(200, 0, 0);
        p2.gameObject.transform.position = new Vector3(-200, 0, 0);
        target = new Vector3(200, 0, 0);
        p2.gameObject.GetComponent<SnakePlayer>().target = new Vector3(-200, 0, 0);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        p2.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        // <-----

        playing = true;
        p2.gameObject.GetComponent<SnakePlayer>().playing = true;
        StartCoroutine(RespawnBites());
    }

    IEnumerator RespawnBites()
    {
        while (!playing) { }
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
            yield return new WaitForSeconds(3);
        }
    }
}

//todo: bugs:
//todo: terminate game fails
//todo: food instantiates out of screen
// todo: background not showing
//todo: make tie an option (mssg containing tie tag)- tie art?
