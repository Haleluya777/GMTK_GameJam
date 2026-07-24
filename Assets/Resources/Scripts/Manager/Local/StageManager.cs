using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class StageManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyList = new List<GameObject>();

    public Image fadePanel;
    private Sequence directionSequence;

    public void StageResultDirection(bool result)
    {
        PlaySequence(result);
    }

    private void PlaySequence(bool result)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(fadePanel.DOFade(1f, 0.7f));
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            foreach (var child in enemyList)
            {
                child.SetActive(false);
            }
        });
        seq.Append(fadePanel.DOFade(0f, 0.3f));

        if (!result) //게임 오버.
        {
            seq.AppendInterval(.25f);
            seq.AppendCallback(() => LocalGameManager.instance.canvasManager.GameOver());
        }
    }
}
