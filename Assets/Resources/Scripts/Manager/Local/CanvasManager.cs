using System.Collections;
using TMPro;
using UnityEngine;
using Gilzoide.LottiePlayer;

public class CanvasManager : MonoBehaviour, IDataInitializable
{
    [SerializeField] private GameObject resultPanel;
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
