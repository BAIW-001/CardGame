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
    public PlayerData enemyData;    //数据

    public List<Card> playerDeckList = new List<Card>();
    public List<Card> enemyDeckList = new List<Card>();    //卡组

    public GameObject cardPrefad;      //卡牌

    public Transform playerHand;
    public Transform enemyHand;    //手牌

    public GameObject[] playerBlocks;
    public GameObject[] enemyBlocks;     //格子

    public GameObject playerIcon;
    public GameObject enemyIcon;    //头像

    public GamePhase GamePhase = GamePhase.gameStart;

    public UnityEvent phaseChangeEvent = new UnityEvent();

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        GameStart();
    }

    void Update()
    {
        // 可以在这里加入其他逻辑，例如监听回合结束的条件
    }

    // 游戏流程
    // 开始游戏：加载数据，卡组洗牌，初始手牌
    // 回合结束，游戏阶段

    public void GameStart()
    {
        ReadDeck(); // 读取卡组数据
        ShuffletDeck(0); // 洗玩家卡组
        ShuffletDeck(1); // 洗敌人卡组

        // 玩家和敌人各自抽卡 3 张
        DrawCard(0, 3);
        DrawCard(1, 3);

        GamePhase = GamePhase.playerDraw;
    }

    public void ReadDeck()
    {
        // 加载玩家卡组
        for (int i = 0; i < playerData.playerDeck.Length; i++)
        {
            if (playerData.playerDeck[i] != 0)
            {
                int count = playerData.playerDeck[i];
                for (int j = 0; j < count; j++)
                {
                    playerDeckList.Add(playerData.CardStore.CopyCard(i));
                }
            }
        }

        // 加载敌人卡组
        for (int i = 0; i < enemyData.playerDeck.Length; i++)
        {
            if (enemyData.playerDeck[i] != 0)
            {
                int count = enemyData.playerDeck[i];
                for (int j = 0; j < count; j++)
                {
                    enemyDeckList.Add(enemyData.CardStore.CopyCard(i));
                }
            }
        }
    }

    public void ShuffletDeck(int _player)  // 0为玩家，1为敌人
    {
        List<Card> shuffletDeck = new List<Card>();
        if (_player == 0)
        {
            shuffletDeck = playerDeckList;
        }
        else if (_player == 1)
        {
            shuffletDeck = enemyDeckList;
        }

        // 洗牌
        for (int i = 0; i < shuffletDeck.Count; i++)
        {
            int rad = Random.Range(0, shuffletDeck.Count);
            Card temp = shuffletDeck[i];
            shuffletDeck[i] = shuffletDeck[rad];
            shuffletDeck[rad] = temp;
        }
    }

    public void DrawCard(int _player, int _count)
    {
        List<Card> drawDeck = new List<Card>();
        Transform hand = transform;
        if (_player == 0)
        {
            drawDeck = playerDeckList;
            hand = playerHand;
        }
        else if (_player == 1)
        {
            drawDeck = enemyDeckList;
            hand = enemyHand;
        }

        for (int i = 0; i < _count; i++)
        {
            // 检查手牌数是否大于 6 张，如果大于6张，则不抽卡
            if (hand.childCount >= 6)
            {
                Debug.Log("手牌已满，不能再抽卡！");
                return;
            }

            // 抽卡
            GameObject card = Instantiate(cardPrefad, hand);
            card.GetComponent<CardDisplay>().card = drawDeck[0];
            drawDeck.RemoveAt(0);
        }
    }

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
            OnEnemyDraw();  // 敌方回合自动抽卡
        }
        else if (GamePhase == GamePhase.enemyAction)
        {
            GamePhase = GamePhase.playerDraw;
            phaseChangeEvent.Invoke();
            OnPlayerDraw();  // 玩家回合自动抽卡
        }
    }

    public void OnPlayerDraw()
    {
        if (GamePhase == GamePhase.playerDraw)
        {
            DrawCard(0, 1);  // 玩家自动抽卡
            GamePhase = GamePhase.playerAction;
            phaseChangeEvent.Invoke();
        }
    }

    public void OnEnemyDraw()
    {
        if (GamePhase == GamePhase.enemyDraw)
        {
            DrawCard(1, 1);  // 敌人自动抽卡
            GamePhase = GamePhase.enemyAction;
            phaseChangeEvent.Invoke();
        }
    }
}
