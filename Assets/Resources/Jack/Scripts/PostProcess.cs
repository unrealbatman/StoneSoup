using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class PostProcess : MonoBehaviour
{
	public Material PostProcessMat;
	[HideInInspector] public List<GameObject> lightObjects;
	public List<LightSrc> srcs;

	ComputeBuffer lights;
	
	private void Start()
	{
		srcs = new List<LightSrc>();
		lightObjects = new List<GameObject>();
		lightObjects = GameObject.FindGameObjectsWithTag("Light").ToList();
		lightObjects.Add(Player.instance.gameObject);
        lights = new ComputeBuffer(1, 24); //4 bytes + 8 bytes + 12;
    }
	void UpdateLightBuffer() {
		srcs.Clear();
		foreach (GameObject go in lightObjects)
		{
			if (go == null) continue;
			LightSrc src = new LightSrc();
			Vector2 pix = Camera.main.WorldToScreenPoint(go.transform.position);
			src.screen_position = new Vector2(pix.x / Camera.main.pixelWidth, pix.y / Camera.main.pixelHeight);
			src.intensity = 0.1f;
			src.glowColor = Vector3.one;
			if(go.TryGetComponent(out GlowEffect glo)) {
				src.glowColor = new Vector3(glo.tint.r, glo.tint.g, glo.tint.b);
			}
			if (go.CompareTag("Player")){
				src.intensity = 1f;
			}

			srcs.Add(src);
		}
		lights.Release();
		lights = new ComputeBuffer(srcs.Count, 24); //4 bytes + 8 bytes + 12;
		lights.SetData(srcs);
    }

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		UpdateLightBuffer();
		PostProcessMat.SetBuffer("lightBuffer", lights);
		PostProcessMat.SetInt("bufferSize", srcs.Count);
		Graphics.Blit(source, destination, PostProcessMat);
	}
}

public struct LightSrc {
	public Vector2 screen_position;
	public float intensity;
	public Vector3 glowColor;
}
