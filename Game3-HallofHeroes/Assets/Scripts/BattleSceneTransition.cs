using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Pathfinding;

public class BattleSceneTransition : MonoBehaviour
{
    public static BattleSceneTransition instance;
    public static bool battleActive = false;
    // Save the current scene state when transitioning to a battle scene
    public void TransitionToBattleScene()
    {
       StartCoroutine(LoadBattleScene());
    }

    // Return to the previous scene when the battle is over
    public void ReturnToPreviousScene()
    {
        if (SceneManager.GetActiveScene().name == "BryansBattleScene")
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                GameObject boss = GameObject.FindGameObjectWithTag("Boss");
                SceneManager.UnloadSceneAsync("BryansBattleScene");
                // Move the player to the new scene
                foreach (GameObject enemy in enemies)
                {
                    enemy.GetComponent<SpriteRenderer>().enabled = true;
                    battleActive = false;
                }
                if (boss != null)
                {
                    boss.GetComponent<SpriteRenderer>().enabled = true;
                    battleActive = false;
                }
                PlayerController.Instance.gameObject.SetActive(true);
                PlayerController.Instance.grid.SetActive(true);
                PlayerController.Instance.sceneCamera.SetActive(true);
                
            }
    }
    

    IEnumerator LoadBattleScene()
    {
        yield return new WaitForEndOfFrame();
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BryansBattleScene", LoadSceneMode.Additive);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        // Wait until the scene is loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Move the player to the new scene
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<SpriteRenderer>().enabled = false;
            battleActive = true;
        }
        if (boss != null)
        {
            boss.GetComponent<SpriteRenderer>().enabled = false;
            battleActive = true;
        }
        PlayerController.Instance.grid.SetActive(false);
        PlayerController.Instance.gameObject.SetActive(false);
        PlayerController.Instance.sceneCamera.SetActive(false);
        
        // Set the new scene as active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("BryansBattleScene"));

    }
}
