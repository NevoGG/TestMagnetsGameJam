using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

public enum Direction
{
    Up, Down, Left, Right, None
}
    
public class BodyLink : MonoBehaviour, Linkable
{
    [SerializeField] private List<Sprite> sprites;
    private static string LEFT = "BlueLeft";
    private static string RIGHT = "BlueRight";
    private static string UP = "BlueDown";
    private static string DOWN = "BlueDown";
    private static string DEAD = "BlueElec";
    private static string ELEC = "BlueElec";
    private static String PLAYER1_TAG = "Player1";
    private static String PLAYER2_TAG = "Player2";
    private static String PLAYER1BODY_TAG = "Player1Body";
    private static String PLAYER2BODY_TAG = "Player2Body";
    private const string SHOT1_TAG = "Shot1";
    private const string SHOT2_TAG = "Shot2";
    private static string BITE_TAG = "Bite";
        

    
        private SnakePlayer snakeParent;
        private int linkNum;

        private Vector3 prevLocation;
        private Vector3 curLocation;
        private Vector3 curDirection;

        private bool isDestroyed = false;
    
        
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private void SnakeDirectionSwitch(Direction newDir)
        {
            //change animationb and direction field
        }

        private void Start()
        {
            Debug.Log("Started");
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator    = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void Update()
        {
        }
        
        public void OnTriggerEnter2D(Collider2D col)
        {
            if (isDestroyed) return; //object is a wall if destroyed
            //shot other player:
            if (col.CompareTag(SHOT2_TAG) && CompareTag((PLAYER1BODY_TAG))) WasShot();
            if (col.CompareTag(SHOT1_TAG) && CompareTag((PLAYER2BODY_TAG))) WasShot();
            //shot oneself:
            if (col.CompareTag(SHOT2_TAG) && CompareTag((PLAYER2BODY_TAG)) && linkNum > 1) WasShot();
            if (col.CompareTag(SHOT1_TAG) && CompareTag((PLAYER1BODY_TAG)) && linkNum > 1) WasShot();
        }
        

        public void setSnakeParent(SnakePlayer p)
        {
            snakeParent = p;
            if (p.CompareTag(PLAYER1_TAG)) tag = PLAYER1BODY_TAG;
            else tag = PLAYER2BODY_TAG;
        }
        
        public void setLinkNum(int num) { linkNum = num; }

        public int getLinkNum() { return linkNum;}

        public void SetDirection(Vector3 dir) { MoveAnimUpdate(0); } //todo: filler - do I need dir?

        private void WasShot()
        {
            if(!isDestroyed) snakeParent.DestroyTail(linkNum, tag);
        }
        
        private void MoveAnimUpdate(Direction dir= 0)
        {
            switch (dir)
            {
                case Direction.Down: //todo: set animation to down
                case Direction.Up: //todo: set animation to up
                case Direction.Left: //todo: set animation to left
                case Direction.Right: //todo: set animation to right
                default: break;
            }

        }

        public void SetDestroyed()
        {
            isDestroyed = true;
            _spriteRenderer.sprite = sprites[1];
        }
        
        public void SetElectrocutedAnim()
        {
            //todo: set animation to electrocuted
        }

        public void BackToNormAnim()
        {
            //todo: set animation to regular, determined by MoveAnimUpdate
        }
    }

//todo: create sprites and animators for electrocuted, destroyed and four movement sides
//todo: set electrocution sound
//todo: fix animation- crashing unity


