using UnityEngine;
using UnityEngine.EventSystems;

public enum BattleCardState
{
    inHand, inBlock
}

public class BattleCard : MonoBehaviour, IPointerDownHandler
{
    public int playerID;
    public BattleCardState state = BattleCardState.inHand;

    public int AttackCount;
    public int attackCount;

    public Card card; // 当前绑定的逻辑卡片数据

    public void OnPointerDown(PointerEventData eventData)
    {
        if (card is MonsterCard)
        {
            if (state == BattleCardState.inHand)
            {
                BattleManager.Instance.SummonRequest(playerID, gameObject);
            }
            else if (state == BattleCardState.inBlock && attackCount > 0)
            {
                BattleManager.Instance.AttackRequst(playerID, gameObject);
            }
        }
    }

    public void ResetAttack()
    {
        attackCount = AttackCount;
    }

    public void Init(Card c, int player)
    {
        card = c;
        playerID = player;

        if (card is MonsterCard monster)
        {
            AttackCount = monster.attackTime;
            attackCount = AttackCount;
        }
        else
        {
            AttackCount = 0;
            attackCount = 0;
        }
    }
}
