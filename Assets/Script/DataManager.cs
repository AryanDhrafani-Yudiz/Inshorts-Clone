using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Source
{
    public object id;
    public string name;
}
[Serializable]
public class Article
{
    public Source source;
    public string author;
    public string title;
    public string description;
    public string url;
    public string urlToImage;
    public DateTime publishedAt;
    public string content;
}
[Serializable]
public class Root
{
    public string status;
    public int totalResults;
    public List<Article> articles;
}
public class DataManager : MonoBehaviour
{
    private readonly string NEWS_API = "https://newsapi.org/v2/everything?q=tesla&from=2024-04-02&sortBy=publishedAt&apiKey=5a2848f75ccb4920805a8bf669a641ba";
    public static Root data;

    public delegate void OnDataLoad();
    public static event OnDataLoad onDataLoad;

    void Start()
    {
        Application.targetFrameRate = 120;
        StartCoroutine(GetRequest(NEWS_API));
    }
    //IEnumerator GetRequest(string url)
    //{
    //    UnityWebRequest request = UnityWebRequest.Get(url);

    //    yield return request.SendWebRequest();

    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        string json = request.downloadHandler.text;
    //        data = JsonUtility.FromJson<Root>(json);
    //        onDataLoad.Invoke();
    //    }
    //    else
    //    {
    //        Debug.Log("Error: " + request.error);
    //    }
    //}
    IEnumerator GetRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Root root = JsonUtility.FromJson<Root>(json);

            // Filter out articles that are not in English
            List<Article> englishArticles = new();
            foreach (Article article in root.articles)
            {
                if (IsEnglish(article))
                {
                    englishArticles.Add(article);
                }
            }
            // Assign the filtered articles to the data
            data = new Root
            {
                status = root.status,
                totalResults = englishArticles.Count,
                articles = englishArticles
            };

            onDataLoad?.Invoke();
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }

    bool IsEnglish(Article article)
    {
        foreach (char c in article.title)
        {
            if (c > 127)
            {
                return false;
            }
        }
        return true;
    }

}