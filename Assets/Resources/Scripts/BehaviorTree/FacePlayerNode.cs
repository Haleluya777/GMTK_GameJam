using UnityEngine;

[CreateNodeMenu("BT/Action/FacePlayer")]
public class FacePlayerNode : BTNode
{
    public override NodeState Evaluate(AIController controller)
    {
        var enemy = controller.ParentObj.GetComponent<EnemyController>();
        if (enemy == null) return NodeState.Failure;

        GameObject player = LocalGameManager.instance.playerObj;
        if (player == null) return NodeState.Failure;

        float dirX = player.transform.position.x - enemy.transform.position.x;

        float scaleX = dirX >= 0 ? -1 : 1;
        enemy.transform.localScale = new Vector2(scaleX, 1);
        enemy.direction = dirX >= 0 ? Vector2.right : Vector2.left;

        return NodeState.Success;
    }
}
