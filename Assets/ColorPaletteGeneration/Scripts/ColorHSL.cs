using UnityEngine;

//This struct is part of the tutorial on color palette generation by Josien Vos
//You can find the tutorial here: josienvos.nl/tutorial/color
//Feel free to use this code for your own projects, I would appreciate credit

//This is a struct for colors in HSL space, to use instead of the built-in RGB-based Color struct.

public struct ColorHSL {

	private float _h;
	private float _s;
	private float _l;



	//CONSTRUCTORS
	public ColorHSL(float h, float s, float l) {
		_h = Mathf.Repeat(h, 1);
		_s = Mathf.Clamp01(s);
		_l = Mathf.Clamp01(l);
	}

	public ColorHSL(Color c) {
		float v;
		Color.RGBToHSV(c, out _h, out _s, out v);
		_l = v * (1 - _s * .5f);
		_s = (v - _l) / Mathf.Min(_l, 1 - _l);
		if (_l == 0 || _l == 1) {
			_s = 0;
		}
	}



	//GET OR SET HSL VALUES
	public float h {
		get {
			return _h;
		}
		set {
			_h = Mathf.Repeat(value, 1);
		}
	}

	public float s {
		get {
			return _s;
		}
		set {
			_s = Mathf.Clamp01(value);
		}
	}

	public float l {
		get {
			return _l;
		}
		set {
			_l = Mathf.Clamp01(value);
		}
	}



	//GET / SET RGB VALUE
	public Color rgb {
		get {
			Color c = Color.HSVToRGB(Mathf.Repeat(_h, 1), 1, 1); //get RGB with the right hue, but full saturation and value

			Vector3 vec = new Vector3(c.r, c.g, c.b);
			vec = Vector3.Lerp(new Vector3(.5f, .5f, .5f), vec, _s); //desaturate
			vec = Vector3.Lerp(vec, new Vector3(1, 1, 1), Mathf.Clamp01(_l * 2 - 1)); //lighten
			vec *= Mathf.Clamp01(_l * 2); //darken

			c = new Color(vec.x, vec.y, vec.z);
			return c;
		}
		set {
			float v;
			Color.RGBToHSV(value, out _h, out _s, out v); //unity's built-in rgb to hsv, to then convert from hsv to hsl
			_l = v * (1 - _s * .5f);
			_s = (v - _l) / Mathf.Min(_l, 1 - _l);
			if (_l == 0 || _l == 1) {
				_s = 0;
			}
		}
	}



	//LERP BETWEEN TWO COLORS USING HSL
	public static ColorHSL Lerp(Color a, Color b, float t) { return Lerp(new ColorHSL(a), new ColorHSL(b), t); }
	public static ColorHSL Lerp(ColorHSL a, Color b, float t) { return Lerp(a, new ColorHSL(b), t); }
	public static ColorHSL Lerp(Color a, ColorHSL b, float t) { return Lerp(new ColorHSL(a), b, t); }
	public static ColorHSL Lerp(ColorHSL a, ColorHSL b, float t) {
		ColorHSL c = new ColorHSL(0, 0, 0);
		if (Mathf.Abs(a.h - b.h) <= .5f) {
			c.h = Mathf.Lerp(a.h, b.h, t);
		}
		else if (a.h > b.h) {
			c.h = Mathf.Lerp(a.h, b.h + 1, t);
		}
		else {
			c.h = Mathf.Lerp(a.h + 1, b.h, t);
		}
		c.s = Mathf.Lerp(a.s, b.s, t);
		c.l = Mathf.Lerp(a.l, b.l, t);
		return c;
	}

}
