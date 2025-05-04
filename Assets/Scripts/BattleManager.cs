using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 游戏阶段：
/// gameStart：开局初始化阶段
/// playerDraw：玩家抽卡阶段
/// playerAction：玩家行动阶段
/// enemyDraw：敌人抽卡阶段
/// enemyAction：敌人行动阶段
/// </summary>
public enum GamePhase
{
    gameStart, playerDraw, playerAction, enemyDraw, enemyAction
}

public class BattleManager : MonoSingleton<BattleManager>
{
    public PlayerData playerData;
    public PlayerData enemyData;

    public List<Card> playerDeckList = new List<Card>();
    public List<Card> enemyDeckList = new List<Card>();

    public GameObject cardPrefad;

    public Transform playerHand;
    public Transform enemyHand;

    public GameObject[] playerBlocks;
    public GameObject[] enemyBlocks;

    public GameObject playerIcon;
    public GameObject enemyIcon;

    public GamePhase GamePhase = GamePhase.gameStart;

    public UnityEvent phaseChangeEvent = new UnityEvent();

    public int[] SummonCountMax = new int[2];      // 每回合最大召唤次数
    private int[] SummonCounter = new int[2];      // 当前回合剩余召唤次数

    private GameObject waitingMonster;
    private int waitingPlayer;

    void Start()
    {
        GameStart();
    }

    // 开始游戏：加载卡组、洗牌、初始抽卡，进入玩家行动阶段（不再抽额外卡）
    public void GameStart()
    {
        ReadDeck();
        ShuffletDeck(0);
        ShuffletDeck(1);

        DrawCard(0, 3);
        DrawCard(1, 3);

        GamePhase = GamePhase.playerAction;
        SummonCounter[0] = SummonCountMax[0];
        phaseChangeEvent.Invoke();
    }

    /// <summary>
    /// 从 PlayerData 中读取卡组生成副本
    /// </summary>
    public void ReadDeck()
    {
        for (int i = 0; i < playerData.playerDeck.Length; i++)
        {
            int count = playerData.playerDeck[i];
            for (int j = 0; j < count; j++)
                playerDeckList.Add(playerData.CardStore.CopyCard(i));
        }

        for (int i = 0; i < enemyData.playerDeck.Length; i++)
        {
            int count = enemyData.playerDeck[i];
            for (int j = 0; j < count; j++)
                enemyDeckList.Add(enemyData.CardStore.CopyCard(i));
        }
    }

    public void ShuffletDeck(int _player)
    {
        List<Card> deck = _player == 0 ? playerDeckList : enemyDeckList;
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(0, deck.Count);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }

    /// <summary>
    /// 抽卡逻辑：检测卡组是否为空/手牌是否满
    /// </summary>
    public void DrawCard(int _player, int _count)
    {
        List<Card> deck = _player == 0 ? playerDeckList : enemyDeckList;
        Transform hand = _player == 0 ? playerHand : enemyHand;

        for (int i = 0; i < _count; i++)
        {
            if (hand.childCount >= 6)
            {
                UIManager.Instance.ShowTip("手牌已满，不能再抽卡！");
                return;
            }

            if (deck.Count == 0)
            {
                UIManager.Instance.ShowTip("卡组为空，无法抽卡！");
                return;
            }

            GameObject cardObj = Instantiate(cardPrefad, hand);
            cardObj.GetComponent<CardDisplay>().card = deck[0];
            cardObj.GetComponent<BattleCard>().playerID = _player;
            deck.RemoveAt(0);
        }
    }

    /// <summary>
    /// 回合结束按钮：只能在玩家或敌方行动阶段按
    /// </summary>
    public void OnClickTurnEnd()
    {
        TurnEnd();
    }

    public void TurnEnd()
    {
        if (GamePhase == GamePhase.playerAction)
        {
            GamePhase = GamePhase.enemyDraw;
            phaseChangeEvent.Invoke();
            OnEnemyDraw();
        }
        else if (GamePhase == GamePhase.enemyAction)
        {
            GamePhase = GamePhase.playerDraw;
            phaseChangeEvent.Invoke();
            OnPlayerDraw();
        }
    }

    public void OnPlayerDraw()
    {
        if (GamePhase != GamePhase.playerDraw) return;

        DrawCard(0, 1);
        SummonCounter[0] = SummonCountMax[0];
        GamePhase = GamePhase.playerAction;
        phaseChangeEvent.Invoke();
    }

    public void OnEnemyDraw()
    {
        if (GamePhase != GamePhase.enemyDraw) return;

        DrawCard(1, 1);
        SummonCounter[1] = SummonCountMax[1];
        GamePhase = GamePhase.enemyAction;
        phaseChangeEvent.Invoke();
    }

    /// <summary>
    /// 召唤请求，仅在己方回合且有召唤次数且有空格子时生效
    /// </summary>
    public void SummonRequest(int _player, GameObject _monster)
    {
        if (!IsPlayerTurn(_player))
        {
            UIManager.Instance.ShowTip("不是你的回合，无法召唤！");
            return;
        }

        if (SummonCounter[_player] <= 0)
        {
            UIManager.Instance.ShowTip("本回合召唤次数已用完！");
            return;
        }

        GameObject[] blocks = _player == 0 ? playerBlocks : enemyBlocks;
        bool hasEmptyBlock = false;

        foreach (var block in blocks)
        {
            if (block.GetComponent<Block>().card == null)
            {
                block.GetComponent<Block>().SummonBlock.SetActive(true);
                hasEmptyBlock = true;
            }
        }

        if (hasEmptyBlock)
        {
            waitingMonster = _monster;
            waitingPlayer = _player;
        }
        else
        {
            UIManager.Instance.ShowTip("没有空的格子可以召唤！");
        }
    }

    /// <summary>
    /// 从Block确认召唤
    /// </summary>
    public void SummonConfirm(Transform _block)
    {
        Summon(waitingPlayer, waitingMonster, _block);

        GameObject[] blocks = waitingPlayer == 0 ? playerBlocks : enemyBlocks;
        foreach (var block in blocks)
            block.GetComponent<Block>().SummonBlock.SetActive(false);
    }

    public void Summon(int _player, GameObject _monster, Transform _block)
    {
        _monster.transform.SetParent(_block);
        _monster.transform.localPosition = Vector3.zero;
        _monster.GetComponent<BattleCard>().state = BattleCardState.inBlock;
        _block.GetComponent<Block>().card = _monster;
        SummonCounter[_player]--;
        UIManager.Instance.ShowTip("召唤成功！");
    }

    /// <summary>
    /// 判断是否是当前玩家的回合
    /// </summary>
    public bool IsPlayerTurn(int _player)
    {
        return (_player == 0 && GamePhase == GamePhase.playerAction) ||
               (_player == 1 && GamePhase == GamePhase.enemyAction);
    }
}
