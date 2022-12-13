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
    [SerializeField] int GAP = 20;
    [SerializeField] int electrocutionTime = 5;
    
    //audio sources:
    [SerializeField] AudioClip shotSound;
    [SerializeField] AudioClip changeDirectionSound;

    private AudioSource audioSource;
    
    public GameObject snakeBody;

    private List<GameObject> segments;
    private List<GameObject> all_segments_stored;
    private List<KeyValuePair<Vector3, Direction>> positionHistory = new List<KeyValuePair<Vector3, Direction>>();
    
    public GameObject shot;

    public float attackTimer = 0.35f;
    private float _currentAttackTimer;
    private bool _canAttack;
    
    public static SnakePlayer player1;
    public static SnakePlayer player2;
    public Animator animator;

    //saves current pressed key, to avoid redundant actions
    private Vector3 curDir;
    public Vector3 saveDir;
    
    private static string PLAYER1_TAG = "Player1";
    private static string PLAYER2_TAG = "Player2";
    private static String PLAYER1BODY_TAG = "Player1Body";
    private static String PLAYER2BODY_TAG = "Player2Body";
    private static string BITE_TAG = "Bite";
    private const string SHOT1_TAG = "Shot1";
    private const string SHOT2_TAG = "Shot2";
    private const float SHOT_DIST_FROM_HEAD = 1f;
    
    //electrocution by borders parameters:

    private bool isElectrocuted = false;
    private float curTimeElectrocution = 0;
    


    // added for general code
    public int speed = 200;
    private bool playing = false;
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
        all_segments_stored = new List<GameObject>();

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
    }
    
    // Update is called once per frame
    void Update()
    {
        //check if snake is electrocuted and if it can move
        if (isElectrocuted)
        {
            curTimeElectrocution += Time.deltaTime;
            if (curTimeElectrocution > electrocutionTime)
            {
                UnelectrocuteSnake();
            }
        }
        Move();
        Attack();
    }

    private void FixedUpdate()
    {
        FixedMove();
    }

    void FixedMove()
    {
        int index = 0;
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
    }

    void Move()
    {
        var dir = DetermineSnakeDirection();
        if (dir != Vector3.zero && curDir != Vector3.zero)
        {
            audioSource.PlayOneShot(changeDirectionSound);
            saveDir = dir;
        }

        if (!isElectrocuted)
        {
            //determine head animation:
            if (saveDir == Vector3.up) animator.SetBool("Up", true);
            else if (saveDir == Vector3.down) animator.SetBool("Down", true);
            else if (saveDir == Vector3.left) animator.SetBool("Left", true);
            else if (saveDir == Vector3.right) animator.SetBool("Right", true);
        }
        else
        {
            animator.SetBool("Up", false);
            animator.SetBool("Down", false);
            animator.SetBool("Left", false);
            animator.SetBool("Right", false);
            animator.SetBool("Electrocuted", true);
        }

        if (!IsInBoundaries()) ElectrocuteSnake();
        curDir = dir;
    }


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
            if (Input.GetKey(KeyCode.Q) && saveDir != Vector3.down) dir = Vector3.up;
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
        
        //hit bite
        if (col.gameObject.CompareTag(BITE_TAG))
        {
            col.gameObject.SetActive(false);
            Grow();
            return;
        }
        
        //hit bullet
        if (CompareTag(PLAYER1_TAG) && col.gameObject.CompareTag(SHOT2_TAG))
        {
            Debug.Log("Shot head terminating game");
            ReadyScript.gameRunner.TerminateGame(SnakePlayer.player2.gameObject, 1);
        }
        else if (CompareTag(PLAYER2_TAG) && col.gameObject.CompareTag(SHOT1_TAG))
        {
            Debug.Log("Shot head terminating game");
            ReadyScript.gameRunner.TerminateGame(SnakePlayer.player1.gameObject, 1);
        }
        
        //hit another head
        if ((CompareTag(PLAYER1_TAG) && col.CompareTag(PLAYER2_TAG)) || (CompareTag(PLAYER2_TAG) && col.CompareTag(PLAYER1_TAG)))
        {
            ReadyScript.gameRunner.TerminateGame(null, 0);
        }
        
        //player1 hit body
        if (CompareTag(PLAYER1_TAG))
        {
            if (col.CompareTag(PLAYER1BODY_TAG))
            {
                col.gameObject.TryGetComponent(out Linkable linkable);
                if (linkable.getLinkNum() > 2) ReadyScript.gameRunner.TerminateGame(SnakePlayer.player2.gameObject, 0);
            }
            else if (col.CompareTag(PLAYER2BODY_TAG)) ReadyScript.gameRunner.TerminateGame(SnakePlayer.player2.gameObject, 0); ;
        }
        
        //player2 hit body
        else if (CompareTag(PLAYER2_TAG))
        {
            if (col.CompareTag(PLAYER2BODY_TAG))
            {
                col.gameObject.TryGetComponent(out Linkable linkable);
                if (linkable.getLinkNum() > 2) ReadyScript.gameRunner.TerminateGame(SnakePlayer.player1.gameObject, 0);
            }
            else if (col.CompareTag(PLAYER1BODY_TAG)) ReadyScript.gameRunner.TerminateGame(SnakePlayer.player1.gameObject, 0); ;
        }
    }

    public void DestroyTail(int fromIdx, string tag)
    {
        StonifyTail(fromIdx, tag);
    }
    
    public void StonifyTail(int fromIdx, string tag)
    {
        int toDestroy = segments.Count - fromIdx;
        for (int i = 0; i < toDestroy; i++)
        {
            GameObject bodyLink = segments[segments.Count - 1].gameObject;
            if(!bodyLink.TryGetComponent(out Linkable linkable)) Debug.Log("Linkable failed on stonify");
            all_segments_stored.Add(bodyLink);
            linkable.SetDestroyed();
            segments.RemoveAt(segments.Count - 1);
        }
    }

    private void ElectrocuteSnake()
    {
        isElectrocuted = true;
        animator.SetBool("Electrocuted",true);
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
            if(!segments[i].TryGetComponent(out Linkable linkable)) Debug.Log("linkable failed in unelectrocute");;
            linkable.BackToNormAnim();
        }
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
                    transform.position + saveDir * SHOT_DIST_FROM_HEAD, shot.transform.rotation) as GameObject;
            }
            else {
                shotObj = Instantiate(Resources.Load("Player2Shot"),
                    transform.position + saveDir * SHOT_DIST_FROM_HEAD, shot.transform.rotation) as GameObject; 
            }
            Debug.Log("removing last");
            segments[segments.Count - 1].gameObject.SetActive(false);
            Destroy(segments[segments.Count - 1].gameObject);
            segments.RemoveAt(segments.Count - 1);

        }
        
    }

    public void DestroyLeftovers()
    {
        if (segments.Count > 1)
        {
            for (int i = segments.Count - 1; i > 0; i--)
            {
                Destroy(segments[i]);
                segments.RemoveAt(i);
            }
        }
        if (all_segments_stored.Count > 0)
        {
            for (int i = all_segments_stored.Count - 1; i >= 0; i--)
            {
                Destroy(all_segments_stored[i]);
                all_segments_stored.RemoveAt(i);
            }
        }
    }
}

//todo: bugs to fix:
//make two heads the same size
//set stone size
//set electricity size
//change spazm screen
//when link forms, it shows for one milisec in the middle of screen
//sound

