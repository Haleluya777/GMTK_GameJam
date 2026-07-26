using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CaptureManager : MonoBehaviour
{
    [SerializeField] private List<Transform> corners;
    public List<AudioClip> countDownAudio;

    private int resolution = 1024;
    private float time;
    public float timer;
    public float timerSpeed;
    //private Sprite captured;
    private float size;
    private int lastSec = 0;
    public bool isCaptured;
    private GameObject playerObj;
    private GlobalGameManager globalGameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        size = 1200f;
        //image.rectTransform.sizeDelta = new Vector2(size, size * .5f);
        playerObj = LocalGameManager.instance.playerObj;
        globalGameManager = GlobalGameManager.instance;
        countDownAudio = globalGameManager.soundManager.countDownSound;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCaptured) time += Time.deltaTime * timerSpeed;

        int currnetSec = Mathf.FloorToInt(time);
        if (currnetSec != lastSec)
        {
            lastSec = currnetSec;

            int remaining = Mathf.CeilToInt(timer - time);
            if (remaining >= 1 && remaining <= 5)
            {
                globalGameManager.soundManager.PlaySound(countDownAudio[remaining - 1]);
            }
            else if (remaining <= 0)
            {
                globalGameManager.soundManager.PlaySound(globalGameManager.soundManager.laserSound);
            }
        }

        if (time >= timer && !isCaptured)
        {
            //captured = Capture();
            LocalGameManager.instance.stageManager.StageResultDirection();
            isCaptured = true;
            time = 0;
            timerSpeed = 1f;
        }
    }

    public void SetCoutDownSound(GimmickType gimmickType)
    {
        Debug.Log("사운드 초기화.");
        if (gimmickType == GimmickType.FastCount)
        {
            countDownAudio = globalGameManager.soundManager.countDownSoundError;
        }
        else
        {
            countDownAudio = globalGameManager.soundManager.countDownSound;
        }
    }

    public void SetTimerSpeed(float value)
    {
        timerSpeed = value;
    }

    public List<GameObject> CheckCapture()
    {
        Bounds bounds = GetBounds();
        List<GameObject> results = new List<GameObject>();
        List<GameObject> units = LocalGameManager.instance.stageManager.unitList;

        foreach (var unit in units)
        {
            Bounds unitBounds = unit.GetComponent<Renderer>().bounds;

            float intersectMinX = Mathf.Max(bounds.min.x, unitBounds.min.x);
            float intersectMaxX = Mathf.Min(bounds.max.x, unitBounds.max.x);
            float intersectMinY = Mathf.Max(bounds.min.y, unitBounds.min.y);
            float intersectMaxY = Mathf.Min(bounds.max.y, unitBounds.max.y);

            float intersectWidth = Mathf.Max(0, intersectMaxX - intersectMinX);
            float intersectHeight = Mathf.Max(0, intersectMaxY - intersectMinY);

            float intersectArea = intersectWidth * intersectHeight;
            float unitArea = unitBounds.size.x * unitBounds.size.y;

            if (unitArea > 0 && intersectArea / unitArea >= .45f)
            {
                results.Add(unit);
            }
        }

        return results;
    }

    Bounds GetBounds()
    {
        Vector3 min = corners[0].position;
        Vector3 max = corners[0].position;

        for (int i = 1; i < corners.Count; i++)
        {
            min = Vector3.Min(min, corners[i].position);
            max = Vector3.Max(max, corners[i].position);
        }

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    public void SetValue(bool boolValue, float floatValue)
    {
        isCaptured = boolValue;
        timer = floatValue;
    }

    void OnDrawGizmos()
    {
        if (corners == null || corners.Count < 4) return;

        Bounds bounds = GetBounds();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        for (int i = 0; i < corners.Count; i++)
        {
            if (corners[i] == null) continue;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(corners[i].position, 0.1f);
            //Debug.Log($"Corner[{i}]: pos={corners[i].position}");
        }
    }

    public void ResetLocalScale()
    {
        gameObject.transform.localScale = new Vector2(1, 1);
    }

    // private Sprite Capture()
    // {
    //     Bounds bounds = GetBounds();

    //     float halfHeight = cam.orthographicSize;
    //     float halfWidth = halfHeight * cam.aspect;
    //     Vector3 camPos = cam.transform.position;

    //     int rtWidth = resolution;
    //     int rtHeight = (int)(resolution / cam.aspect);

    //     float worldToPixelX = rtWidth / (2f * halfWidth);
    //     float worldToPixelY = rtHeight / (2f * halfHeight);

    //     int px = (int)((bounds.min.x - (camPos.x - halfWidth)) * worldToPixelX);
    //     int py = (int)((bounds.min.y - (camPos.y - halfHeight)) * worldToPixelY);
    //     int pw = (int)(bounds.size.x * worldToPixelX);
    //     int ph = (int)(bounds.size.y * worldToPixelY);

    //     px = Mathf.Clamp(px, 0, rtWidth - 1);
    //     py = Mathf.Clamp(py, 0, rtHeight - 1);
    //     pw = Mathf.Clamp(pw, 1, rtWidth - px);
    //     ph = Mathf.Clamp(ph, 1, rtHeight - py);

    //     RenderTexture rt = new RenderTexture(rtWidth, rtHeight, 24);
    //     cam.targetTexture = rt;
    //     cam.Render();

    //     Texture2D tex = new Texture2D(pw, ph, TextureFormat.RGBA32, false);
    //     RenderTexture.active = rt;
    //     tex.ReadPixels(new Rect(px, py, pw, ph), 0, 0);
    //     tex.Apply();

    //     Sprite sprite = Sprite.Create(tex, new Rect(0, 0, pw, ph), new Vector2(0.5f, 0.5f));

    //     cam.targetTexture = null;
    //     RenderTexture.active = null;
    //     Destroy(rt);

    //     return sprite;
    // }
}
