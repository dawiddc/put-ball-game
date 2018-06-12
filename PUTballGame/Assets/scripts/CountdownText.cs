using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownText : MonoBehaviour {

	public delegate void CountdownFinished();
	public static event CountdownFinished OnCountdownFinished;

	Text countdown;

	void OnEnable() {
		countdown = GetComponent<Text>();
		countdown.text = "3";
		StartCoroutine("Countdown");
	}

	IEnumerator Countdown() {
		int count = 3;
		for (int i = 0; i < count; i++) {
			countdown.text = (count - i).ToString();
			yield return new WaitForSeconds(1);
		}

		OnCountdownFinished();
	}
}
