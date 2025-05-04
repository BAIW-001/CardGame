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

    public void OnPointerDown(PointerEventData eventData)
    {
        // 只有手牌中的怪物卡可以召唤
        if (state == BattleCardState.inHand &&
            GetComponent<CardDisplay>().card is MonsterCard)
        {
            BattleManager.Instance.SummonRequest(playerID, gameObject);
        }
    }
}
