using System;
using DefaultNamespace;
using UnityEngine;

namespace DefaultNamespace
{
    public interface Linkable
    {
        public void setSnakeParent(SnakePlayer p);

        public void setLinkNum(int num);
        
        public int  getLinkNum();
        
        public void SetDirection(Direction dir);
        public void SetDestroyed(float delay);


        public void SetElectrocutedAnim();
        
        public void BackToNormAnim();

        public void WasShot();
    }
}