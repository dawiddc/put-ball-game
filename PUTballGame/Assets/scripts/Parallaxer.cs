﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{

    class PoolObject
    {
        public Transform transform;
        public bool inUse;
        public PoolObject(Transform t) { transform = t; }
        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }
    }

    [System.Serializable]
    public struct XSpawnRange
    {
        public float minX;
        public float maxX;
    }

    public GameObject Prefab;
    public int poolSize;
    public static float shiftSpeed;
    public static float spawnRate;

    public XSpawnRange xSpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate;
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    PoolObject[] poolObjects;
    float targetAspect;
    GameManager game;

    void Awake()
    {
        Configure();
    }

    public static void ConfigureDifficulty()
    {
        if (GameManager.gameDifficulty == GameManager.GameDifficulty.Easy)
        {
            shiftSpeed = -1.5f;
            spawnRate = 1.1f;
        } else if (GameManager.gameDifficulty == GameManager.GameDifficulty.Medium)
        {
            shiftSpeed = -2.0f;
            spawnRate = 0.8f;
        } else
        {
            shiftSpeed = -2.5f;
            spawnRate = 0.65f;
        }
    }

    void Start()
    {
        game = GameManager.Instance;
    }

    void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        Configure();
    }

    private bool startSpawn = false;

    void Update()
    {
        if (game.GameOver) return;

        if (!startSpawn)
        {
            Spawn();
            startSpawn = true;
        }

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Debug.Log(shiftSpeed + ", " + spawnRate);
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure()
    {
        //spawning pool objects
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Spawn()
    {
        //moving pool objects into place
        Transform t = GetPoolObject();
        if (t == null)
        {
            return;
        }
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(xSpawnRange.minX, xSpawnRange.maxX);
        pos.y = (defaultSpawnPos.y * Camera.main.aspect) / targetAspect;
        t.position = pos;
    }

    void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        if (t == null) return;
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(xSpawnRange.minX, xSpawnRange.maxX);
        pos.y = (defaultSpawnPos.y * Camera.main.aspect) / targetAspect;
        t.position = pos;
        Spawn();
    }

    void Shift()
    {
        //loop through pool objects 
        //moving them
        //discarding them as they go off screen
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += Vector3.down * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
        //place objects off screen
        if (poolObject.transform.position.y > (-defaultSpawnPos.y * Camera.main.aspect) / targetAspect)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        //retrieving first available pool object
        for (int i = 0; i < poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }

}
