using UnityEngine;
using TMPro;

public class CollectablePointsFX : MonoBehaviour
{
    //Todo: Fade out FX
    [SerializeField]
    private float _upSpeed = 3.0f;
    [SerializeField]
    private float _enabledTime = 0.75f;

    private float _currentTime;
    private TMPro.TextMeshPro _textMeshPro;

    public bool TextRequest = false;
    private void OnEnable()
    {
        _currentTime = Time.time;
        if(!_textMeshPro)
        {
            _textMeshPro = GetComponent<TextMeshPro>();
            _textMeshPro.text = "+2up";
        }
    }
    void FixedUpdate()
    {
        if(_currentTime + _enabledTime < Time.time)
        {
            gameObject.SetActive(false);
        }
        if (TextRequest) PreviewTextRequest();
        transform.Translate(_upSpeed * Time.fixedDeltaTime * Vector3.up, Space.World);
    }
    public void PreviewTextRequest(string text = "+99up")
    {
        gameObject.SetActive(true);
        _textMeshPro.text = text;
    }
}
