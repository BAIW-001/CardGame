using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GamePhase
{
    gameStart, playerDraw, playerAction, enemyDraw, enemyAction
}

public class BattleManager : MonoSingleton<BattleManager>
{
    public static BattleManager Instance;

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

    public int[] SummonCountMax = new int[2];
    private int[] SummonCounter = new int[2];

    private GameObject waitingMonster;
    private int waitingPlayer = -1;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameStart();
    }

    public void GameStart()
    {
        ReadDeck();
        ShuffletDeck(0);
        ShuffletDeck(1);

        DrawCard(0, 3);
        DrawCard(1, 3);

        GamePhase = GamePhase.playerDraw;

        SummonCounter = (int[])SummonCountMax.Clone(); // 深拷贝
    }

    public void ReadDeck()
    {
        for (int i = 0; i < playerData.playerDeck.Length; i++)
        {
            if (playerData.playerDeck[i] > 0)
            {
                for (int j = 0; j < playerData.playerDeck[i]; j++)
                {
                    playerDeckList.Add(playerData.CardStore.CopyCard(i));
                }
            }
        }

        for (int i = 0; i < enemyData.playerDeck.Length; i++)
        {
            if (enemyData.playerDeck[i] > 0)
            {
                for (int j = 0; j < enemyData.playerDeck[i]; j++)
                {
                    enemyDeckList.Add(enemyData.CardStore.CopyCard(i));
                }
            }
        }
    }

    public void ShuffletDeck(int _player)
    {
        List<Card> deck = (_player == 0) ? playerDeckList : enemyDeckList;

        for (int i = 0; i < deck.Count; i++)
        {
            int r = Random.Range(0, deck.Count);
            (deck[i], deck[r]) = (deck[r], deck[i]);
        }
    }

    public void DrawCard(int _player, int _count)
    {
        List<Card> drawDeck = (_player == 0) ? playerDeckList : enemyDeckList;
        Transform hand = (_player == 0) ? playerHand : enemyHand;

        for (int i = 0; i < _count; i++)
        {
            if (drawDeck.Count == 0)
            {
                Debug.LogWarning((_player == 0 ? "玩家" : "敌人") + "卡组已空，无法抽卡");
                return;
            }

            if (hand.childCount >= 6)
            {
                Debug.Log((_player == 0 ? "玩家" : "敌人") + "手牌已满！");
                return;
            }

            GameObject card = Instantiate(cardPrefad, hand);
            card.GetComponent<CardDisplay>().card = drawDeck[0];
            card.GetComponent<BattleCard>().playerID = _player;
            drawDeck.RemoveAt(0);
        }
    }

    public void OnClickTurnEnd()
    {
        TurnEnd();
    }

    public void TurnEnd()
    {
        // 清除未完成的召唤状态
        CancelSummonState();

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
        if (GamePhase == GamePhase.playerDraw)
        {
            DrawCard(0, 1);
            SummonCounter[0] = SummonCountMax[0];
            GamePhase = GamePhase.playerAction;
            phaseChangeEvent.Invoke();
        }
    }

    public void OnEnemyDraw()
    {
        if (GamePhase == GamePhase.enemyDraw)
        {
            DrawCard(1, 1);
            SummonCounter[1] = SummonCountMax[1];
            GamePhase = GamePhase.enemyAction;
            phaseChangeEvent.Invoke();
        }
    }

    public void SummonRequest(int _player, GameObject _monster)
    {
        if (waitingMonster != null)
        {
            Debug.Log("已有卡牌等待召唤，忽略新的召唤请求。");
            return;
        }

        GameObject[] blocks = (_player == 0) ? playerBlocks : enemyBlocks;
        bool hasEmpty = false;

        if (SummonCounter[_player] > 0)
        {
            foreach (var block in blocks)
            {
                if (block.GetComponent<Block>().card == null)
                {
                    block.GetComponent<Block>().SummonBlock.SetActive(true);
                    hasEmpty = true;
                }
            }
        }

        if (hasEmpty)
        {
            waitingMonster = _monster;
            waitingPlayer = _player;
            Debug.Log("等待召唤：" + _monster.name);
        }
        else
        {
            Debug.Log("无法召唤：没有空格或次数不足！");
        }
    }

    public void SummonConfirm(Transform _block)
    {
        if (waitingMonster == null)
        {
            Debug.LogWarning("没有等待召唤的卡牌！");
            return;
        }

        Summon(waitingPlayer, waitingMonster, _block);

        GameObject[] blocks = (waitingPlayer == 0) ? playerBlocks : enemyBlocks;
        foreach (var block in blocks)
        {
            block.GetComponent<Block>().SummonBlock.SetActive(false);
        }

        waitingMonster = null;
        waitingPlayer = -1;
    }

    public void Summon(int _player, GameObject _monster, Transform _block)
    {
        _monster.transform.SetParent(_block);
        _monster.transform.localPosition = Vector3.zero;
        _monster.GetComponent<BattleCard>().state = BattleCardState.inBlock;
        _block.GetComponent<Block>().card = _monster;
        SummonCounter[_player]--;
        Debug.Log("召唤成功：" + _monster.name);
    }

    public void CancelSummonState()
    {
        waitingMonster = null;
        waitingPlayer = -1;

        foreach (var block in playerBlocks)
        {
            block.GetComponent<Block>().SummonBlock.SetActive(false);
        }
        foreach (var block in enemyBlocks)
        {
            block.GetComponent<Block>().SummonBlock.SetActive(false);
        }
    }
}
