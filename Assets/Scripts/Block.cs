using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Block : MonoBehaviour,IPointerDownHandler
{
    public GameObject card;
    public GameObject SummonBlock;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (SummonBlock.activeInHierarchy)
        {
            BattleManager.Instance.SummonConfirm(transform);
        }
       
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
