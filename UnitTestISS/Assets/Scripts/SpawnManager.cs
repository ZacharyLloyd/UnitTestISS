using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    //Ideal position for player to spawn which will be used to find the neariest spawn point to this location
    [SerializeField] Transform idealSpawnLocation;
    //Actual spawn point the player will be instantiated on
    [SerializeField] Transform pointToSpawnOn;

    //May get removed
    Vector3 idealLoction;
    Vector3 sumOf2Calcs;
    float totalImportance;


    [SerializeField] Vector3 pref1Calc;
    [SerializeField] Vector3 pref2Calc;
    [SerializeField] Vector3 pref3Calc;

    //May get removed
    public GameObject objectToSpawn;
    public GameObject hishestSpawn;


    //Singleton for reference to SpawnManager
    public static SpawnManager Instance;
    //Position that will be used when spawning
    public Transform playerLastPosition;
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
        CalculateIdealSpawn();

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
        //enemy calc
        pref1Calc = Vector3.Lerp(playerLastPosition.position, enemies[0].gameObject.transform.position, pref1Value );
        //base calc
        pref2Calc = Vector3.Lerp(friendlyBase[0].gameObject.transform.position, enemyBase[0].gameObject.transform.position, pref2Value );
        //elavation calc
        pref3Calc = Vector3.Lerp(playerLastPosition.position, hishestSpawn.transform.position, pref3Value ) ;

        //This may get removed
        totalImportance = importancePref1Value + importancePref2Value + importancePref3Value;
        
        //This may get removed
        sumOf2Calcs = Vector3.Lerp(pref1Calc, pref2Calc, totalImportance);
        idealLoction = Vector3.Lerp(sumOf2Calcs, pref3Calc, totalImportance);

        //Instantiate(objectToSpawn, pref1Calc, Quaternion.identity);
        //Instantiate(objectToSpawn, pref2Calc, Quaternion.identity);
        //Instantiate(objectToSpawn, pref3Calc, Quaternion.identity);
        //Instantiate(objectToSpawn, idealLoction, Quaternion.identity);

        Debug.Log($"{pref1Calc},\n{pref2Calc},\n{pref3Calc}\n{sumOf2Calcs}\n{idealLoction}");

        //Debug.DrawLine(playerLastPosition.position, enemies[0].gameObject.transform.position, Color.green, 10f);
        //Debug.DrawLine(friendlyBase[0].gameObject.transform.position, enemyBase[0].gameObject.transform.position, Color.red, 10f);
        //Debug.DrawLine(playerLastPosition.position, hishestSpawn.transform.position, Color.blue, 10f);
        //Debug.DrawLine(pref1Calc, pref2Calc, Color.yellow, 10f);
        //Debug.DrawLine(sumOf2Calcs, pref3Calc, Color.gray, 10f);
    }
    //Used to find the neareest spawn point to the ideal spawn found
    void CalculateNearestSpawn()
    {

    }
    //Instantiate player on the nearest spawn point to the ideal spawn found
    void SpawnPlayer()
    {

    }
}