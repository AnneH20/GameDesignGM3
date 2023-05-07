﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

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
	public Button button;
	public Button closeButton;

	public BattleSceneTransition transition;
    public static bool battleExit = false;

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		// Set the player's inventory based on the current scene state
		playerInventory = PlayerController.Instance.playerInventory;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();
		// Set the player's stats on the current scene state
		playerUnit.currentHP = PlayerController.health;
		playerUnit.maxHP = PlayerController.maxHealth;
		playerUnit.damage = PlayerController.damage;
		playerUnit.baseDefense = PlayerController.defense;
		Debug.Log("Player health: " + playerUnit.currentHP);
		Debug.Log("Player max health: " + playerUnit.maxHP);

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

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

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "The attack is successful!";
		GameObject.Find("AttackButton").GetComponent<Button>().interactable = false;
		GameObject.Find("Items").GetComponent<Button>().interactable = false;
		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
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

	void EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
			// Set the player's health and max health based on the current scene state
			PlayerController.health = playerUnit.currentHP;
			PlayerController.maxHealth = playerUnit.maxHP;
			playerInventory.inventory.items.Find(item => item.itemName == "Potion").itemAmount += 1;
			battleExit = true;
			// Transition to the previous scene
			Invoke(nameof(ReturnScene), 2f);
		} else if (state == BattleState.LOST)
		{
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
		//playerInventory.inventory.items.Add(new InventoryScript.Item { itemName = "Potion", itemDescription = "Heals 10 HP.", itemEffect = 10, itemAmount = 0 });
		//playerInventory.inventory.items.Add(new InventoryScript.Item { itemName = "Super Potion", itemDescription = "Heals 20 HP.", itemEffect = 20, itemAmount = 0 });

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

    void UseItem(InventoryScript.Item item)
    {
        if (item.itemEffect > 0)
        {
            // If the item has a positive effect, heal the player.
            playerUnit.Heal(item.itemEffect);
            playerHUD.SetHP(playerUnit.currentHP);
            dialogueText.text = "Used " + item.itemName + " to heal " + item.itemEffect + " HP.";
        }
        else if (item.itemEffect < 0)
        {
            // If the item has a negative effect, damage the enemy.
            bool isDead = enemyUnit.TakeDamage(-item.itemEffect);
            enemyHUD.SetHP(enemyUnit.currentHP);
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

        // Remove the item from the inventory.
        playerInventory.inventory.items.Remove(item);

        // Update the item menu.
        UpdateItemMenu();

        // End the player's turn.
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

}
