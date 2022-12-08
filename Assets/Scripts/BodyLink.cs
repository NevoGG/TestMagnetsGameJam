using System;
using DefaultNamespace;
using UnityEngine;

enum Direction
{
    Up, Down, Left, Right, None
}
    
public class BodyLink : MonoBehaviour, Linkable
    {
        
        private static String PLAYER1_TAG = "PlayerBody1";
        private static String PLAYER2_TAG = "PlayerBody2";
        private static string BITE_TAG = "Bite";
        
        //animations:
        [SerializeField] private AnimationClip upAnimation;
        [SerializeField] private AnimationClip downAnimation;
        [SerializeField] private AnimationClip rightAnimation;
        [SerializeField] private AnimationClip leftAnimation;

        private SnakePlayer snakeParent;
        private int linkNum;

        private Vector3 prevLocation;
        private Vector3 curLocation;
        private Direction curDirection;

        private void SnakeDirectionSwitch(Direction newDir)
        {
            //change animationb and direction field
        }

        private void Start()
        {
            Debug.Log("Started");
            //StartCoroutine(RespawnBite());   
        }
        
        private void Update()
        {
        
        }
        
        public void OnTriggerEnter2D(Collider2D col)
        {
            if (CompareTag(PLAYER1_TAG) && col.CompareTag("Shot2")) wasShot();
            else if (CompareTag(PLAYER2_TAG) && col.CompareTag("Shot1")) wasShot();
        }

        private void wasShot()
        {
            snakeParent.DestroyTail(linkNum);
        }

        public Bםגטvoid setSnakeParent(SnakePlayer p)
        {
            snakeParent = p;
        }
        
        public void setLinkNum(int num)
        {
            linkNum = num;
        }
    }


