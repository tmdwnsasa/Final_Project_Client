using UnityEngine;
using UnityEngine.UI;

public class EquipPrefabs : MonoBehaviour
{
    public Image slotImage; // 슬롯 이미지
    public Text itemNameText;

    public void SetSlotImage(Sprite newImage)
    {
        if (slotImage != null)
        {
            slotImage.sprite = newImage;
            //Debug.Log("Equipped slot image set to: " + newImage.name);

            slotImage.enabled = true;
            slotImage.gameObject.SetActive(true);

            foreach (Transform child in transform)
            {
                Image imageComponent = child.GetComponent<Image>();
                if (imageComponent != null && child.name != "Back0")
                {
                    if (imageComponent.sprite == newImage)
                    {
                        imageComponent.gameObject.SetActive(true);  // Enable only the selected sprite
                    }
                    else
                    {
                        imageComponent.gameObject.SetActive(false); // Disable others
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Slot image is not assigned in SlotPrefabs.");
        }
    }

    public void SetItemName(string itemSpriteName)
    {
        if (itemNameText != null)
        {
            itemNameText.text = itemSpriteName;
            Debug.Log("Item name set to: " + itemSpriteName);
        }
        else
        {
            Debug.LogError("Item name text component is not assigned in EquipPrefabs.");
        }
    }
}