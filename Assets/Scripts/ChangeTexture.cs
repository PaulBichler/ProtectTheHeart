using UnityEngine;

public class ChangeTexture : MonoBehaviour
{
    private static readonly int EmissiveColorMap = Shader.PropertyToID("EmissiveColorMap");

    public Texture2D TextureToChangeTo;
    private Texture2D _previousTexture;
    private Renderer _renderer;

    private bool previous = true;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _previousTexture = (Texture2D) _renderer.material.GetTexture(EmissiveColorMap);
    }

    public void Change()
    {
        _renderer.material.SetTexture(EmissiveColorMap, previous ? TextureToChangeTo : _previousTexture);

        previous = !previous;
    }
}