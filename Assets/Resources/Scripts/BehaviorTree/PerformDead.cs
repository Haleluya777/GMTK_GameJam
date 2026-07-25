using UnityEngine;
using DG.Tweening;

[CreateNodeMenu("BT/Action/PerformDead")]
public class PerformDead : BTNode
{
    private EnemyController enemyController;

    public override NodeState Evaluate(AIController controller)
    {
        if (enemyController == null) enemyController = controller.ParentObj.GetComponent<EnemyController>();

        if (!enemyController.isDead) return NodeState.Failure;

        controller.PlayAnimation("Dead");
        DOVirtual.DelayedCall(1.5f, () => controller.ParentObj.SetActive(false));
        return NodeState.Success;
    }
}
