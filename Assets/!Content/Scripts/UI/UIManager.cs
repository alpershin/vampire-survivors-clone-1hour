using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image xpBarFill;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private Transform upgradesContainer;
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private UpgradeCard upgradeCardPrefab;

    private ExperienceManager experienceManager;
    private PlayerController player;

    private void Start()
    {
        experienceManager = ExperienceManager.Instance;
        player = FindObjectOfType<PlayerController>();
        if (abilityManager == null)
        {
            abilityManager = FindObjectOfType<AbilityManager>();
        }

        if (xpBarFill != null)
        {
            xpBarFill.fillAmount = 0f; // ensure XP bar starts empty
        }

        if (experienceManager != null)
        {
            experienceManager.OnXpChanged += HandleXpChanged;
            experienceManager.OnLevelUp += HandleLevelUp;
        }

        if (player != null)
        {
            player.OnHealthChanged += HandleHealthChanged;
        }

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (experienceManager != null)
        {
            experienceManager.OnXpChanged -= HandleXpChanged;
            experienceManager.OnLevelUp -= HandleLevelUp;
        }

        if (player != null)
        {
            player.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(float current, float max)
    {
        if (healthBarFill != null && max > 0f)
        {
            healthBarFill.fillAmount = current / max;
        }
    }

    private void HandleXpChanged(float current, float max)
    {
        if (xpBarFill != null && max > 0f)
        {
            xpBarFill.fillAmount = current / max;
        }

        if (levelText != null)
        {
            levelText.text = $"Lv. {experienceManager.level}";
        }
    }

    private void HandleLevelUp()
    {
        if (abilityManager == null)
        {
            return;
        }

        var options = abilityManager.GetUpgradeOptions();
        if (options.Count == 0)
        {
            return;
        }

        ShowLevelUpMenu(options);
    }

    public void ShowLevelUpMenu(List<AbilityOption> options)
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        ClearUpgradeButtons();

        if (upgradeCardPrefab == null || upgradesContainer == null)
        {
            Debug.LogWarning("UpgradeCard prefab or container not set");
            return;
        }

        foreach (var option in options)
        {
            var cardObj = Instantiate(upgradeCardPrefab.gameObject, upgradesContainer);
            var card = cardObj.GetComponent<UpgradeCard>();
            if (card != null)
            {
                card.Setup(option, () => SelectUpgrade(option));
            }
        }
    }

    private void SelectUpgrade(AbilityOption data)
    {
        abilityManager?.ApplyUpgrade(data.Name);
        HideLevelUpMenu();
    }

    private void HideLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        ClearUpgradeButtons();
    }

    private void ClearUpgradeButtons()
    {
        if (upgradesContainer == null)
        {
            return;
        }

        for (int i = upgradesContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(upgradesContainer.GetChild(i).gameObject);
        }
    }
}
