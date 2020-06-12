using System.Collections.Generic;
using UnityEngine;

//This class is part of the tutorial on color palette generation by Josien Vos
//You can find the tutorial here: josienvos.nl/tutorial/color
//Feel free to use this code for your own projects, I would appreciate credit

//This class generates color, and tells all objects that the class ObjectBasedColor is attached to to change color
//The relevant part of the tutorial for this class is part 5: Applying a Palette to a Unity Scene

public class ApplyOnObjecBasis : MonoBehaviour
{
	public Material skyboxMaterial;

	public List<ObjectBasedColor> paletteObjects = new List<ObjectBasedColor>();

	private ColorHSL[] colorPalette = new ColorHSL[6]{
		new ColorHSL(0, .95f, .5f),
		new ColorHSL(0, .95f, .5f),
		new ColorHSL(0, .95f, .5f),
		new ColorHSL(0, .95f, .5f),
		new ColorHSL(0, .95f, .5f),
		new ColorHSL(0, .95f, .5f)
	};

	private void FixedUpdate() {
		NewColorPalette();
	}

	private void NewColorPalette() {

		UNIXpalette();
		
		foreach (ObjectBasedColor obj in paletteObjects) {
			obj.SetColor(colorPalette); //pass the entire palette to the object
		}

		RenderSettings.fogColor = colorPalette[5].rgb;
		if (skyboxMaterial != null) {
			skyboxMaterial.SetColor("_yColor", colorPalette[2].rgb);
			skyboxMaterial.SetColor("_horColor", colorPalette[5].rgb);
		}
	}

	private void UNIXpalette() {
		float currentTime = Time.time;

		float mainHue = (currentTime % 60) * .016f;
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
