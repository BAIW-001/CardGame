using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
public enum CardState
{
    Library, Deck
}

public class ClickCard : MonoBehaviour, IPointerDownHandler
{
    private DeckManager DeckManager;
    //private PlayerData PlayerData;

    public CardState state;
    // Start is called before the first frame update
    void Start()
    {
        void Start()
        {
            GameObject obj = GameObject.Find("DeckManager");
            if (obj == null)
            {
                Debug.LogWarning("DeckManager not found. Disabling ClickCard.");
                this.enabled = false;
                return;
            }

            DeckManager = obj.GetComponent<DeckManager>();
            if (DeckManager == null)
            {
                Debug.LogWarning("DeckManager script not found on DeckManager GameObject. Disabling ClickCard.");
                this.enabled = false;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        int id = this.GetComponent<CardDisplay>().card.id;
        DeckManager.UpdateCard(state, id);
    }
}