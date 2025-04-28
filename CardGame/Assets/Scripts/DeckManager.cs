using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public Transform deckPanel;
    public Transform libraryPanel;

    public GameObject deckPrefab;
    public GameObject cardPrefab;

    public GameObject DataManager;

    private PlayerData PlayerData;
    private CardStore CardStore;

    private Dictionary<int, GameObject> libraryDic = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> deckDic = new Dictionary<int, GameObject>();

    void Start()
    {
        PlayerData = DataManager.GetComponent<PlayerData>();
        CardStore = DataManager.GetComponent<CardStore>();
        UpdateLibrary();
        UpdateDeck();
    }

    void Update()
    {
    }

    // 更新 Library 中的卡牌
    public void UpdateLibrary()
    {
        for (int i = 0; i < PlayerData.playerCards.Length; i++)
        {
            if (PlayerData.playerCards[i] > 0)
            {
                CreateCard(i, CardState.Library);
            }
            else if (libraryDic.ContainsKey(i))
            {
                // 保持已经存在的卡牌，即使数量为0，也不销毁它
                libraryDic[i].GetComponent<CardCounter>().SetCounter(0, CardState.Library);
            }
        }
    }

    // 更新 Deck 中的卡牌
    public void UpdateDeck()
    {
        for (int i = 0; i < PlayerData.playerDeck.Length; i++)
        {
            if (PlayerData.playerDeck[i] > 0)
            {
                CreateCard(i, CardState.Deck);
            }
            else
            {
                // 如果 Deck 中的卡牌数量为 0，销毁卡牌
                if (deckDic.ContainsKey(i))
                {
                    GameObject deckCard = deckDic[i];
                    if (deckCard != null)
                    {
                        Destroy(deckCard);  // 销毁卡牌对象
                        deckDic.Remove(i);  // 从字典中移除
                    }
                }
            }
        }
    }

    // 更新卡牌的数量
    public void UpdateCard(CardState _state, int _id)
    {
        if (_state == CardState.Deck)
        {
            // 如果 Deck 中卡牌数量大于 0，减少 Deck 卡牌数量，并增加 Library 中的卡牌
            if (PlayerData.playerDeck[_id] > 0)
            {
                PlayerData.playerDeck[_id]--;
                PlayerData.playerCards[_id]++;

                // 更新 Deck 中的卡牌显示
                if (deckDic.ContainsKey(_id))
                {
                    GameObject deckCard = deckDic[_id];
                    if (deckCard != null)
                    {
                        if (!deckCard.GetComponent<CardCounter>().SetCounter(-1, CardState.Deck)) // 如果数量减少到 0，移除
                        {
                            Destroy(deckCard);  // 销毁卡牌对象
                            deckDic.Remove(_id);  // 从字典中移除
                        }
                    }
                }

                // 更新 Library 中的卡牌显示
                if (libraryDic.ContainsKey(_id))
                {
                    GameObject libraryCard = libraryDic[_id];
                    if (libraryCard != null)
                    {
                        libraryCard.GetComponent<CardCounter>().SetCounter(1, CardState.Library);
                    }
                }
                else
                {
                    CreateCard(_id, CardState.Library);
                }
            }
        }
        else if (_state == CardState.Library)
        {
            // 当 Library 中卡牌的数量大于 0 时，允许减少
            if (PlayerData.playerCards[_id] > 0)
            {
                PlayerData.playerCards[_id]--;
                PlayerData.playerDeck[_id]++;

                // 更新 Library 中的卡牌显示
                if (libraryDic.ContainsKey(_id))
                {
                    GameObject libraryCard = libraryDic[_id];
                    if (libraryCard != null)
                    {
                        libraryCard.GetComponent<CardCounter>().SetCounter(-1, CardState.Library);
                    }
                }

                // 更新 Deck 中的卡牌显示
                if (deckDic.ContainsKey(_id))
                {
                    GameObject deckCard = deckDic[_id];
                    if (deckCard != null)
                    {
                        deckCard.GetComponent<CardCounter>().SetCounter(1, CardState.Deck);
                    }
                }
                else
                {
                    CreateCard(_id, CardState.Deck);
                }
            }
            else
            {
                // 如果 Library 中卡牌数量已经为 0，就不允许再减少，只能增加
                Debug.Log("Cannot reduce card count in library. Only increase allowed.");
            }
        }
    }

    // 创建卡牌
    public void CreateCard(int _id, CardState _cardState)
    {
        Transform targetPanel;
        GameObject targetPrefab;
        var refData = PlayerData.playerCards;
        Dictionary<int, GameObject> targetDic = libraryDic;

        if (_cardState == CardState.Library)
        {
            targetPanel = libraryPanel;
            targetPrefab = cardPrefab;
        }
        else
        {
            targetPanel = deckPanel;
            targetPrefab = deckPrefab;
            refData = PlayerData.playerDeck;
            targetDic = deckDic;
        }

        GameObject newCard = Instantiate(targetPrefab, targetPanel);
        newCard.GetComponent<CardCounter>().SetCounter(refData[_id], _cardState);
        newCard.GetComponent<CardDisplay>().card = CardStore.cardList[_id];
        targetDic.Add(_id, newCard);
    }
}
