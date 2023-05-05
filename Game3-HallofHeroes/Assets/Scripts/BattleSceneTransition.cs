using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneState
{
    public int playerHealth;
    public int playerMaxHealth;
    public List<Tilemap> tilemaps;
    public List<TileData> tileDataList;
}
[System.Serializable]
public class TileData
{
    public string tilemapName;
    public Vector3Int position;
    public Sprite sprite;
}

public class BattleSceneTransition : MonoBehaviour
{
    public static BattleSceneTransition instance;
    [SerializeField] public SceneState previousSceneState;
    [SerializeField] private Tilemap floorTilemap, wallTilemap, nextLevelTilemap;
    // Save the current scene state when transitioning to a battle scene
    public void TransitionToBattleScene()
    {
        previousSceneState = new SceneState();
        previousSceneState.playerHealth = PlayerController.health;
        previousSceneState.playerMaxHealth = PlayerController.maxHealth;
        previousSceneState.tilemaps = new List<Tilemap>();
        previousSceneState.tilemaps.Add(floorTilemap);
        previousSceneState.tilemaps.Add(wallTilemap);
        previousSceneState.tilemaps.Add(nextLevelTilemap);

        previousSceneState.tileDataList = new List<TileData>();

        foreach (Tilemap tilemap in previousSceneState.tilemaps)
        {
            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(position);
                if (tile != null)
                {
                    TileData tileData = new TileData();
                    tileData.tilemapName = tilemap.name;
                    tileData.position = position;
                    tileData.sprite = tilemap.GetSprite(position);
                    previousSceneState.tileDataList.Add(tileData);
                }
            }
        }
        
        string json = JsonUtility.ToJson(previousSceneState);
        File.WriteAllText(Application.persistentDataPath + "/previousSceneState.json", json);
        PlayerPrefs.SetString("PreviousSceneName", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        Debug.Log("Saving previous scene: " + PlayerPrefs.GetString("PreviousSceneName"));
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().grid.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }

    // Return to the previous scene when the battle is over
    public void ReturnToPreviousScene()
    {
        string previousSceneStateJson = File.ReadAllText(Application.persistentDataPath + "/previousSceneState.json");
        if (!string.IsNullOrEmpty(previousSceneStateJson))
        {
            previousSceneState = JsonUtility.FromJson<SceneState>(previousSceneStateJson);
            PlayerController.health = previousSceneState.playerHealth;
            PlayerController.maxHealth = previousSceneState.playerMaxHealth;
            
            foreach (TileData tileData in previousSceneState.tileDataList)
            {
                Tilemap tilemap = previousSceneState.tilemaps.Find(t => t.name == tileData.tilemapName);
                TileBase tile = tilemap.GetTile(tileData.position);
                if (tile != null)
                {
                    tilemap.SetTile(tileData.position, ScriptableObject.CreateInstance<Tile>());
                    tilemap.SetTile(tileData.position, tile);
                }
            }
        }
        Resources.FindObjectsOfTypeAll<PlayerController>()[0].grid.SetActive(true);
        string previousSceneName = PlayerPrefs.GetString("PreviousSceneName");
        Debug.Log("Loading previous scene: " + previousSceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(previousSceneName);
    }
}