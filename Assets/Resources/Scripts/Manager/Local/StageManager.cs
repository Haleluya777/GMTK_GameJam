using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
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

    private int turn;

    public void DataInitialize()
    {
        unitList.AddRange(enemyList);
        unitList.Add(LocalGameManager.instance.playerObj);
        turn = 1;
        //LocalGameManager.instance.eventBus.ResetTimer?.Invoke(false, 10f);
    }

    public void AddMonster(int count)
    {
        List<Transform> positions = monsterSpawnPoint;
        for (int i = 0; i < count; i++)
        {
            Debug.Log("소!환!");
            var enemy = LocalGameManager.instance.objectPoolManager.GetGo("Enemy"); //Instantiate(enemyPrefab);
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
            LocalGameManager.instance.canvasManager.timerPanel.SetActive(false);
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
                resultList.Clear();

                /*게임 클리어 조건
                if (unitList.Count == 1 && unitList.Contains(LocalGameManager.instance.playerObj))
                {
                    Debug.Log("게임 클리어!");
                    return;
                }*/

                if (turn % 2 == 0) AddMonster(REFILL_MONSTER_COUNT);
                turn++;

                ProccessNextStage();
                Debug.Log($"남아 있는 적의 수 : {enemyList.Count}");
            }
        });
    }

    public void ProccessNextStage()
    {
        var gimmick = stageData.GetRandomGimmick();
        LocalGameManager.instance.eventBus.ResetTimer?.Invoke(false, gimmick.timerDuration);
        LocalGameManager.instance.canvasManager.timerPanel.SetActive(true);

        switch (gimmick.gimmickType)
        {
            case GimmickType.StrongWind:
                LocalGameManager.instance.eventBus.WindGimmick?.Invoke(Random.Range(-2, 3));
                GlobalGameManager.instance.soundManager.PlayWindSound();
                SetSafeZoneScale(1);
                break;

            case GimmickType.LightOff:
                LocalGameManager.instance.eventBus.LightsOffGimmick?.Invoke(gimmick.gimmickValue);
                SetSafeZoneScale(1);
                break;

            case GimmickType.SmallSafeZone:
                SetSafeZoneScale(gimmick.gimmickValue);
                break;

            case GimmickType.MovingSafeZone:
                LocalGameManager.instance.eventBus.MovingSafeZone?.Invoke(gimmick.gimmickValue);
                SetSafeZoneScale(1);
                break;

            case GimmickType.None:
                SetSafeZoneScale(1);
                break;

            case GimmickType.NoCountImage:
                LocalGameManager.instance.eventBus.NoCountImage?.Invoke();
                break;

            case GimmickType.FastCount:
                LocalGameManager.instance.eventBus.FastCount?.Invoke(gimmick.gimmickValue);
                break;

        }

        safeZone.transform.position = SetSafeZonePos(safeZone.transform.position);
        LocalGameManager.instance.eventBus.ChangeCountSound?.Invoke(gimmick.gimmickType);
    }

    public void MoveSafeZone(float duration)
    {
        DOTween.Sequence()
        .AppendInterval(2f)
        .Append(safeZone.transform.DOMove(SetSafeZonePos(safeZone.transform.position), duration))
        .AppendInterval(2f)
        .Append(safeZone.transform.DOMove(SetSafeZonePos(safeZone.transform.position), duration))
        .AppendInterval(2f)
        .Append(safeZone.transform.DOMove(SetSafeZonePos(safeZone.transform.position), duration))
        .AppendInterval(2.5f);
    }

    private void SetSafeZoneScale(float value)
    {
        safeZone.transform.localScale = new Vector2(value, value);
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
