using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour, IPointerDownHandler
{
    public GameObject card;
    public GameObject SummonBlock; // 高亮标识
    public GameObject AttackBlock;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (SummonBlock.activeInHierarchy)
        {
            BattleManager.Instance.SummonConfirm(transform);
        }
    }
}
