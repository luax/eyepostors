using UnityEngine;
using System.Collections;

public class GenerateNormals : MonoBehaviour
{
	private Transform myTransform;
	private Transform midGeoTransform;
	private SkinnedMeshRenderer myRenderer;
	private Camera camera;
	private int[] triangles;
	private Vector3[] vertices;
	private Vector3[] normals;
	private Texture2D normalMap;
	private Color[] colors;
	private Color normalColor;
	private float[,] depthBuffer;
	private int index = 0;

	private struct Pixel
	{
		public int x;
		public int y;
		public float zInv;
		public Vector3 normal;

		public Pixel (int x, int y, float z, Vector3 normal)
		{
			this.x = x;
			this.y = y;
			this.zInv = z;
			this.normal = Vector3.Normalize (normal);
		}

		public Pixel (Vector3 pos3d, Vector3 normal)
		{
			this.x = (int)pos3d.x;
			this.y = (int)pos3d.y;
			this.zInv = 1f / pos3d.z;
			this.normal = Vector3.Normalize (normal);
		}
		
		public override string ToString ()
		{
			return "Pixel(position: (" + x + ", " + y + ", " + zInv + "), normal: " + normal + ")";
		}


	}
	
	private Color GetNormalColor (Vector3 normal)
	{
		Color c = new Color (normal.x + 1, normal.y + 1, normal.z + 1) / 2;
		c.a = 1f;
		return c;
	}
	
	// Use this for initialization
	private void Start ()
	{
		myTransform = transform;
		midGeoTransform = myTransform.FindChild ("CarlMidGeo");
		myRenderer = midGeoTransform.GetComponent<SkinnedMeshRenderer> ();
		camera = Camera.main;
		int w = Screen.width;
		int h = Screen.height;
		normalMap = new Texture2D (w, h);
		colors = new Color[w * h];
		depthBuffer = new float[w, h];
		normalColor = GetNormalColor (new Vector3 (0, 0, 1));
	}
	
	public Texture2D GetNormal ()
	{
		Mesh mesh = new Mesh ();	
		myRenderer.BakeMesh (mesh);
		triangles = mesh.triangles;
		vertices = mesh.vertices;
		normals = mesh.normals;
		int w = Screen.width;
		int h = Screen.height;
		depthBuffer = new float[w, h];
		normalMap = new Texture2D (w, h);
		
		ResetColors ();

		for (int i = 0; i < triangles.Length; i += 3) {
			if (!FacingCamera (i)) {
				continue;
			}
			Pixel v0 = new Pixel (ToScreenPosition (vertices [triangles [i]]), normals [triangles [i]]);
			Pixel v1 = new Pixel (ToScreenPosition (vertices [triangles [i + 1]]), normals [triangles [i + 1]]);
			Pixel v2 = new Pixel (ToScreenPosition (vertices [triangles [i + 2]]), normals [triangles [i + 2]]);
			
			DrawTriangle (new Pixel[]{v0,v1,v2});
		}
		normalMap.SetPixels (colors);
		normalMap.Apply ();
		return normalMap;
	}
	
	private bool FacingCamera (int index)
	{
		for (int i = index; i < index + 3; i++) {
			Vector3 normal = normals [triangles [i]];
			if (Vector3.Dot (normal, camera.transform.forward) < 0) {
				return true;
			}
		}
		return false;
	}

	private void ResetColors ()
	{
		for (int i = 0; i < colors.Length; i++) {
			colors [i] = normalColor;
		}
	}
	

	private Vector3 ToScreenPosition (Vector3 v)
	{
		return camera.WorldToScreenPoint (v);
	}

	private void DrawTriangle (Pixel[] vertices)
	{
		int numVertices = vertices.Length;
		int maxY = int.MinValue;
		int minY = int.MaxValue;
		foreach (Pixel p in vertices) {
			maxY = Mathf.Max (maxY, p.y);
			minY = Mathf.Min (minY, p.y);
		}
		int rows = (maxY - minY + 1);
		Pixel[] leftPixels = new Pixel[rows];
		Pixel[] rightPixels = new Pixel[rows];
		
		// Initialize rows
		for (int i = 0; i < rows; i++) {
			leftPixels [i].y = rightPixels [i].y = minY + i;
			leftPixels [i].x = int.MaxValue;
			rightPixels [i].x = int.MinValue;
		}
		
		// Calculate polygon rows
		for (int i = 0; i < numVertices; i++) {
			Pixel[] edge = GetLine (vertices [i], vertices [(i + 1) % numVertices]);
			foreach (Pixel p in edge) {
				int index = Mathf.Max ((p.y - minY), 0);
				if (leftPixels [index].x > p.x)
					leftPixels [index] = p;
				if (rightPixels [index].x < p.x)
					rightPixels [index] = p;
			}
		}
		
		for (int i = 0; i < rows; i++) {
			Pixel[] pixels = GetLine (leftPixels [i], rightPixels [i]);
			DrawLine (pixels);
		}

	}

	private void DrawLine (Pixel[] pixels)
	{
		foreach (Pixel p in pixels) {
			DrawPixel (p);
		}
	}

	private void DrawPixel (Pixel p)
	{
		if (p.zInv > depthBuffer [p.x, p.y]) {
			SetColorInTexture (p.x, p.y, GetNormalColor (p.normal));
			depthBuffer [p.x, p.y] = p.zInv;
		}
	}
	
	private void SetColorInTexture (int x, int y, Color color)
	{
		colors [y * Screen.width + x] = color;
	}

	private Pixel[] GetLine (Pixel a, Pixel b)
	{	
		int x = Mathf.Abs (a.x - b.x);
		int y = Mathf.Abs (a.y - b.y);
		return Interpolate (a, b, Mathf.Max (x, y) + 1);
	}

	private Pixel[] Interpolate (Pixel a, Pixel b, int n)
	{
		Pixel[] pixels = new Pixel[n];
		float factor = Mathf.Max ((n - 1), 1);
		float stepX = (b.x - a.x) / factor;
		float stepY = (b.y - a.y) / factor;
		float stepZ = (b.zInv - a.zInv) / factor;
		Vector3 stepNormal = (b.normal - a.normal) / factor;
		float currentX = a.x;
		float currentY = a.y;
		float currentZ = a.zInv;
		Vector3 currentNormal = a.normal;
		for (int i = 0; i < n; i++) {
			pixels [i] = new Pixel (Mathf.FloorToInt (currentX), Mathf.FloorToInt (currentY), currentZ, currentNormal);
			currentX += stepX;
			currentY += stepY;
			currentZ += stepZ;
			currentNormal += stepNormal;
		}
		return pixels;
	}


}
