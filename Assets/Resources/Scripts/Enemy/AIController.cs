using UnityEngine;

public class AIController : MonoBehaviour
{
    public enum UnitState { Moving, Attacking, Idle, Dead }

    public UnitState curState;
    public GameObject ParentObj;
    public BehaviorTreeGraph behaviorTree;
    public Animator anim;

    void LateUpdate()
    {
        switch (curState)
        {
            case UnitState.Moving:
                anim.CrossFade("Walking", 0f);
                break;

            case UnitState.Idle:
                anim.CrossFade("Idle", 0f);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (behaviorTree != null && behaviorTree.rootNode != null)
        {
            behaviorTree.rootNode.Evaluate(this);
        }
    }

    public void PlayAnimation(string animName)
    {
        anim.CrossFade(animName, 0f);
    }
}
