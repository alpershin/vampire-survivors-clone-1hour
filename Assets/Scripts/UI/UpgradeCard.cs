using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button button;

    public void Setup(AbilityOption option, Action onClick)
    {
        if (nameText != null)
        {
            nameText.text = option.Name;
        }

        if (descriptionText != null)
        {
            descriptionText.text = string.IsNullOrWhiteSpace(option.Description)
                ? "No description"
                : option.Description;
        }

        if (levelText != null)
        {
            string levelLabel = option.IsNew
                ? "Unlock"
                : $"Lvl {option.CurrentLevel} → {option.NextLevel}";
            levelText.text = levelLabel;
        }

        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}
