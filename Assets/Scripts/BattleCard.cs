using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BattleCardState
{
    inHand,inBlock
}

public class BattleCard : MonoBehaviour,IPointerDownHandler
{
    public int playerID;
    public BattleCardState state = BattleCardState.inHand;
    public void OnPointerDown(PointerEventData eventData)
    {
        //当在手牌点击时，发起召唤请求
        if(GetComponent<CardDisplay>().card is MonsterCard)
        {
            if (state == BattleCardState.inHand)
            {
                BattleManager.Instance.SummonRequest(playerID, gameObject);
            }
            
        }
        

        //当在场上点击时，发起攻击请求
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
