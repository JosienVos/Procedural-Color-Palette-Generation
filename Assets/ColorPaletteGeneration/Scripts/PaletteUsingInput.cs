using System.Collections.Generic;
using UnityEngine;

//This class is part of the tutorial on color palette generation by Josien Vos
//You can find the tutorial here: josienvos.nl/tutorial/color
//Feel free to use this code for your own projects, I would appreciate credit

//This class generates color palettes based on input variables: either color, three abstract floats, or the UNIX timestamp
//The relevant part of the tutorial for this class is part 3: Using input to control palette generation

public class PaletteUsingInput : MonoBehaviour
{

	public enum InputMethod {
		unixTime,
		InputColor,
		AbstractVariables
	}
	
	public KeyCode updateKey = KeyCode.Return;

	[Header("Input")]
	public InputMethod inputMethod = InputMethod.unixTime;
	public Color inputColor = new Color(1, 0, 0);
	[Range(0f, 1f)] public float variableA = 0;
	[Range(0f, 1f)] public float variableB = 1;
	[Range(0f, 1f)] public float variableC = .5f;

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
		switch (inputMethod) {
			case (InputMethod.unixTime):
				UnixPalette();
				break;
			case (InputMethod.InputColor):
				ColorBasedPalette();
				break;
			case (InputMethod.AbstractVariables):
				VariableBasedPalette();
				break;
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
	
	private void UnixPalette() {
		//get UNIX timestamp
		System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		int unixTime = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

		//use it to generate color scheme variables with
		float mainHue = (unixTime % 60) * .016f; //loops from 0 to 1 each minute, all other variables oscillate between two values at different speeds
		float hueShift = Mathf.Sin(unixTime * .02f * Mathf.PI) * .1f;
		float mainSaturation = .3f + Mathf.PingPong(unixTime * .015f, 5) * .1f;
		float saturationShift = Mathf.PingPong(unixTime * .007f, .1f) + .01f;
		float mainLuminance = .5f + Mathf.Cos(unixTime * .1f) * .25f;
		float luminanceShift = (.5f - mainLuminance) * .2f;

		//analogous hue scheme, saturation and luminance rise or fall linearly. you can replace this with any other color scheme calculation. (see PaletteUsingRandom)
		for (int i = 0; i < 6; i++) {
			colorPalette[i].h = Mathf.Repeat(mainHue + i * hueShift, 1);
			colorPalette[i].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
			colorPalette[i].l = Mathf.Clamp01(mainLuminance + luminanceShift * i);
		}
	}

	private void ColorBasedPalette() {
		ColorHSL inputHSL = new ColorHSL(inputColor);

		//base color scheme variables on the hue, saturation and lightness of the input color
		float mainHue = inputHSL.h;
		float hueShift = inputHSL.s * inputHSL.l * .5f - .2f;
		float mainSaturation = inputHSL.s;
		float saturationShift = inputHSL.s * .1f * (inputHSL.s + inputHSL.l);
		float mainLuminance = inputHSL.l;
		float luminanceShift = Mathf.Sin(inputHSL.l * 31) * .07f;

		//analogous hue scheme, saturation and luminance rise or fall linearly. you can replace this with any other color scheme calculation. (see PaletteUsingRandom)
		for (int i = 0; i < 6; i++) {
			colorPalette[i].h = Mathf.Repeat(mainHue + i * hueShift, 1);
			colorPalette[i].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
			colorPalette[i].l = Mathf.Clamp01(mainLuminance + luminanceShift * i);
		}
	}

	private void VariableBasedPalette() {
		//base color scheme variables on the abstract input variables A, B and C
		//as A, B and C currently mean nothing, it doesn't matter what effect they have, but if you implement variables that have a meaning (for example score), thinking about how they influence color is more important
		float mainHue = variableA + variableB;
		float hueShift = Mathf.Clamp(variableC * .3f, .05f, .15f);
		float mainSaturation = Mathf.Repeat(variableA + variableB + variableC, 1);
		float saturationShift = Mathf.Clamp(Mathf.Sin(variableC * 3) * variableA, 0, .2f);
		float mainLuminance = Mathf.Sin(variableB * 60) * .2f + .5f;
		float luminanceShift = Mathf.Clamp(Mathf.Sin(variableA * 3 + variableB * 2) * .1f, -.1f, .1f);

		//analogous hue scheme, saturation and luminance rise or fall linearly. you can replace this with any other color scheme calculation. (see PaletteUsingRandom)
		for (int i = 0; i < 6; i++) {
			colorPalette[i].h = Mathf.Repeat(mainHue + i * hueShift, 1);
			colorPalette[i].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
			colorPalette[i].l = Mathf.Clamp01(mainLuminance + luminanceShift * i);
		}
	}

}
