using UnityEngine;

[CreateNodeMenu("BT/Condition/DistanceToPlayer")]
public class DistanceCondition : BTNode
{
    public float range = 10f;
    private EnemyController enemyController;

    public override NodeState Evaluate(AIController controller)
    {
        if (enemyController == null) enemyController = controller.ParentObj.GetComponent<EnemyController>();

        var enemy = controller.ParentObj;
        if (enemy == null) return NodeState.Failure;

        GameObject player = LocalGameManager.instance.playerObj;
        if (player == null) return NodeState.Failure;

        float dist = Vector2.Distance(enemy.transform.position, player.transform.position);

        if (dist <= range)
        {
            controller.curState = AIController.UnitState.Moving;
            return NodeState.Success;
        }
        else
        {
            controller.curState = AIController.UnitState.Idle;
            enemyController.direction = Vector2.zero;
            return NodeState.Failure;
        }
    }
}
