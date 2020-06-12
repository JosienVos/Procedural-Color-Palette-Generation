using System.Collections.Generic;
using UnityEngine;

//This class is part of the tutorial on color palette generation by Josien Vos
//You can find the tutorial here: josienvos.nl/tutorial/color
//Feel free to use this code for your own projects, I would appreciate credit

//This class generates color palettes, based on relations between the colors' hue, saturation and lightness differences
//The relevant part of the tutorial for this class is part 2: Color theory to code

public class PaletteUsingRandoms : MonoBehaviour
{
    
	public enum ColorScheme {
		Analogous,
		Complementary,
		Triadic,
		Tetradic,
		SplitComplementary,
		Random
	}

	public ColorScheme colorScheme = ColorScheme.Random;
	public KeyCode updateKey = KeyCode.Return;
	public bool adaptiveSaturation = false;

	public Material skyboxMaterial;

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

	private void Start() {
		//collect all renderers in the proper lists to use later
		for (int i = 0; i < 6; i++) {
			foreach (Transform child in paletteGroups[i]) {
				paletteRenderers[i].Add(child.GetComponent<Renderer>());
			}
		}
		NewColorPalette();
	}

	private void Update() {
		if (Input.GetKeyDown(updateKey)) {
			NewColorPalette();
		}
	}

	private void NewColorPalette() {
		if (colorScheme == ColorScheme.Random) {
			ColorScheme randomScheme = (ColorScheme)Random.Range(0, 5);
			GenerateHue(randomScheme);
			GenerateLightness(randomScheme);
			GenerateSaturation(randomScheme);
		}
		else {
			GenerateHue(colorScheme);
			GenerateLightness(colorScheme);
			GenerateSaturation(colorScheme);
		}

		//apply to objects
		for (int i = 0; i < 6; i++) {
			foreach (Renderer rend in paletteRenderers[i]) {
				rend.material.SetColor("_Color", colorPalette[i].rgb);
			}
		}

		//apply to fog and skybox
		RenderSettings.fogColor = colorPalette[5].rgb;
		if (skyboxMaterial != null) {
			skyboxMaterial.SetColor("_yColor", colorPalette[2].rgb);
			skyboxMaterial.SetColor("_horColor", colorPalette[5].rgb);
		}
	}
	
	private void GenerateHue(ColorScheme scheme) {

		float mainHue = Random.Range(0f, 1f);
		float hueShift = 0;

		switch (scheme) {
			case ColorScheme.Analogous:
				hueShift = Random.Range(.03f, .08f);
				for (int i = 0; i < 6; i++) {
					colorPalette[i].h = Mathf.Repeat(mainHue + i * hueShift, 1);
				}
				break;

			case ColorScheme.Complementary:
				hueShift = Random.Range(.03f, .05f) + .5f;
				for (int i = 0; i < 6; i++) {
					colorPalette[i].h = Mathf.Repeat(mainHue + i * hueShift, 1);
				}
				break;

			case ColorScheme.Triadic:
				hueShift = Random.Range(.01f, .025f) + .33f;
				for (int i = 0; i < 6; i++) {
					colorPalette[i].h = Mathf.Repeat(mainHue + i * hueShift, 1);
				}
				break;

			case ColorScheme.Tetradic:
				hueShift = Random.Range(.1f, .4f);
				for (int i = 0; i < 6; i++) {
					colorPalette[i].h = Mathf.Repeat(mainHue + (i % 2) * hueShift - (i % 2 - 1) * .5f, 1);
				}
				break;

			case ColorScheme.SplitComplementary:
				hueShift = Random.Range(.03f, .08f);
				colorPalette[0].h = Mathf.Repeat(mainHue + .5f, 1); //the complementary color to the otherwise analogous scheme
				for (int i = 1; i < 6; i++) {
					colorPalette[i].h = Mathf.Repeat(mainHue + (i - 3) * hueShift, 1);
				}
				break;
		}
	}

	private void GenerateSaturation(ColorScheme scheme) {

		float mainSaturation = 1 - Random.Range(0, .6f);
		float saturationShift = mainSaturation * Random.Range(.2f, 1);

		switch (scheme) {
			//this scheme linearly lowers the saturation of each color
			case ColorScheme.Analogous:
			case ColorScheme.SplitComplementary:
				saturationShift *= .16f;
				for (int i = 0; i < 6; i++) {
					colorPalette[i].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
				}
				break;
			
			//this scheme lowers the saturation every two colors
			case ColorScheme.Complementary:
			case ColorScheme.Tetradic:
				saturationShift *= .33f;
				for (int i = 0; i < 3; i++) {
					colorPalette[i * 2].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
					colorPalette[i * 2 + 1].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
				}
				break;
			
			//this scheme lowers the saturation every three colors
			case ColorScheme.Triadic:
				saturationShift *= .5f;
				for (int i = 0; i < 2; i++) {
					colorPalette[i * 3].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
					colorPalette[i * 3 + 1].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
					colorPalette[i * 3 + 2].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
				}
				break;
		}

		//saturation is heightened when the lightness is further from .5f (ergo, very dark and very light colors will be more saturated)
		if (adaptiveSaturation) {
			saturationShift *= .16f;
			for (int i = 0; i < 6; i++) {
				colorPalette[i].s = Mathf.Clamp01(mainSaturation - saturationShift * i + Mathf.Abs(colorPalette[i].l - .5f));
			}
		}
	}

	private void GenerateLightness(ColorScheme scheme) {

		float mainLightness = Random.Range(.2f, .8f);
		float lightnessShift;
		int randomScheme = Random.Range(0, 3);

		if (randomScheme == 0) { //lighten
			lightnessShift = (1 - mainLightness) * .15f * Random.Range(.2f, .9f);
			for (int i = 0; i < 6; i++) {
				colorPalette[i].l = Mathf.Clamp01(mainLightness + lightnessShift * i);
			}
		}
		else if (randomScheme == 1) { //darken
			lightnessShift = mainLightness * .15f * Random.Range(.2f, .9f);
			for (int i = 0; i < 6; i++) {
				colorPalette[i].l = Mathf.Clamp01(mainLightness - lightnessShift * i);
			}
		}
		else if (randomScheme == 2) { //switch between lightening and darkening
			lightnessShift = Mathf.Min(mainLightness, 1 - mainLightness) * .15f * Random.Range(.2f, .9f);
			for (int i = 0; i < 3; i++) {
				colorPalette[i * 2].l = Mathf.Clamp01(mainLightness + lightnessShift * i);
				colorPalette[i * 2 + 1].l = Mathf.Clamp01(mainLightness - lightnessShift * i);
			}
		}
	}

}
