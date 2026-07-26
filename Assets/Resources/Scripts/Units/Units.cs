using UnityEngine;
using DG.Tweening;

public abstract class Units : PoolAble
{
    public bool isDead = false;

    public abstract void Dead();
}