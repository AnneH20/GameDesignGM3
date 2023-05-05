using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SceneManager.GetActiveScene().name == "BattleScene")
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                SceneManager.UnloadSceneAsync("BattleScene");
                // Move the player to the new scene
                SceneManager.MoveGameObjectToScene(PlayerController.Instance.gameObject, SceneManager.GetSceneByName("Level1"));
                SceneManager.MoveGameObjectToScene(PlayerController.Instance.grid, SceneManager.GetSceneByName("Level1"));
                foreach (GameObject enemy in enemies)
                {
                    SceneManager.MoveGameObjectToScene(enemy, SceneManager.GetSceneByName("Level1"));
                    enemy.GetComponent<SpriteRenderer>().enabled = true;
                    enemy.GetComponent<AIChase>().enabled = true;
                }
                PlayerController.Instance.grid.SetActive(true);
                
            }
            else
            {
                StartCoroutine(LoadBattleScene());
            }
        }
    }
    IEnumerator LoadBattleScene()
    {
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // Wait until the scene is loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Move the player to the new scene
        SceneManager.MoveGameObjectToScene(PlayerController.Instance.gameObject, SceneManager.GetSceneByName("BattleScene"));
        SceneManager.MoveGameObjectToScene(PlayerController.Instance.grid, SceneManager.GetSceneByName("BattleScene"));
        foreach (GameObject enemy in enemies)
        {
            SceneManager.MoveGameObjectToScene(enemy, SceneManager.GetSceneByName("BattleScene"));
            enemy.GetComponent<SpriteRenderer>().enabled = false;
            enemy.GetComponent<AIChase>().enabled = false;
        }
        PlayerController.Instance.grid.SetActive(false);
        
        // Set the new scene as active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("BattleScene"));

    }
}
