using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour {

	[System.Serializable]
	public struct SpawnHeight {
		public float min;
		public float max;
	}

	public GameObject PlatformPrefab;
	public float shiftSpeed;
	public float spawnRate;
	public SpawnHeight spawnHeight;
	public Vector3 spawnPos;
	public Vector2 targetAspectRatio;
	public bool beginInScreenCenter;

	List<Transform> platforms;
	float spawnTimer;
	GameManager game;
	float targetAspect;
	Vector3 dynamicSpawnPos;

	void Start() {
		platforms = new List<Transform>();
		game = GameManager.Instance;
		if (beginInScreenCenter)
			SpawnPlatform();
	}

	void OnEnable() {
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
	}

	void OnDisable() {
		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
	}
    
	void OnGameOverConfirmed() {
		for (int i = platforms.Count - 1; i >= 0; i--) {
			GameObject temp = platforms[i].gameObject;
			platforms.RemoveAt(i);
			Destroy(temp);
		}
		if (beginInScreenCenter)
            SpawnPlatform();
	}

	void Update() {
		if (game.GameOver) return;

		targetAspect = (float)targetAspectRatio.x / targetAspectRatio.y;
		dynamicSpawnPos.x = (spawnPos.x * Camera.main.aspect) / targetAspect;
		
		spawnTimer += Time.deltaTime;
		if (spawnTimer >= spawnRate) {
            SpawnPlatform();
			spawnTimer = 0;
		}

		ShiftPlatforms();
	}

	void SpawnPlatform() {
		GameObject platform = Instantiate(PlatformPrefab) as GameObject;
		platform.transform.SetParent(transform);
		platform.transform.localPosition = dynamicSpawnPos;
		if (beginInScreenCenter && platforms.Count == 0) {
			platform.transform.localPosition = Vector3.zero;
		}
		float randomYPos = Random.Range(spawnHeight.min, spawnHeight.max);
		platform.transform.position += Vector3.up * randomYPos;
		platforms.Add(platform.transform);
	}

	void ShiftPlatforms() {
		for (int i = platforms.Count - 1; i >= 0; i--) {
			platforms[i].position -= Vector3.right * shiftSpeed * Time.deltaTime;
			if (platforms[i].position.x < (-dynamicSpawnPos.x * Camera.main.aspect) / targetAspect) {
				GameObject temp = platforms[i].gameObject;
				platforms.RemoveAt(i);
				Destroy(temp);
			}
		}
	}
}
