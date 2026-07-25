using System.Collections;
using UnityEngine;

public class PlatForm : MonoBehaviour
{
    public bool canThrough;
    private Collider2D col;
    private Renderer playerRender;
    private bool isDropping;
    public float CachedTop { get; private set; }

    void Start()
    {
        col = this.GetComponent<Collider2D>();
        playerRender = LocalGameManager.instance.playerObj.GetComponent<Renderer>();
        RefreshCache();
    }

    void Update()
    {
        if (!isDropping)
        {
            if (playerRender.bounds.min.y < this.transform.GetChild(0).position.y)
            {
                col.enabled = false;
            }
            else
            {
                col.enabled = true;
            }
        }
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
        col.enabled = false;

        while (playerRender.bounds.max.y > this.transform.GetChild(0).position.y)
        {
            yield return null;
        }

        col.enabled = true;
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
