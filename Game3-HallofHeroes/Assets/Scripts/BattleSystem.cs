using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;
	public GameObject bossPrefab, bossPrefab2, bossPrefab3, bossPrefab4, bossPrefab5;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;
	
	Unit playerUnit;
	Unit enemyUnit;
	private InventoryScript playerInventory;
	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

	public GameObject itemMenu;
	private float potionChance, superPotionChance, hyperPotionChance, maxPotionChance;
	public Button button;
	public Button closeButton;
	private LevelUpSystem levelUpSystem;
	public BattleSceneTransition transition;
    public static bool battleExit = false;

	public AudioSource attackSound;
	public AudioSource loseSound;

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		// Set the player's inventory based on the current scene state
		playerInventory = PlayerController.Instance.playerInventory;
		levelUpSystem = PlayerController.Instance.GetComponent<LevelUpSystem>();
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();
		// Set the player's stats on the current scene state
		playerUnit.currentHP = PlayerPrefs.GetInt("Health");
		playerUnit.maxHP = PlayerPrefs.GetInt("MaxHealth");
		playerUnit.damage = PlayerPrefs.GetInt("Damage");
		playerUnit.baseDefense = PlayerPrefs.GetInt("Defense");
		Debug.Log("Player health: " + playerUnit.currentHP);
		Debug.Log("Player max health: " + playerUnit.maxHP);
		// If the player is fighting the boss, spawn the boss prefab
		if (PlayerController.Instance.isBoss)
		{
			PlayerPrefs.SetInt("Boss Dead", 1);
			potionChance = playerInventory.inventory.items.Find(item => item.itemName == "Potion").itemChance = 1f; // 100% chance to drop a potion
			superPotionChance = playerInventory.inventory.items.Find(item => item.itemName == "Super Potion").itemChance = 0.5f; // 50% chance to drop a super potion
			hyperPotionChance = playerInventory.inventory.items.Find(item => item.itemName == "Hyper Potion").itemChance = 0.25f; // 25% chance to drop a hyper potion
			maxPotionChance = playerInventory.inventory.items.Find(item => item.itemName == "Max Potion").itemChance = 0.1f; // 10% chance to drop a max potion
			if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level1"))
			{
				GameObject enemyGO = Instantiate(bossPrefab, enemyBattleStation);
				enemyUnit = enemyGO.GetComponent<Unit>();
				enemyUnit.xpGiven = 200;
			}
			if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level2"))
			{
				GameObject enemyGO = Instantiate(bossPrefab2, enemyBattleStation);
				enemyUnit = enemyGO.GetComponent<Unit>();
				enemyUnit.xpGiven = 400;
			}
			if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level3"))
			{
				GameObject enemyGO = Instantiate(bossPrefab3, enemyBattleStation);
				enemyUnit = enemyGO.GetComponent<Unit>();
				enemyUnit.xpGiven = 600;
			}
			if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level4"))
			{
				GameObject enemyGO = Instantiate(bossPrefab4, enemyBattleStation);
				enemyUnit = enemyGO.GetComponent<Unit>();
				enemyUnit.xpGiven = 800;
			}
			if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level5"))
			{
				GameObject enemyGO = Instantiate(bossPrefab5, enemyBattleStation);
				enemyUnit = enemyGO.GetComponent<Unit>();
			}
			
		}
		else
		{
			potionChance = playerInventory.inventory.items.Find(item => item.itemName == "Potion").itemChance = 0.5f; // 50% chance to drop a potion
			superPotionChance = playerInventory.inventory.items.Find(item => item.itemName == "Super Potion").itemChance = 0.25f; // 25% chance to drop a super potion
			hyperPotionChance = playerInventory.inventory.items.Find(item => item.itemName == "Hyper Potion").itemChance = 0.1f; // 10% chance to drop a hyper potion
			maxPotionChance = playerInventory.inventory.items.Find(item => item.itemName == "Max Potion").itemChance = 0.05f; // 5% chance to drop a max potion
			GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
			enemyUnit = enemyGO.GetComponent<Unit>();
			enemyUnit.xpGiven = 100;
		}
		dialogueText.text = "A " + enemyUnit.unitName + " approaches...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);
		GameObject.Find("Items").GetComponent<Button>().interactable = false;
		yield return new WaitForSeconds(2f);
		

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack()
	{
		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

		attackSound.Play();
		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "You attacked the enemy! " + enemyUnit.unitName + " took " + playerUnit.damage + " damage.";
		GameObject.Find("AttackButton").GetComponent<Button>().interactable = false;
		GameObject.Find("Items").GetComponent<Button>().interactable = false;
		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
			if (PlayerController.Instance.isBoss)
			{
				PlayerPrefs.SetInt("Boss Dead", 1);
			}
			EndBattle();
		} else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn()
	{
		dialogueText.text = enemyUnit.unitName + " attacks!";

		yield return new WaitForSeconds(1f);

		attackSound.Play();
		bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(1f);
	

		if(isDead)
		{
			state = BattleState.LOST;
			EndBattle();
		} else
		{
			state = BattleState.PLAYERTURN;
			GameObject.Find("AttackButton").GetComponent<Button>().interactable = true;
			PlayerTurn();
		}

	}

	public void EndBattle()
	{
		PlayerController.Instance.isBoss = false;
		// Set the player's health and max health based on the current scene state
		PlayerPrefs.SetInt("Health", playerUnit.currentHP);
		PlayerPrefs.SetInt("MaxHealth", playerUnit.maxHP);
		PlayerPrefs.SetInt("Damage", playerUnit.damage);
		PlayerPrefs.SetInt("Defense", playerUnit.baseDefense);
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
			dialogueText.text += "\nYou gained " + enemyUnit.xpGiven + " XP!";
			levelUpSystem.AddXP(enemyUnit.xpGiven);
			battleExit = true;
			StartCoroutine(PostBattle());
		} else if (state == BattleState.LOST)
		{
			PlayerController.isDead = true;
			BattleSceneTransition.battleActive = false;
			GameObject.Find("BGM").SetActive(false);
			loseSound.Play();
			dialogueText.text = "You were defeated.";
		}
	}

	void ReturnScene()
	{
		transition.ReturnToPreviousScene();
	}

	void PlayerTurn()
	{
		
		dialogueText.text = "Choose an action:";
		UpdateItemMenu();
	}

	IEnumerator PostBattle()
	{
		yield return new WaitForSeconds(2f);
		if (levelUpSystem.CheckForLevelUp())
		{
			dialogueText.text = "You leveled up to " + levelUpSystem.currentLevel + "! Your stats have increased!";
			yield return new WaitForSeconds(2f);
		}
		if (UnityEngine.Random.Range(0f, 1f) <= potionChance)
		{
			
			playerInventory.inventory.items.Find(item => item.itemName == "Potion").itemAmount++;
			dialogueText.text = " You found a potion!";
			yield return new WaitForSeconds(2f);
		}
		if (UnityEngine.Random.Range(0f, 1f) <= superPotionChance)
		{	
			playerInventory.inventory.items.Find(item => item.itemName == "Super Potion").itemAmount++;
			dialogueText.text = " You found a super potion!";
			yield return new WaitForSeconds(2f);
		}
		if (UnityEngine.Random.Range(0f, 1f) <= hyperPotionChance)
		{
			playerInventory.inventory.items.Find(item => item.itemName == "Hyper Potion").itemAmount++;
			dialogueText.text = " You found a hyper potion!";
			yield return new WaitForSeconds(2f);
		}
		if (UnityEngine.Random.Range(0f, 1f) <= maxPotionChance)
		{
			playerInventory.inventory.items.Find(item => item.itemName == "Max Potion").itemAmount++;
			dialogueText.text = " You found a max potion!";
			yield return new WaitForSeconds(2f);
		}
		// Transition to the previous scene
		ReturnScene();
	}
	public void OnAttackButton()
	{

		if (state != BattleState.PLAYERTURN)
		{
			return;
		}
		else if (state == BattleState.PLAYERTURN)
		{
			StartCoroutine(PlayerAttack());
		}	
	}

	void UpdateItemMenu()
    {
		Vector2 firstButtonPosition = new Vector2(-164.7f, 57.43f); // adjust this value as needed
		float buttonSpacing = 10f; // adjust this value as needed
		int buttonIndex = 0;
		GameObject.Find("Items").GetComponent<Button>().interactable = true;

        // Add a button for each item in the inventory.
        foreach (InventoryScript.Item item in playerInventory.inventory.items)
        {
			Button newButton = Instantiate(button, itemMenu.transform);
			newButton.GetComponentInChildren<Text>().text = item.itemName + " x" + item.itemAmount;

			// Set the position of the new button based on the index of existing buttons
			RectTransform buttonTransform = newButton.GetComponent<RectTransform>();
			Vector2 newPosition = firstButtonPosition + new Vector2(0f, -buttonIndex * buttonSpacing);
			buttonTransform.anchoredPosition = newPosition;
			newButton.onClick.AddListener(() => UseItem(item));
			buttonIndex++;
        }

        // Add a button to close the item menu.
        closeButton.GetComponentInChildren<Text>().text = "Close";
        closeButton.onClick.AddListener(() => itemMenu.SetActive(false));
    }

    public void UseItem(InventoryScript.Item item)
    {
        if (item.itemEffect > 0 && item.itemAmount > 0)
        {
            // If the item has a positive effect, heal the player.
            playerUnit.Heal(item.itemEffect);
            playerHUD.SetHP(playerUnit.currentHP);
			// Remove the item from the inventory.
        	playerInventory.inventory.items.Find(i => i.itemName == item.itemName).itemAmount--;
            dialogueText.text = "Used " + item.itemName + " to heal " + item.itemEffect + " HP.";
			Debug.Log("Used " + item.itemName + " to heal " + item.itemEffect + " HP.");
        }
        else if (item.itemEffect < 0 && item.itemAmount > 0)
        {
            // If the item has a negative effect, damage the enemy.
            bool isDead = enemyUnit.TakeDamage(-item.itemEffect);
            enemyHUD.SetHP(enemyUnit.currentHP);
			// Remove the item from the inventory.
        	playerInventory.inventory.items.Find(i => i.itemName == item.itemName).itemAmount--;
            dialogueText.text = "Used " + item.itemName + " to deal " + (-item.itemEffect) + " damage.";

            if (isDead)
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
		else
		{
			dialogueText.text = "You don't have any " + item.itemName + "s left.";
		}
        // Update the item menu.
        UpdateItemMenu();
    }

}
