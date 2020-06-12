using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is part of the tutorial on color palette generation by Josien Vos
//You can find the tutorial here: josienvos.nl/tutorial/color
//Feel free to use this code for your own projects, I would appreciate credit

//This class is attached to an object that is supposed to change color, and used together with the ApplyOnObjectBasis class.
//The relevant part of the tutorial for this class is part 5: Applying a Palette to a Unity Scene

public class ObjectBasedColor : MonoBehaviour
{

	public int colorID = 0;
	public bool random = false;

	private int randomInt = 0;
	private List<Renderer> rendererList = new List<Renderer>();

	private void Awake() {
		if (GetComponent<Renderer>() != null) {
			rendererList.Add(GetComponent<Renderer>());
		}
		foreach (Transform child in transform) {
			rendererList.Add(child.GetComponent<Renderer>());
		}
		StartCoroutine(Randomizer());
	}

	public void SetColor(ColorHSL[] colorPalette) {
		foreach (Renderer rend in rendererList) {
			if (!random) {
				rend.material.SetColor("_Color", colorPalette[colorID % colorPalette.Length].rgb);
			}
			else {
				rend.material.SetColor("_Color", colorPalette[randomInt % colorPalette.Length].rgb);
			}
		}
	}

	private IEnumerator Randomizer() {
		while (true) {
			randomInt = Random.Range(0, 100);
			yield return new WaitForSeconds(1);
		}
	}

}
