﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class Item
{
    public string itemName;
    public string itemDescription;
    public int itemEffect; // For example, the amount of health to heal the player or damage to deal to the enemy.
}

public class Inventory
{
    public List<Item> items = new List<Item>();
}
public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

	public GameObject itemMenu;
	public Button button;
	public Button closeButton;

    private Inventory inventory = new Inventory();

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		dialogueText.text = "A " + enemyUnit.unitName + " approaches...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);
	
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
		GameObject.Find("ItemButton").GetComponent<Button>().interactable = false;
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
			GameObject.Find("ItemButton").GetComponent<Button>().interactable = true;
			PlayerTurn();
		}

	}

	void EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
			PlayerController.health = playerUnit.currentHP;
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
		}
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
		inventory.items.Add(new Item { itemName = "Potion", itemDescription = "Heals 10 HP.", itemEffect = 10 });
        // Clear null item buttons.
        foreach (Transform child in itemMenu.transform)
        {
			Item item = inventory.items.Find(i => i.itemName == child.GetComponentInChildren<Text>().text);
			if (item == null && child.gameObject != closeButton.gameObject)
				Destroy(child.gameObject);
		}

        // Add a button for each item in the inventory.
        foreach (Item item in inventory.items)
        {
			Button newButton = Instantiate(button, itemMenu.transform);
            newButton.GetComponentInChildren<Text>().text = item.itemName;
			newButton.gameObject.SetActive(true);
            newButton.onClick.AddListener(() => UseItem(item));
        }

        // Add a button to close the item menu.
        closeButton.GetComponentInChildren<Text>().text = "Close";
        closeButton.onClick.AddListener(() => itemMenu.SetActive(false));
    }

    void UseItem(Item item)
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
        inventory.items.Remove(item);

        // Update the item menu.
        UpdateItemMenu();

        // End the player's turn.
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

}
