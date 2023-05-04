using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneState
{
    public int playerHealth;
}

public class BattleSceneTransition : MonoBehaviour
{
    private SceneState previousSceneState;
    public SceneState currentSceneState;

    // Save the current scene state when transitioning to a battle scene
    public void TransitionToBattleScene()
    {
        previousSceneState = currentSceneState;
        previousSceneState.playerHealth = PlayerController.health;
        // Save any other relevant data here
        PlayerPrefs.SetString("PreviousSceneState", JsonUtility.ToJson(previousSceneState));
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }

    // Load the previous scene state when returning from a battle scene
    public void ReturnToPreviousScene()
    {
        string previousSceneStateJson = PlayerPrefs.GetString("PreviousSceneState");
        if (!string.IsNullOrEmpty(previousSceneStateJson))
        {
            previousSceneState = JsonUtility.FromJson<SceneState>(previousSceneStateJson);
            currentSceneState.playerHealth = previousSceneState.playerHealth;
            // Load any other relevant data here
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("PreviousSceneName");
    }
}
