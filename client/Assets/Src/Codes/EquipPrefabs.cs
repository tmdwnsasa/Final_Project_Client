using UnityEngine;
using UnityEngine.UI;

public class EquipPrefabs : MonoBehaviour
{
    public Image slotImage; // 슬롯 이미지

    public void SetSlotImage(Sprite newImage)
    {
        if (slotImage != null)
        {
            slotImage.sprite = newImage;
            Debug.Log("Equipped slot image set to: " + newImage.name);

            // Ensure the image is enabled and active
            slotImage.enabled = true;
            slotImage.gameObject.SetActive(true);

            // Keep Back0 visible and on the background
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
}