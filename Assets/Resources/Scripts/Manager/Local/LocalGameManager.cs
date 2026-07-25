using UnityEngine;
using UnityEngine.InputSystem;

public class LocalGameManager : MonoBehaviour
{
    public static LocalGameManager instance;

    public StageManager stageManager;
    public CanvasManager canvasManager;
    public CaptureManager captureManager;
    public GameObject playerObj;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        foreach (var child in GetComponentsInChildren<IDataInitializable>())
        {
            child.DataInitialize();
        }
    }

    public void GameOver()
    {
        canvasManager.GameOverUI();
    }

    public void DisableInput()
    {
        InputSystem.DisableDevice(Keyboard.current);
    }

    public void EnableInput()
    {
        InputSystem.EnableDevice(Keyboard.current);
    }
}
