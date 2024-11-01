using UnityEngine;
using System.Collections;
using System.Linq;

public class heatmapPixelsTime : MonoBehaviour {

	public bool OpacityColor;

	public Gradient ColorGradient;

	[Range(1, 10)]
	public int radiusInfluence;

	public enum Size{
		veryLow,
		low,
		medium,
		high,
		veryHigh
	};
	public Size TexResolution = Size.low;

	private float[] pixel;
	private float maxPixel;

	private int u;
	private int v;
	private Vector2 pixelUV;

	public GameObject sphere;

	private Texture2D tex;

	public bool hiddenMode;
	public KeyCode hiddenModeKey = KeyCode.H;

	void Awake (){

		switch (TexResolution){
			case Size.veryLow:
				tex = new Texture2D(32,16);
				break;
			case Size.low:
				tex = new Texture2D(64,32);
				break;
			case Size.medium:
				tex = new Texture2D(128,64);
				break;
			case Size.high:
				tex = new Texture2D(256,128);
				break;
			case Size.veryHigh:
				tex = new Texture2D(512,256);
				break;
		}

		sphere.GetComponent<MeshRenderer>().material.mainTexture = tex;

		//create pixel variable  depending on size texture
		pixel = new float[tex.width*tex.height];


	}

	public void ResetValues (){

		print ("RESET");
		//value of pixels 0 
		for (int i = 0; i < tex.width*tex.height; i++) {
			pixel[i] = 0f;
		}
		maxPixel = 1f;
	}

	void Start (){

		ResetValues();
		Draw();

	}

	void Update()
    {
		if (Input.GetKeyDown(hiddenModeKey))
		{
			hiddenModeSwitch();
		}

		Calculate();

		if (hiddenMode == false) {
			Draw();
		}

	}

	public void hiddenModeSwitch()
    {
		if (hiddenMode == true)
		{
			hiddenMode = false;
			sphere.GetComponent<MeshRenderer>().enabled = true;
		}
		else
		{
			hiddenMode = true;
			sphere.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	public void Draw() {

		//if texture size is 64x32 , does 2048 calculations
		for (int x = 0; x < tex.width; x++) {
			for (int y = 0; y < tex.height; y++) {
				int i = x + tex.width * y;

				//take the highest value of the pixels
				if (pixel[i] > maxPixel) {
					maxPixel = pixel[i];
				}

				//put the value of pixel in alpha or in ramp, relative to maximum value of all reticule
				if (OpacityColor == true) {
					Color colorUpdate = new Color(0f, 1f, 0f, pixel[i] / maxPixel);
					tex.SetPixel(x, y, colorUpdate);

				} else {
					Color colorUpdate = ColorGradient.Evaluate(pixel[i] / maxPixel);
					tex.SetPixel(x, y, colorUpdate);
				}

			}
		}

		//apply texture
		tex.Apply();

	}

	void Calculate() {

		//raycast in the center of viewport
		Ray ray = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;
		Physics.Raycast(ray, out hit);

		//apply hit to renderer
		Renderer rend = hit.transform.GetComponent<Renderer>();
		MeshCollider meshCollider = hit.collider as MeshCollider;

		if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
			return;

		//take coordinates of hit
		pixelUV = hit.textureCoord;

		//coordinates in depending on height and width
		pixelUV.x *= tex.width;
		pixelUV.y *= tex.height;

		//check position of pixel
		tex.GetPixel ((int)pixelUV.x, (int)pixelUV.y);

		//choose radius of the circle
		float rSquared = radiusInfluence * radiusInfluence;

		//for each pixel of texture
		for (int u = (int)pixelUV.x - (int)radiusInfluence; u < (int)pixelUV.x + (int)radiusInfluence + 1; u++) {
			for (int v = (int)pixelUV.y - (int)radiusInfluence; v < (int)pixelUV.y + (int)radiusInfluence + 1; v++) {
				//define limits
				if (v >= 0 && v < tex.height) {
						//create circle
						if ((pixelUV.x - u) * (pixelUV.x - u) + (pixelUV.y - v) * (pixelUV.y - v) < rSquared)
					{
						//edit the value of the pixel, adding deltatime and making a gradient from the center
						int PixCurrent = u + tex.width * v;
						pixel[PixCurrent] += Time.deltaTime * (1f - ((pixelUV.x - u) * (pixelUV.x - u) + (pixelUV.y - v) * (pixelUV.y - v)) / radiusInfluence * 0.05f);
					}
				}
			}
		}



	}

}