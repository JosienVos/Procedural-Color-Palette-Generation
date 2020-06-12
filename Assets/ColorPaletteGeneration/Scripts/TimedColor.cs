using System.Collections.Generic;
using UnityEngine;

//This class is part of the tutorial on color palette generation by Josien Vos
//You can find the tutorial here: josienvos.nl/tutorial/color
//Feel free to use this code for your own projects, I would appreciate credit

//This generates a palette based on the game time each frame, displaying a smooth transition at all times
//The relevant part of the tutorial for this class is part 4: Customizing the generation for specific purposes

public class TimedColor : MonoBehaviour
{

	public Transform[] paletteGroups;

	private List<List<Renderer>> paletteRenderers = new List<List<Renderer>>() {
		new List<Renderer>(),
		new List<Renderer>(),
		new List<Renderer>(),
		new List<Renderer>(),
		new List<Renderer>(),
		new List<Renderer>()
	};

	private ColorHSL[] colorPalette = new ColorHSL[6]{
		new ColorHSL(0, .95f, .5f),
		new ColorHSL(0, .95f, .5f),
		new ColorHSL(0, .95f, .5f),
		new ColorHSL(0, .95f, .5f),
		new ColorHSL(0, .95f, .5f),
		new ColorHSL(0, .95f, .5f)
	};

	private void Awake() {
		for (int i = 0; i < 6; i++) {
			foreach (Transform child in paletteGroups[i]) {
				paletteRenderers[i].Add(child.GetComponent<Renderer>());
			}
		}
	}

	private void Update() {
		TimeBasedPalette();

		//update colors each frame
		for (int i = 0; i < 6; i++) {
			foreach (Renderer rend in paletteRenderers[i]) {
				rend.material.SetColor("_Color", colorPalette[i].rgb);
			}
		}
	}

	private void TimeBasedPalette() {
		float currentTime = Time.time;

		float mainHue = (currentTime % 60) * .016f; //loops from 0 to 1 each minute, all other variables oscillate between two values at different speeds
		float hueShift = Mathf.Sin(currentTime * .02f * Mathf.PI) * .1f;
		float mainSaturation = .3f + Mathf.PingPong(currentTime * .015f, 5) * .1f;
		float saturationShift = Mathf.PingPong(currentTime * .007f, .1f) + .01f;
		float mainLuminance = .5f + Mathf.Cos(currentTime * .1f) * .25f;
		float luminanceShift = (.5f - mainLuminance) * .2f;

		//analogous hue scheme, saturation and luminance rise or fall linearly. you can replace this with any other color scheme calculation.
		for (int i = 0; i < 6; i++) {
			colorPalette[i].h = Mathf.Repeat(mainHue + i * hueShift, 1);
			colorPalette[i].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
			colorPalette[i].l = Mathf.Clamp01(mainLuminance + luminanceShift * i);
		}
	}

}
