using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class PostProcess : MonoBehaviour
{
	public Material PostProcessMat;
	List<GameObject> ligos;
	public List<LightSrc> srcs;

	ComputeBuffer lights;
	
	private void Start()
	{
		srcs = new List<LightSrc>();
		ligos = GameObject.FindGameObjectsWithTag("Light").ToList();
		ligos.Add(Player.instance.gameObject);
 	}
	void UpdateLightBuffer() {
		srcs.Clear();
		foreach (GameObject go in ligos)
		{
			if (go == null) continue;
			LightSrc src = new LightSrc();
			Vector2 pix = Camera.main.WorldToScreenPoint(go.transform.position);
			src.screen_position = new Vector2(pix.x / Camera.main.pixelWidth, pix.y / Camera.main.pixelHeight);
			src.intensity = 0.1f;
			if (go.CompareTag("Player")){
				src.intensity = 1f;
			}

			srcs.Add(src);
		}
		lights = new ComputeBuffer(srcs.Count, 12); //4 bytes + 8 bytes
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
}
