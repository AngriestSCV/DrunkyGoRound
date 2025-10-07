
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class CardManager : UdonSharpBehaviour
{
    const string dealCard = "deal-card";
    const string returnCard = "return-card";

    public Animator CardAnimator;
    public VRCObjectPool Pool;
    public Collider CardDeckCollider;

    public Transform CardMovementAnchor;

    public bool Verbose;

    GameObject Card;

    public void _DealCard()
    {
        Pool.Shuffle();

        if (Card != null)
        {
            _ReturnCard();
            return;
        }

        Card = Pool.TryToSpawn();
        if (Card == null)
        {
            Debug.LogError($"CardManager: Card was null when spawning.");
            return;
        }


        Card.transform.SetParent(CardMovementAnchor, false);
        Card.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (Card.transform.parent != CardMovementAnchor)
        {
            Debug.LogError($"CardManager: Setting the card parent seems to have failed");
        }

        CardAnimator.SetTrigger(dealCard);

        if (Verbose)
            Debug.Log($"Cardmanager: Dealt card {Card.name}");
    }

    private void ReturnCardToPool()
    {
        if (Card != null)
        {
            if (Verbose)
                Debug.Log($"CardManager: Setting Card {Card.name} to inactive and returning to pool");

            Card.SetActive(false);
            Pool.Return(Card);
        }
        Card = null;
    }


    public void _ReturnCard()
    {
        string cardName;

        if (Card == null)
            cardName = "[null]";
        else
            cardName = Card.name;

        if (Verbose)
            Debug.Log($"Returning card: {cardName}");

        CardDeckCollider.enabled = true;
        CardAnimator.SetTrigger(returnCard);

        IssueFinishReset();
    }

    public void _FinishReset()
    {
        ReturnCardToPool();
        CardDeckCollider.enabled = true;
    }

    private void IssueFinishReset()
    {
        if (Verbose)
            Debug.Log("CardManager: Issuing Finish Reset");
        SendCustomEventDelayedSeconds(nameof(_FinishReset), 3.0f);
    }

}
