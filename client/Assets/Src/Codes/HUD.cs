using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { PlayerId, Time }
    public InfoType type;

    Text myText;

    void Awake() {
        myText = GetComponent<Text>();
    }

    void LateUpdate() {
        switch(type) {
            case InfoType.PlayerId:
                myText.text = string.Format("{0}", GameManager.instance.playerId);
                break;
            case InfoType.Time:
                int min = Mathf.FloorToInt(GameManager.instance.gameTime / 60);
                int sec = Mathf.FloorToInt(GameManager.instance.gameTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
        }
    }
}
