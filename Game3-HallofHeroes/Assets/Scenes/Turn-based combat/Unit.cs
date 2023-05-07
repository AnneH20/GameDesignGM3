using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

	public string unitName;

	public int damage;
	public int baseDefense;
	public int bonusDefense;
	public int maxHP;
	public int currentHP;

	public bool TakeDamage(int dmg)
	{
		// Calculate damage after defense
		int totalDefense = baseDefense + bonusDefense; // Calculate total defense
		float damageReduction = Mathf.Clamp01((float)totalDefense / (totalDefense + 300f)); // Calculate damage reduction based on total defense
		int actualDamage = Mathf.Max(damage - Mathf.RoundToInt(damage * damageReduction), 1); // Calculate actual damage after defense
		
		currentHP -= actualDamage;
		// Check if unit is dead
		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

}
