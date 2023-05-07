/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example code to return to previous scene
public class BattleController : MonoBehaviour
{
    public BattleSceneTransition transition;
    public GameObject player;
    private int playerHealth;
    private int playerMaxHealth;
    public static bool battleExit = false;
    private void Start()
    {
        // Set the player's health and max health based on the current scene state
        playerHealth = PlayerController.health;
        playerMaxHealth = PlayerController.maxHealth;
        Debug.Log("Player health: " + playerHealth);
        Debug.Log("Player max health: " + playerMaxHealth);
    }
    // Example method to end the battle
    public void EndBattle()
    {
        // Set the player's health and max health based on the current scene state
        PlayerController.health = playerHealth;
        PlayerController.maxHealth = playerMaxHealth;
        Camera.main.orthographicSize = 10f;
        Camera.main.GetComponent<CameraFollow>().enabled = true;
        battleExit = true;
        // Save any necessary data
        transition.ReturnToPreviousScene();
        
    }
} */

