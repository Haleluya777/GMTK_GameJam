using UnityEngine;

public class LocalGameManager : MonoBehaviour
{
    public static LocalGameManager instance;

    public StageManager stageManager;
    public CanvasManager canvasManager;
    public GameObject playerObj;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }
}
