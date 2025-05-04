using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject tipPanel;
    public Text tipText;

    public void ShowTip(string message)
    {
        tipText.text = message;
        tipPanel.SetActive(true);
        CancelInvoke(nameof(HideTip));
        Invoke(nameof(HideTip), 2f);
    }

    void HideTip()
    {
        tipPanel.SetActive(false);
    }
}
