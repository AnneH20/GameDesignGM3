using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTile : MonoBehaviour
{
    [SerializeField] private string nextLevelName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LoadNextLevel();
            PlayerController.Instance.gameObject.transform.position = new Vector2(0, 0);
        }
    }

    protected void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }
}

