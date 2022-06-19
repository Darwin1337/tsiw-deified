using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject myPrefab;
    public GameObject trampolimPrefab;
    public List<GameObject> obstacles;
    public List<GameObject> spawnList;
    public List<GameObject> colectables;

    public List<GameObject> spawnListObstacles;
    public List<GameObject> spawnedPrefabList;

    [SerializeField] float platformsSpeed = 2f;
    public bool isGameOver = false;
    private float curXCoord;
    private float satelliteHeight;

    private bool isPaused = false;
    public bool shouldSpawnFirstTwo = false;

    private System.DateTime lastColectableSpawned = System.DateTime.Now;

    // Start is called before the first frame update
    void Start()
    {
        spawnList.Add(Instantiate(myPrefab));
        spawnObstacles(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Random.Range(1, 100) <= 10)
        {
            // 10% de chance de spawnar um colectável
            // Mas só spawnar o último spawn tiver sido há mais de 15 segundos
            if ((System.DateTime.Now - lastColectableSpawned).TotalSeconds > 15)
            {
                Collider[] hitColliders = Physics.OverlapSphere(new Vector3(-11.5f, 0f, -.5f), 1);
                GameObject targetPlatform = spawnList[0];
                if (hitColliders.Length > 0 && spawnList.Count > 1)
                {
                    if (spawnList[1].GetInstanceID() == hitColliders[0].gameObject.transform.parent.gameObject.GetInstanceID())
                        targetPlatform = spawnList[1];
                }
                Instantiate(colectables[UnityEngine.Random.Range(0, colectables.Count)], new Vector3(-11.5f, 1f, -.5f), Quaternion.identity, targetPlatform.transform);
                lastColectableSpawned = System.DateTime.Now;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused)
            {
                Time.timeScale = 0f;
                isPaused = true;
            }
            else
            {
                Time.timeScale = 1f;
                isPaused = false;
            }
        }

        if (!isGameOver)
        {
            for (int i = 0; i < spawnList.Count; i++)
            {
                spawnList[i].transform.position = new Vector3(spawnList[i].transform.position.x + (platformsSpeed * Time.deltaTime), spawnList[i].transform.position.y, spawnList[i].transform.position.z);
            }

            if (spawnList[0].transform.position.x >= .2f)
            {
                if (spawnList.Count == 1)
                {
                    spawnList.Add(Instantiate(myPrefab, new Vector3(spawnList[0].transform.position.x - 27.43f, spawnList[0].transform.position.y, spawnList[0].transform.position.z), Quaternion.identity));
                    spawnObstacles(spawnList.Count - 1);
                }
            }

            for (int i = 0; i < spawnListObstacles.Count; i++)
            {
                if (spawnListObstacles[i].transform.position.x >= 4.25f)
                {
                    GameObject obs = Instantiate(spawnedPrefabList[i], new Vector3(-15f, spawnedPrefabList[i].name != "Nothingness" ? spawnList[0].transform.GetChild(0).position.y + (spawnList[0].transform.GetChild(1).GetComponent<Collider>().bounds.size.y / 2) + (spawnListObstacles[i].transform.GetComponent<Renderer>().bounds.size.y / 2) : 0f, spawnedPrefabList[i].name == "Satellite" ? 0f : -.5f), Quaternion.identity, spawnList[spawnList.Count - 1].transform);
                    if (obs.gameObject.name == "SatelliteDish(Clone)")
                    {
                        obs.transform.Rotate(-29.24f, 90f, 0f);
                        obs.transform.position = new Vector3(-15, spawnList[0].transform.GetChild(0).position.y + (spawnList[0].transform.GetChild(1).GetComponent<Collider>().bounds.size.y / 2) + (satelliteHeight / 2), -.5f);
                    }
                    else if (obs.gameObject.name == "Satellite(Clone)")
                        obs.transform.Rotate(0f, 90f, 0f);

                    if (UnityEngine.Random.Range(1, 100) <= 15)
                    {
                        // 15% de chance de aparecer um trampolim à frente do obstáculo
                        if (spawnedPrefabList[i].name != "Nothingness")
                        {
                            GameObject trampolim = Instantiate(trampolimPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, spawnList[spawnList.Count - 1].transform);
                            trampolim.transform.position = new Vector3(-13f, spawnList[0].transform.GetChild(0).position.y + (spawnList[0].transform.GetChild(1).GetComponent<Collider>().bounds.size.y / 2) + (trampolim.transform.GetChild(0).GetComponent<Renderer>().bounds.size.y / 2), -.5f);
                        }
                    }
                    Destroy(spawnListObstacles[i]);
                    spawnedPrefabList.RemoveAt(i);
                    spawnListObstacles.RemoveAt(i);
                    break;
                }
            }

            if (spawnList[0].transform.position.x >= 18f)
            {
                Destroy(spawnList[0]);
                spawnList[0] = spawnList[1];
                spawnList.RemoveAt(1);
            }
        }
    }

    private void spawnObstacles(int idx)
    {
        curXCoord = spawnList[idx].transform.position.x - (8.5f + 1.5f);
        int rndIdx;
        float platformHeight, obstacleHeight;
        for (int i = 0; i < 4; i++)
        {
            if (!(!shouldSpawnFirstTwo && i == 3))
            {
                rndIdx = UnityEngine.Random.Range(0, obstacles.Count);
                spawnedPrefabList.Add(obstacles[rndIdx]);
                spawnListObstacles.Add(Instantiate(obstacles[rndIdx], new Vector3(0f, 0f, 0f), Quaternion.identity, spawnList[idx].transform));
                if (obstacles[rndIdx].name == "SatelliteDish")
                {
                    spawnListObstacles[spawnListObstacles.Count - 1].transform.Rotate(29.24f, 90f, 180f);
                    satelliteHeight = spawnListObstacles[spawnListObstacles.Count - 1].transform.GetComponent<Collider>().bounds.size.y;
                }
                else if (obstacles[rndIdx].name == "Satellite")
                    spawnListObstacles[spawnListObstacles.Count - 1].transform.Rotate(0f, 90f, 0f);
                else if (obstacles[rndIdx].name == "SpaceDebris")
                    spawnListObstacles[spawnListObstacles.Count - 1].transform.Rotate(-180f, 0f, 0f);
                platformHeight = spawnList[idx].transform.GetChild(1).GetComponent<Collider>().bounds.size.y;
                obstacleHeight = spawnListObstacles[spawnListObstacles.Count - 1].transform.GetComponent<Collider>().bounds.size.y;
                spawnListObstacles[spawnListObstacles.Count - 1].transform.position = new Vector3(curXCoord, spawnList[idx].transform.GetChild(1).position.y - ((platformHeight / 2) + (obstacleHeight / 2)), obstacles[rndIdx].name == "Satellite" ? 0f : -.5f);
            }

            if (i < 1)
                curXCoord += (8.5f / 2) + 1.5f;
            else if (i > 1)
                curXCoord = (spawnList[idx].transform.position.x + 1.5f) + (((8.5f / 2) + 1.5f) * (i - 1));
            else if (i == 1)
                curXCoord = spawnList[idx].transform.position.x + 1.5f;
        }
        if (shouldSpawnFirstTwo)
        {
            rndIdx = UnityEngine.Random.Range(0, obstacles.Count);
            spawnedPrefabList.Add(obstacles[rndIdx]);
            spawnListObstacles.Add(Instantiate(obstacles[rndIdx], new Vector3(0f, 0f, 0f), Quaternion.identity, spawnList[idx].transform));
            if (obstacles[rndIdx].name == "SatelliteDish")
            {
                spawnListObstacles[spawnListObstacles.Count - 1].transform.Rotate(29.24f, 90f, 180f);
                satelliteHeight = spawnListObstacles[spawnListObstacles.Count - 1].transform.GetComponent<Collider>().bounds.size.y;
            }
            else if (obstacles[rndIdx].name == "Satellite")
                spawnListObstacles[spawnListObstacles.Count - 1].transform.Rotate(0f, 90f, 0f);
            else if (obstacles[rndIdx].name == "SpaceDebris")
                spawnListObstacles[spawnListObstacles.Count - 1].transform.Rotate(-180f, 0f, 0f);
            platformHeight = spawnList[idx].transform.GetChild(1).GetComponent<Collider>().bounds.size.y;
            obstacleHeight = spawnListObstacles[spawnListObstacles.Count - 1].transform.GetComponent<Collider>().bounds.size.y;
            spawnListObstacles[spawnListObstacles.Count - 1].transform.position = new Vector3(curXCoord, spawnList[idx].transform.GetChild(1).position.y - ((platformHeight / 2) + (obstacleHeight / 2)), obstacles[rndIdx].name == "Satellite" ? 0f : -.5f);
        }
        shouldSpawnFirstTwo = true;
    }
}
