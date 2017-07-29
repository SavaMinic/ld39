using UnityEngine;
using System.Collections;

public class TextureOffsetScroll : MonoBehaviour
{

	public float scrollSpeed;
	private Vector2 savedOffset;

	private Renderer myRenderer;

	void Start()
	{
		myRenderer = GetComponent<Renderer>();
		savedOffset = myRenderer.sharedMaterial.GetTextureOffset("_MainTex");
	}

	void Update()
	{
		float x = Mathf.Repeat(Time.time * scrollSpeed, 1);
		Vector2 offset = new Vector2(x, savedOffset.y);
		myRenderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
	}

	void OnDisable()
	{
		myRenderer.sharedMaterial.SetTextureOffset("_MainTex", savedOffset);
	}
}
