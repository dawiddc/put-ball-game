using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour {

	[System.Serializable]
	public struct SpawnHeight {
		public float min;
		public float max;
	}

	public GameObject PipePrefab;
	public float shiftSpeed;
	public float spawnRate;
	public SpawnHeight spawnHeight;
	public Vector3 spawnPos;
	public Vector2 targetAspectRatio;
	public bool beginInScreenCenter;

	List<Transform> pipes;
	float spawnTimer;
	GameManager game;
	float targetAspect;
	Vector3 dynamicSpawnPos;

	void Start() {
		pipes = new List<Transform>();
		game = GameManager.Instance;
		if (beginInScreenCenter)
			SpawnPipe();
	}

	void OnEnable() {
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
	}

	void OnDisable() {
		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
	}

	void OnGameOverConfirmed() {
		for (int i = pipes.Count - 1; i >= 0; i--) {
			GameObject temp = pipes[i].gameObject;
			pipes.RemoveAt(i);
			Destroy(temp);
		}
		if (beginInScreenCenter)
			SpawnPipe();
	}

	void Update() {
		if (game.GameOver) return;

		targetAspect = (float)targetAspectRatio.x / targetAspectRatio.y;
		dynamicSpawnPos.x = (spawnPos.x * Camera.main.aspect) / targetAspect;
		
		spawnTimer += Time.deltaTime;
		if (spawnTimer >= spawnRate) {
			SpawnPipe();
			spawnTimer = 0;
		}

		ShiftPipes();
	}

	void SpawnPipe() {
		GameObject pipe = Instantiate(PipePrefab) as GameObject;
		pipe.transform.SetParent(transform);
		pipe.transform.localPosition = dynamicSpawnPos;
		if (beginInScreenCenter && pipes.Count == 0) {
			pipe.transform.localPosition = Vector3.zero;
		}
		float randomYPos = Random.Range(spawnHeight.min, spawnHeight.max);
		pipe.transform.position += Vector3.up * randomYPos;
		pipes.Add(pipe.transform);
	}

	void ShiftPipes() {
		for (int i = pipes.Count - 1; i >= 0; i--) {
			pipes[i].position -= Vector3.right * shiftSpeed * Time.deltaTime;
			if (pipes[i].position.x < (-dynamicSpawnPos.x * Camera.main.aspect) / targetAspect) {
				GameObject temp = pipes[i].gameObject;
				pipes.RemoveAt(i);
				Destroy(temp);
			}
		}
	}
}
