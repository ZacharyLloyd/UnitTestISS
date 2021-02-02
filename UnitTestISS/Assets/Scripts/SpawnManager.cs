using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    //Reference for spawnmanager
    public static SpawnManager Instance;
    //Position that will be used when spawning
    [HideInInspector]public Transform playerLastPosition;
    //All players on level
    public List<GameObject> players = new List<GameObject>();
    //All enemies on level
    public List<GameObject> enemies = new List<GameObject>();
    //All spawn points on level
    public List<GameObject> spawnpoints = new List<GameObject>();
    //Awake is called before the anything else
    private void Awake()
    {
        #region
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }
    // Start is called before the first frame update
    void Start()
    {
        #region
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(player);
        }
       foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemy);
        }
       foreach(GameObject spawn in GameObject.FindGameObjectsWithTag("Spawn"))
        {
            spawnpoints.Add(spawn);
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
