using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using Gilzoide.LottiePlayer;

public class StageManager : MonoBehaviour, IDataInitializable
{
    public List<GameObject> enemyList = new List<GameObject>();
    public List<GameObject> unitList = new List<GameObject>();

    [SerializeField] private StageData stageData;
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject safeZone;
    [SerializeField] private List<Transform> monsterSpawnPoint;
    [SerializeField] private List<Transform> safeZonePoint;

    private Sequence directionSequence;
    private bool result = true;
    private const int REFILL_MONSTER_COUNT = 2;

    public void DataInitialize()
    {
        unitList.AddRange(enemyList);
        unitList.Add(LocalGameManager.instance.playerObj);
    }

    public void AddMonster(int count)
    {
        List<Transform> positions = monsterSpawnPoint;
        for (int i = 0; i < count; i++)
        {
            Debug.Log("소!환!");
            var enemy = Instantiate(enemyPrefab);
            enemyList.Add(enemy);

            Transform _position = positions[Random.Range(0, positions.Count)];
            positions.Remove(_position);

            enemy.transform.position = _position.position;
        }
    }

    public void StageResultDirection()
    {
        Sequence seq = DOTween.Sequence();
        List<GameObject> resultList = null;

        seq.AppendCallback(() =>
        {
            var lottie = fadePanel.GetComponent<ImageLottiePlayer>();
            lottie.DOFade(1f, .1f);
            lottie.Play();
        });

        seq.AppendInterval(2f);
        seq.AppendCallback(() =>
        {
            resultList = LocalGameManager.instance.captureManager.CheckCapture();
            List<GameObject> outsideFrame = unitList.FindAll((a) => !resultList.Contains(a));
            foreach (var child in outsideFrame)
            {
                child.GetComponent<Units>().Dead();
            }

            unitList.Clear();
            unitList.AddRange(resultList);

            enemyList.Clear();
            foreach (var child in resultList)
            {
                if (child.tag == "Player") continue;
                enemyList.Add(child);
            }

            result = unitList.Contains(LocalGameManager.instance.playerObj);
        });

        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            if (!result)
            {
                LocalGameManager.instance.GameOver();
            }
            else
            {
                /*게임 클리어 조건
                resultList.Clear();
                if (unitList.Count == 1 && unitList.Contains(LocalGameManager.instance.playerObj))
                {
                    Debug.Log("게임 클리어!");
                    return;
                }*/

                //AddMonster(REFILL_MONSTER_COUNT);
                ProccessNextStage();
                Debug.Log($"남아 있는 적의 수 : {enemyList.Count}");
            }
        });
    }

    public void ProccessNextStage()
    {
        var gimmick = stageData.GetRandomGimmick();
        switch (gimmick.gimmickType)
        {
            case GimmickType.StrongWind:
                GlobalGameManager.instance.eventBus.WindGimmick?.Invoke(gimmick.gimmickValue);
                break;

            case GimmickType.LightOff:
                GlobalGameManager.instance.eventBus.LightsOffGimmick?.Invoke(gimmick.gimmickValue);
                break;
        }

        safeZone.transform.position = SetSafeZonePos(safeZone.transform.position);
        GlobalGameManager.instance.eventBus.ResetTimer?.Invoke(false, gimmick.timerDuration);
    }

    private Vector2 SetSafeZonePos(Vector2 currentPos)
    {
        Vector2 next;
        do
        {
            next = safeZonePoint[Random.Range(0, safeZonePoint.Count)].position; ;
        }
        while (next == currentPos);
        return next;
    }
}
