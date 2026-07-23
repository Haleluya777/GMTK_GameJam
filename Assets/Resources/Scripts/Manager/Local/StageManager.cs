using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;

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
            if (result) //쉘터 내부 진입 성공
            {
                foreach (var child in enemyList)
                {
                    child.SetActive(false);
                }
            }
            else //진입 실패. 게임 오버.
            {

            }
        });
        seq.Append(fadePanel.DOFade(0f, 0.3f));

        if (!result) //게임 오버.
        {
            seq.AppendInterval(.25f);

        }
    }
}
