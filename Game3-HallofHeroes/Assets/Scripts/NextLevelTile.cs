using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTile : MonoBehaviour
{
    [SerializeField] private string nextLevelName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }

    protected void LoadNextLevel()
    {
        LevelManager.Instance.LoadScene(nextLevelName);
    }
}

