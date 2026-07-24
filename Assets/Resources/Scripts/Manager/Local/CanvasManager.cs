using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI tmp;
    private const string GAME_OVER_TXT = "Game Over";

    public void GameOver()
    {
        resultPanel.SetActive(true);
        StartCoroutine(TypingText());
    }

    private IEnumerator TypingText()
    {
        tmp.text = "";
        foreach (var c in GAME_OVER_TXT)
        {
            tmp.text += c;
        }

        yield return new WaitForSeconds(.2f);
    }
}
