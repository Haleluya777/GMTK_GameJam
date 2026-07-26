using UnityEngine;
using DG.Tweening;

public class Wind : MonoBehaviour
{

    public void SetDir(float value)
    {
        this.gameObject.transform.localScale = new Vector2(value < 0 ? -1 : 1, 1);
    }

    void OnEnable()
    {
        DOVirtual.DelayedCall(10f, () => gameObject.SetActive(false));
    }

}
