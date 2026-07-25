using UnityEngine;
using XNode;

[CreateAssetMenu]
public class BehaviorTreeGraph : NodeGraph
{
    public BTNode rootNode;
    public BlackBoard blackboard;
}
