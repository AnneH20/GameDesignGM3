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
		if(baseDefense < dmg)
			currentHP -= (dmg - baseDefense);
		else
			currentHP -= 1;

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
