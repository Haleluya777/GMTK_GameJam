using System.Collections;
using UnityEngine;

public class PlatForm : MonoBehaviour
{
    public bool canThrough;
    private Collider2D col;
    private Collider2D playerCol;
    private bool isDropping;
    private bool isPlayerIgnored;
    public float CachedTop { get; private set; }

    void Start()
    {
        col = this.GetComponent<Collider2D>();
        playerCol = LocalGameManager.instance.playerObj.GetComponent<Collider2D>();
        RefreshCache();
    }

    [SerializeField] private float ignoreThreshold = 0.05f;

    void Update()
    {
        if (!isDropping)
        {
            float surfaceY = GetSurfaceYAtX(playerCol.bounds.center.x);
            bool shouldIgnore = playerCol.bounds.min.y < surfaceY - ignoreThreshold;

            if (shouldIgnore != isPlayerIgnored)
            {
                Physics2D.IgnoreCollision(col, playerCol, shouldIgnore);
                isPlayerIgnored = shouldIgnore;
            }
        }
    }

    public float GetSurfaceYAtX(float worldX)
    {
        if (col is BoxCollider2D box)
        {
            Vector2 localTopLeft = new Vector2(-box.size.x * 0.5f, box.size.y * 0.5f) + box.offset;
            Vector2 localTopRight = new Vector2(box.size.x * 0.5f, box.size.y * 0.5f) + box.offset;

            Vector3 worldTopLeft = transform.TransformPoint(localTopLeft);
            Vector3 worldTopRight = transform.TransformPoint(localTopRight);

            float dx = worldTopRight.x - worldTopLeft.x;
            if (Mathf.Approximately(dx, 0f))
                return worldTopLeft.y;

            float t = (worldX - worldTopLeft.x) / dx;
            t = Mathf.Clamp01(t);
            return Mathf.Lerp(worldTopLeft.y, worldTopRight.y, t);
        }

        return this.transform.GetChild(0).position.y;
    }

    public void RefreshCache()
    {
        CachedTop = col.bounds.max.y;
    }

    public void Through()
    {
        StartCoroutine(DropDown());
    }

    private IEnumerator DropDown()
    {
        isDropping = true;
        Physics2D.IgnoreCollision(col, playerCol, true);
        isPlayerIgnored = true;

        while (true)
        {
            yield return null;
            float surfaceY = GetSurfaceYAtX(playerCol.bounds.center.x);
            if (playerCol.bounds.max.y <= surfaceY)
                break;
        }

        Physics2D.IgnoreCollision(col, playerCol, false);
        isPlayerIgnored = false;
        isDropping = false;
    }

    void OnDrawGizmos()
    {
        var c = GetComponent<Collider2D>();
        if (c == null) return;
        Gizmos.color = c.enabled ? Color.green : Color.red;
        Gizmos.DrawWireCube(c.bounds.center, c.bounds.size);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(new Vector3(c.bounds.center.x, c.bounds.max.y, 0f), 0.05f);
    }
}
