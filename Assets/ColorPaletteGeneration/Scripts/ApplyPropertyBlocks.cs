using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyPropertyBlocks : MonoBehaviour
{

	public Material skyboxMaterial;

	public Transform[] paletteGroups;

	private MaterialPropertyBlock propertyBlock;

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

		propertyBlock = new MaterialPropertyBlock();

		for (int i = 0; i < 6; i++) {

			foreach (Transform child in paletteGroups[i]) {
				paletteRenderers[i].Add(child.GetComponent<Renderer>());
			}
		}

		NewColorPalette();
	}

	private void FixedUpdate() {
		NewColorPalette();
	}

	private void NewColorPalette() {

		UNIXpalette();

		for (int i = 0; i < 6; i++) {
			foreach (Renderer rend in paletteRenderers[i]) {
				rend.GetPropertyBlock(propertyBlock);
				propertyBlock.SetColor("_Color", colorPalette[i].rgb);
				rend.SetPropertyBlock(propertyBlock);
			}
		}

		RenderSettings.fogColor = colorPalette[5].rgb;
		if (skyboxMaterial != null) {
			skyboxMaterial.SetColor("_yColor", colorPalette[2].rgb);
			skyboxMaterial.SetColor("_horColor", colorPalette[5].rgb);
		}
	}

	private void UNIXpalette() {
		//get UNIX timestamp
		System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		int unixTime = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

		float mainHue = (unixTime % 60) * .016f;
		float hueShift = Mathf.Sin((unixTime % 100) * .02f * Mathf.PI) * .1f;

		float mainSaturation = .5f + (unixTime % 1000) * .0005f;
		float saturationShift = (unixTime % 7) * .028f - .1f;

		float mainLuminance = .5f + Mathf.Cos((unixTime % 10) * .1f) * .25f;
		float luminanceShift = (unixTime % 6) * .033f - .1f;

		//analogous hue scheme, saturation and luminance rise or fall linearly. you can replace this with any other color scheme calculation.
		for (int i = 0; i < 6; i++) {
			colorPalette[i].h = Mathf.Repeat(mainHue + i * hueShift, 1);
			colorPalette[i].s = Mathf.Clamp01(mainSaturation - saturationShift * i);
			colorPalette[i].l = Mathf.Clamp01(mainLuminance + luminanceShift * i);
		}
	}
}
