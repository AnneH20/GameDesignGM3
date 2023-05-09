using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelUpSystem : MonoBehaviour
{
    [SerializeField] public int currentLevel;
    [SerializeField] public int currentXP;
    [SerializeField] public int[] XPToNextLevel = new int[99];
    [SerializeField] public int maxHealth;
    [SerializeField] public int currentHealth;
    [SerializeField] public int damage;
    [SerializeField] public int defense;
    [SerializeField] private TextMeshProUGUI levelUpText;

    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel"); // Get the player's level from PlayerPrefs
        currentXP = PlayerPrefs.GetInt("PlayerXP"); // Get the player's XP from PlayerPrefs
        // Set the XP required to level up based on the player's current level
        for (int i = 0; i < XPToNextLevel.Length; i++)
        {
            XPToNextLevel[i] = 100 + ((i + currentLevel - 1) * 50); // set each level's required XP based on the player's current level
        }
        maxHealth = PlayerPrefs.GetInt("MaxHealth"); // Get the player's max health from PlayerPrefs
        currentHealth = PlayerPrefs.GetInt("Health"); // Get the player's current health from PlayerPrefs
        damage = PlayerPrefs.GetInt("Damage"); // Get the player's damage from PlayerPrefs
        defense = PlayerPrefs.GetInt("Defense"); // Get the player's defense from PlayerPrefs
    }

    private void Update() {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel"); // Get the player's level from PlayerPrefs
        currentXP = PlayerPrefs.GetInt("PlayerXP"); // Get the player's XP from PlayerPrefs
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        SavePlayerData();
    }

    public bool CheckForLevelUp()
    {
        if (currentLevel < XPToNextLevel.Length && currentXP >= XPToNextLevel[currentLevel - 1])
        {
            currentLevel++;
            // Set the XP required to level up based on the player's new level
            for (int i = currentLevel - 1; i < XPToNextLevel.Length; i++)
            {
                XPToNextLevel[i] = 100 + ((i + currentLevel - 1) * 50); // set each level's required XP based on the player's new level
            }
            // Update the player's stats based on their new level
            maxHealth += 10;
            PlayerPrefs.SetInt("MaxHealth", maxHealth);

            damage += 2;
            PlayerPrefs.SetInt("Damage", damage);

            defense += 1;
            PlayerPrefs.SetInt("Defense", defense);

            currentHealth = maxHealth;
            PlayerPrefs.SetInt("Health", currentHealth);
            /*levelUpText.text = "Congratulations, you've leveled up to level " + currentLevel + "!";
            levelUpText.gameObject.SetActive(true); // show the text
            Invoke("HideLevelUpText", 3f); // hide the text after 3 seconds*/
            SavePlayerData();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void HideLevelUpText()
    {
        levelUpText.gameObject.SetActive(false); // hide the text
    }

    private void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        PlayerPrefs.SetInt("PlayerXP", currentXP);
    }
}
