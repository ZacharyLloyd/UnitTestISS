using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    //Ideal location to spawn
    Vector3 idealLoction = Vector3.zero;
    //Actual point player will spawn at this will be found when finding the closest spawn point to the idealLocation
    Vector3 pointToSpawn = Vector3.zero;

    public GameObject objToSpawn;

    //Vectors for calculating each prefernce point on level
    [SerializeField] Vector3 pref1Calc = Vector3.zero;
    [SerializeField] Vector3 pref2Calc = Vector3.zero;
    [SerializeField] Vector3 pref3Calc = Vector3.zero;

    //Singleton for reference to SpawnManager
    public static SpawnManager Instance;
    //Position that will be used when spawning
    [SerializeField]Vector3 playerLastPosition;

    //Variables for lists needed to track data
    #region
    [Header("Lists needed to track data")]
    //All players on level
    public List<GameObject> players = new List<GameObject>();
    //All enemies on level
    public List<GameObject> enemies = new List<GameObject>();
    //All spawn points on level
    public List<GameObject> spawnpoints = new List<GameObject>();
    //All friendly bases on level
    public List<GameObject> friendlyBase = new List<GameObject>();
    //All enemy bases on level
    public List<GameObject> enemyBase = new List<GameObject>();
    #endregion
    //Variables for sliders themselves
    #region
    [Header("UI pieces needed")]
    //Slider that will represent how far or close the player wants to spawn from enemies
    public Slider pref1;
    //Slider that will represent how far or close the player wants to spawn from a friendly base or enemy base
    public Slider pref2;
    //SLider that will represent how high or low to the ground they would like to spawn(elevation)
    public Slider pref3;
    //Importance factor for pref1
    public Slider importancePref1;
    //Importance factor for pref2
    public Slider importancePref2;
    //Importance factor for pref3
    public Slider importancePref3;
    #endregion
    //Varibles for slider values
    #region
    [Header("Variables to hold value for sliders")]
    //Value for pref1 to be saved
    public float pref1Value;
    //Value for pref2 to be saved
    public float pref2Value;
    //Value for pref3 to be saved
    public float pref3Value;
    //Importance value for pref1 to be saved
    public float importancePref1Value;
    //Importance value for pref2 to be saved
    public float importancePref2Value;
    //Importance value for pref3 to be saved
    public float importancePref3Value;
    #endregion

    //Awake is called before the anything else
    void Awake()
    {
        //Singleton set up
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
        //Adding everything to lists to access
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
       foreach(GameObject fBase in GameObject.FindGameObjectsWithTag("fBase"))
        {
            friendlyBase.Add(fBase);
        }
       foreach(GameObject eBase in GameObject.FindGameObjectsWithTag("eBase"))
        {
            enemyBase.Add(eBase);
        }
        #endregion
       
        //Assigning sliders
        #region
        pref1 = GameObject.FindGameObjectWithTag("pref1").GetComponent<Slider>();
        pref2 = GameObject.FindGameObjectWithTag("pref2").GetComponent<Slider>();
        pref3 = GameObject.FindGameObjectWithTag("pref3").GetComponent<Slider>();
        importancePref1 = GameObject.FindGameObjectWithTag("iPref1").GetComponent<Slider>();
        importancePref2 = GameObject.FindGameObjectWithTag("iPref2").GetComponent<Slider>();
        importancePref3 = GameObject.FindGameObjectWithTag("iPref3").GetComponent<Slider>();
        #endregion


        LoadValues();
    }
    //Save function for values to be stored
    public void SaveValues()
    {
        //Saving using PlayerPrefs
        #region
        pref1Value = pref1.value;
        pref2Value = pref2.value;
        pref3Value = pref3.value;
        importancePref1Value = importancePref1.value;
        importancePref2Value = importancePref2.value;
        importancePref3Value = importancePref3.value;
        PlayerPrefs.SetFloat("pref1", pref1Value);
        PlayerPrefs.SetFloat("pref2", pref2Value);
        PlayerPrefs.SetFloat("pref3", pref3Value);
        PlayerPrefs.SetFloat("importancePref1", importancePref1Value);
        PlayerPrefs.SetFloat("importancePref2", importancePref2Value);
        PlayerPrefs.SetFloat("importancePref3", importancePref3Value);
        #endregion

    }
    //Load function for values to be assigned based off last save
    public void LoadValues()
    {
        //Load using PlayerPrefs
        #region
        pref1Value = PlayerPrefs.GetFloat("pref1");
        pref2Value = PlayerPrefs.GetFloat("pref2");
        pref3Value = PlayerPrefs.GetFloat("pref3");
        importancePref1Value = PlayerPrefs.GetFloat("importancePref1");
        importancePref2Value = PlayerPrefs.GetFloat("importancePref2");
        importancePref3Value = PlayerPrefs.GetFloat("importancePref3");
        #endregion
        UpdateUI();
    }
    void UpdateUI()
    {
        //Assigning values to slider value to update UI
        #region
        pref1.value = pref1Value;
        pref2.value = pref2Value;
        pref3.value = pref3Value;
        importancePref1.value = importancePref1Value;
        importancePref2.value = importancePref2Value;
        importancePref3.value = importancePref3Value;
        #endregion
    }
    //Used to calculate the ideal spawn location
    void CalculateIdealSpawn()
    {
        //get updated player position to use
        playerLastPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        //Variables for function
        #region
        //Overall vector for enemies
        Vector3 cumulitativeVectorEnemies = Vector3.zero;
        //Average vector for enemies
        Vector3 averageVectorEnemies = Vector3.zero;
        //Number that will be used to divide cumulitativeVectorEnemies to find an average
        int numberOfEnemies = enemies.Count;
        //Overall vector for friendly bases
        Vector3 cumulitativeVectorFriendlyBase = Vector3.zero;
        //Average vector for friendly bases
        Vector3 averageVectorFriendlyBase = Vector3.zero;
        //Overall vector for enemy bases
        Vector3 cumulitativeVectorEnemyBase = Vector3.zero;
        //Average vector for enemy bases
        Vector3 averageVectorEnemyBase = Vector3.zero;
        //Number that will be used to divide cumulitativeVectorFriendlyBases to find an average
        int numberOfFriendlyBases = friendlyBase.Count;
        //Number that will be used to divide cumulitativeVectorEnemyBases to find an average
        int numberOfEnemyBases = enemyBase.Count;
        //Keep track of prevous Y value for finding highest spawn point
        float previousYValue = 0;
        //Y value to keep for elevationVector
        float yValueToKeep = 0;
        //Vector that will be used to lerp to find a calculated point on level
        Vector3 elevationVector = Vector3.zero;
        
        #endregion

        //pref1Calc
        #region
        //Foreach for all enemy positions
        foreach (GameObject enemy in enemies)
        {
            //add up all of the positions of the enemies
            cumulitativeVectorEnemies += enemy.transform.position;

        }
        //average out the vector to be used when calculating this preference and divide by the number of enemies on the level
        averageVectorEnemies = cumulitativeVectorEnemies / numberOfEnemies;
        //enemy calc
        //Lerp the players position and the average position for the enemy and go towards one or the other based on the pref1Value
        pref1Calc = Vector3.Lerp(playerLastPosition, averageVectorEnemies, pref1Value);
        #endregion

        //pref2Calc
        #region
        //Foreach for all friendly bases
        foreach (GameObject fBase in friendlyBase)
        {
            //add up all of the positions of the friendly bases
            cumulitativeVectorFriendlyBase += fBase.transform.position;
        }
        //average out the vector to be used when calculating this preference and divide by the number of friendly bases on the level
        averageVectorFriendlyBase = cumulitativeVectorFriendlyBase / numberOfFriendlyBases;
        
        //Foreach for all enemy bases
        foreach (GameObject eBase in enemyBase)
        {
            //add up all of the positions of the enemy bases
            cumulitativeVectorEnemyBase += eBase.transform.position;
        }
        //average out the vector to be used when calculating this preference and divide by the number of enemy bases on the level
        averageVectorEnemyBase = cumulitativeVectorEnemyBase / numberOfEnemyBases;
        //base calc
        //Lerp the average position for the friendly bases and the average position for the enemy bases and go towards one or the other based on the pref2Value
        pref2Calc = Vector3.Lerp(averageVectorFriendlyBase, averageVectorEnemyBase, pref2Value);
        #endregion

        //pref3Calc
        #region
        //Foreach to find the spawn point with the greatest y value
        foreach (GameObject spawnPoint in spawnpoints)
        {
            if (previousYValue <= spawnPoint.transform.position.y)
            {
                //Set the previousYValue to the spawnpoint we are cycling through
                previousYValue = spawnPoint.transform.position.y;
            }
            else
            {
                //Set the yValueToKeep for the spawn point that was found
                yValueToKeep = spawnPoint.transform.position.y;
                //Set the elevationVector
                elevationVector = new Vector3(0, yValueToKeep, 0);
            }
               
        }
        //elavation calc
        //Lerp the players position and the spawn point with the greatest y value and go towards one or the other based on the pref3Value
        pref3Calc = Vector3.Lerp(playerLastPosition, elevationVector, pref3Value );
        #endregion

        //Find the ideal location to spawn using prefCalcs multiplied by the importance values and adding them all together
        idealLoction = ((pref1Calc * importancePref1Value) + (pref2Calc * importancePref2Value) + (pref3Calc * importancePref3Value));
    }
    //Used to find the neareest spawn point to the ideal spawn found
    void CalculateNearestSpawn()
    {
        //Set a minDistance for when we try and find the closest spawn point
        float minDistance = Mathf.Infinity;
        //Foreach to cycle through all the spawnpoints to find the closest one to the idealLocation
        foreach(GameObject spawn in spawnpoints)
        {
            //Find the distance between the ideal location and the spawn point to find the closest one out of all spawn points
            float distance = Vector3.Distance(idealLoction, spawn.transform.position);
            Debug.Log(distance);
            //Is the distance we are on smaller than the previous distance
            if(distance < minDistance)
            {
                //Set the min distance to the distance of the spawn point that was compared
                minDistance = distance;
                //Set the pointToSpawn
                pointToSpawn = spawn.transform.position;
                Debug.Log($"Ideal Location is: {idealLoction} : Spawning to {pointToSpawn}");
            }
        }
    }
    //Instantiate player on the nearest spawn point to the ideal spawn found
    public void SpawnPlayer()
    {
        CalculateIdealSpawn();
        CalculateNearestSpawn();
        Instantiate(objToSpawn, pointToSpawn, Quaternion.identity);
    }
}