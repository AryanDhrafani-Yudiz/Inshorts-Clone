using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform CardParent;

    private int numberOfCards = 7;
    private int halfNumbers => numberOfCards / 2;
    private int currIndex = 3;

    private Card currentCard;
    private List<Card> cardList = new List<Card>();

    public delegate void OnNextPage();
    public static event OnNextPage onNextPage;
    public delegate void OnPreviousPage();
    public static event OnPreviousPage onPreviousPage;

    private void Awake()
    {
        onNextPage += OnNext;
        onPreviousPage += OnPrevious;
        DataManager.onDataLoad += InitializeCard;
    }
    public static void TriggerNextPage()
    {
        onNextPage?.Invoke();
    }
    public static void TriggerPreviousPage()
    {
        onPreviousPage?.Invoke();
    }
    public void InitializeCard()
    {
        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, CardParent);
            var card = cardObj.GetComponent<Card>();
            card.ind = i;
            cardList.Add(card);
            card.LoadData();
            card.DisableCard();
        }
        currentCard = cardList[currIndex];
        UpdateCurrentCard();
    }
    void OnNext()
    {
        foreach (var card in cardList)
        {
            if (card.ind == currIndex + halfNumbers + 1)
            {
                currentCard.DisableCard();
                card.LoadData();
                currIndex++;
                UpdateCurrentCard();
                return;
            }
        }
        foreach (var card in cardList)
        {
            if (currIndex == card.ind + halfNumbers)
            {
                card.ind = currIndex + halfNumbers + 1;
                currentCard.DisableCard();

                card.LoadData();
                currIndex++;
                UpdateCurrentCard();
                return;
            }
        }
    }
    void OnPrevious()
    {
        if (currIndex == 1)
        {
            return;
        }
        if (currIndex - halfNumbers <= 0)
        {
            currIndex--;
            currentCard.DisableCard();
            UpdateCurrentCard();
            return;
        }
        foreach (var card in cardList)
        {
            if (currIndex == card.ind - halfNumbers)
            {
                card.ind = currIndex - halfNumbers - 1;
                card.LoadData();

                currentCard.DisableCard();

                currIndex--;
                UpdateCurrentCard();
                return;
            }
        }
    }
    private void UpdateCurrentCard()
    {
        foreach (var card in cardList)
        {
            if (currIndex == card.ind)
            {
                currentCard = card;
                currentCard.EnableCard();

                SetPreviousCard();
                break;
            }
        }
    }
    private void SetPreviousCard()
    {
        foreach (var card in cardList)
        {
            if (currIndex - 1 == card.ind && currentCard != null)
            {
                currentCard.swipeEffect.previousCard = card.swipeEffect;
                currentCard.swipeEffect.isPreviousCardMovable = currIndex != 1;
                card.SetPreviousCard();
            }
            if (currIndex + 1 == card.ind)
            {
                card.SetNextCard();
            }
            if (currIndex + 2 == card.ind || currIndex - 2 == card.ind) card.DisableCard();
        }
    }
}