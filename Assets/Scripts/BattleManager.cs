using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private GameObject attackingMonster;
    public GameObject attackArrow;

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

            // 实例化卡牌预制体并设置为手牌子物体
            GameObject cardObj = Instantiate(cardPrefad, hand);

            // 取出卡组顶牌
            Card cardData = deck[0];

            // 设置 CardDisplay 中的 card 属性
            cardObj.GetComponent<CardDisplay>().card = cardData;

            // 初始化 BattleCard 信息（包含攻击次数、playerID 等）
            BattleCard battleCard = cardObj.GetComponent<BattleCard>();
            battleCard.Init(cardData, _player);

            // 从卡组中移除
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

        MonsterCard mc = _monster.GetComponent<CardDisplay>().card as MonsterCard;
        _monster.GetComponent<BattleCard>().AttackCount = mc.attackTime;
        _monster.GetComponent<BattleCard>().ResetAttack();
    }

    /// <summary>
    /// 发起攻击请求（当前玩家、攻击者）
    /// </summary>
    public void AttackRequst(int _player, GameObject _monster)
    {
        if (!IsPlayerTurn(_player))
        {
            UIManager.Instance.ShowTip("不是你的回合，无法攻击！");
            return;
        }

        BattleCard attackerCard = _monster.GetComponent<BattleCard>();

        if (attackerCard.state != BattleCardState.inBlock)
        {
            UIManager.Instance.ShowTip("该卡牌不在战斗区域，不能攻击！");
            return;
        }

        // 启动攻击指示器（例如箭头）
        attackArrow.SetActive(true);
        attackArrow.transform.position = _monster.transform.position;

        attackingMonster = _monster;
        UIManager.Instance.ShowTip("请选择一个敌方目标进行攻击！");
    }

    /// <summary>
    /// 选择目标并确认攻击
    /// block 参数来自被点击的目标格子
    /// </summary>
    public void AttackConfirm(Transform _targetBlock)
    {
        if (attackingMonster == null)
        {
            UIManager.Instance.ShowTip("未选择攻击者！");
            return;
        }

        GameObject targetCard = _targetBlock.GetComponent<Block>().card;
        if (targetCard == null)
        {
            UIManager.Instance.ShowTip("目标格子为空，无法攻击！");
            return;
        }

        // 停用箭头
        attackArrow.SetActive(false);

        // 执行攻击
        Attack(attackingMonster, targetCard);

        // 攻击后清空攻击者
        attackingMonster = null;
    }

    /// <summary>
    /// 攻击逻辑
    /// </summary>
    public void Attack(GameObject attacker, GameObject target)
    {
        BattleCard atkCard = attacker.GetComponent<BattleCard>();
        BattleCard tgtCard = target.GetComponent<BattleCard>();

        int damage = 0;  // 在方法开始时定义 damage

        // 确保攻击者和目标卡片是 MonsterCard 类型
        if (atkCard.card is MonsterCard atkMonster && tgtCard.card is MonsterCard tgtMonster)
        {
            damage = atkMonster.attack;  // 使用 MonsterCard 的 attack

            tgtMonster.healthPoint -= damage;  // 使用 MonsterCard 的 healthPoint
        }
        else
        {
            Debug.LogWarning("卡片类型不匹配，无法攻击！");
        }

        // 显示攻击信息
        UIManager.Instance.ShowTip($"{atkCard.card.cardName} 对 {tgtCard.card.cardName} 造成 {damage} 点伤害！");

        // 如果目标死亡，销毁目标卡牌
        if (tgtCard.card is MonsterCard tgtMonsterDeath && tgtMonsterDeath.healthPoint <= 0)
        {
            UIManager.Instance.ShowTip($"{tgtCard.card.cardName} 被击败！");
            target.transform.SetParent(null);
            Destroy(target);

            // 清空所在格子的引用
            Transform block = target.transform.parent;
            if (block != null)
            {
                block.GetComponent<Block>().card = null;
            }
        }

        // TODO：如果攻击者只能攻击一次，可以加入状态限制
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
