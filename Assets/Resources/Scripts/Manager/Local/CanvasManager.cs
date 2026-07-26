using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class CanvasManager : MonoBehaviour, IDataInitializable
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private RectTransform charImg;
    [SerializeField] private RectTransform clearImg;
    [SerializeField] private RectTransform credit;
    [SerializeField] private TextMeshProUGUI tmp;

    public GameObject timerPanel;

    private const string GAME_OVER_TXT = "Game Over";

    public void DataInitialize()
    {

    }

    public void GameOverUI()
    {
        Debug.Log("게임오버.");
        resultPanel.SetActive(true);
        StartCoroutine(TypingText());
    }

    public void GameClearUI()
    {
        Sequence seq = DOTween.Sequence();

        Debug.Log("게임 클리어.");
        clearPanel.SetActive(true);

        seq.Append(clearImg.DOAnchorPosY(0, .7f).SetEase(Ease.OutQuad));
        seq.Append(charImg.DOAnchorPosY(0, .7f).SetEase(Ease.OutQuad));
        seq.Append(credit.DOAnchorPosY(0, .7f).SetEase(Ease.OutQuad));

        LocalGameManager.instance.DisableInput();
    }

    public void LaserDirection()
    {
        //animPlayer.Play();
    }

    private IEnumerator TypingText()
    {
        tmp.text = "";
        foreach (var c in GAME_OVER_TXT)
        {
            tmp.text += c;
            yield return new WaitForSeconds(.05f);
        }
    }
}
