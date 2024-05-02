using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] RawImage _image;
    [SerializeField] TMP_Text _titleField;
    [SerializeField] TMP_Text _descriptionField;
    [SerializeField] Button _moreButton;
    private RectTransform rectTransform;

    private Canvas canvas;
    public SwipeEffect swipeEffect;

    public int ind = 0;
    private Article article;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        swipeEffect = GetComponent<SwipeEffect>();
        rectTransform = GetComponent<RectTransform>();
        DisableCard();
    }
    public void EnableCard()
    {
        rectTransform.localPosition = new Vector3(0, 0, 0);
        canvas.enabled = true;
        canvas.sortingOrder = 2;
        swipeEffect.enabled = true;
    }
    public void DisableCard()
    {
        canvas.sortingOrder = 0;
        canvas.enabled = false;
        swipeEffect.enabled = false;
    }
    public void SetNextCard()
    {
        canvas.enabled = true;
        canvas.sortingOrder = 1;
        rectTransform.localPosition = Vector3.zero;
    }
    public void SetPreviousCard()
    {
        canvas.enabled = true;
        canvas.sortingOrder = 4;
        rectTransform.localPosition = new Vector3(0, Camera.main.pixelHeight, 0);
    }
    private void Start()
    {
        _moreButton.onClick.AddListener(OnMore);
    }
    public void LoadData()
    {
        article = DataManager.data.articles[ind];
        _titleField.text = article.title;
        _descriptionField.text = article.description;
        StopAllCoroutines();
        StartCoroutine(GetImage(article.urlToImage));
    }
    private void OnMore()
    {
        Application.OpenURL(article.url);
    }

    IEnumerator GetImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            _image.texture = DownloadHandlerTexture.GetContent(request);
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }
}