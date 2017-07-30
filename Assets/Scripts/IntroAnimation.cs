using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IntroAnimation : MonoBehaviour {

	public Texture[] introTextures;
	public float speed = 0.1f;
	public RawImage image;

	private float index;

	void Update()
	{
		index += speed * Time.deltaTime;
		if (index >= introTextures.Length)
		{
			index = 0;
		}

		image.texture = introTextures[(int)index];
	}
}
