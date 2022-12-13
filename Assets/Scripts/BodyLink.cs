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
    public Animator animator;
        

    
        private SnakePlayer snakeParent;
        private int linkNum;

        private Vector3 prevLocation;
        private Vector3 curLocation;
        private Direction curDirection;

        private bool isDestroyed = false;
        private bool isElectrocuted = false;
    
        
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


        public void setSnakeParent(SnakePlayer p)
        {
            snakeParent = p;
            if (p.CompareTag(PLAYER1_TAG)) tag = PLAYER1BODY_TAG;
            else tag = PLAYER2BODY_TAG;
        }
        
        public void setLinkNum(int num) { linkNum = num; }

        public int getLinkNum() { return linkNum;}

        public void SetDirection(Direction dir)
        {
            MoveAnimUpdate(dir);
            curDirection = dir;
        } 

        public void WasShot()
        {
            if(!isDestroyed) snakeParent.DestroyTail(linkNum, tag);
        }
        
        private void MoveAnimUpdate(Direction dir)
        {
            if (dir == curDirection || isElectrocuted) return; //if its the same as the one now, no need to change.
            switch (dir)
            {
                case Direction.Down:
                    animator.SetBool("Down",true);
                    break;
                case Direction.Up:
                    animator.SetBool("Up",true);
                    break;
                case Direction.Left:
                    animator.SetBool("Left",true);
                    break;
                case Direction.Right:
                    animator.SetBool("Right",true);
                    break;
                case Direction.None:
                    animator.SetBool("Left",false);
                    animator.SetBool("Right",false);
                    animator.SetBool("Up",false);
                    animator.SetBool("Down",false);
                    break;
            }
        }

        public void SetDestroyed()
        {
            isDestroyed = true;
            animator.SetBool("Dead",true);
            animator.SetBool("Left",false);
            animator.SetBool("Right",false);
            animator.SetBool("Up",false);
            animator.SetBool("Down",false);
        }
        
        public void SetElectrocutedAnim()
        {
            isElectrocuted = true;
            animator.SetBool("Electrocuted", true);
        }

        public void BackToNormAnim()
        {
            isElectrocuted = false;
            //todo: set animation to regular, determined by MoveAnimUpdate
        }
    }




