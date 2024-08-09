using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chatting : MonoBehaviour
{
    public InputField inputField;
    public Text chattingLog;
    public ScrollRect scrollRect;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            sendChatting();
        }
    }

    public void sendChatting() {
        if(!string.IsNullOrWhiteSpace(inputField.text)) {
            NetworkManager.instance.SendChattingPacket(inputField.text, (uint)0);
            inputField.text = "";
        }
    }

    public void updateChatting(string msg) {
        if (msg.Contains("<color=")) { //font color included
            chattingLog.text += '\n' + msg;
        } else {
            chattingLog.text += '\n' + msg;
        }
            scrollRect.verticalNormalizedPosition = 0f;
    }
}
