using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
	private static int
		baseColorId = Shader.PropertyToID("_BaseColor"),
		cutoffId = Shader.PropertyToID("_Cutoff");

	static MaterialPropertyBlock block;

	[SerializeField]
	Color baseColor = Color.white;

	[SerializeField, Range(0f, 1f)]
	float alphaCutoff = 0.5f;

	void Awake () {
		OnValidate();
	}

	void OnValidate () {
		if (block == null) {
			block = new MaterialPropertyBlock();
		}
		block.SetColor(baseColorId, baseColor);
		block.SetFloat(cutoffId, alphaCutoff);
		GetComponent<Renderer>().SetPropertyBlock(block);
	}
}