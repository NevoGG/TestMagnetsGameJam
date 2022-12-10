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
        
        public void SetDirection(Vector3 dir);
        public void SetDestroyedAnim();


        public void SetElectrocutedAnim();
        
        public void BackToNormAnim();

    }
}