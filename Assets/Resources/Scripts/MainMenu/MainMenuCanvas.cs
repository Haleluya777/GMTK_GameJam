using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class MainMenuCanvas : MonoBehaviour
{
    private Sequence seq;

    [SerializeField] private RectTransform enemyChar;
    [SerializeField] private RectTransform playerChar;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject loadingScreen;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        asyncOp.allowSceneActivation = false;

        while (!asyncOp.isDone)
        {
            Debug.Log($"로딩 진행률: {asyncOp.progress * 100f:F1}%");

            if (asyncOp.progress >= 0.9f)
            {
                asyncOp.allowSceneActivation = true;
            }
            yield return null;
        }
    }


    void Start()
    {
        seq = DOTween.Sequence();
        seq.Append(playerChar.DORotateQuaternion(Quaternion.Euler(0, 0, 0), .5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            enemyChar.DOAnchorPosX(0, 1f).OnComplete(() => startButton.SetActive(true));
        }));
    }
}
