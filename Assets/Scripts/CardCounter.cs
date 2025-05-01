using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCounter : MonoBehaviour
{
    public Text counterText;
    private int counter = 0;

    public bool SetCounter(int _value, CardState cardState)
    {
        counter += _value;
        OnCounterChange();

        // 当数量为 0 时，在 library 中禁用交互，但在 deck 中销毁卡牌对象
        if (counter == 0)
        {
            if (cardState == CardState.Library)
            {
                // 禁用交互，不销毁对象
                DisableCardInteractions();
            }
            else if (cardState == CardState.Deck)
            {
                // 在 Deck 中，数量为 0 时销毁对象
                Destroy(gameObject);
                return false;  // 返回 false 表示卡牌已销毁
            }
        }
        else
        {
            // 启用交互（如果有）
            EnableCardInteractions();
        }

        return true;
    }

    private void OnCounterChange()
    {
        counterText.text = counter.ToString();
    }

    private void DisableCardInteractions()
    {
        // 禁用 Button 或 Collider 防止卡牌被点击
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = false;
        }
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    private void EnableCardInteractions()
    {
        // 启用 Button 或 Collider，使卡牌可以被点击
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
        }
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }
}
