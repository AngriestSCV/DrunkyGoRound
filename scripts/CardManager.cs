using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AngryLabs.Props.DrunkyGoRound {

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class CardManager : UdonSharpBehaviour
    {
        [UdonSynced]
        public int ActiveCardIndex;

        [UdonSynced]
        public int NextCardIndex;

        [UdonSynced]
        public int[] DrawOrder;

        const string dealCard = "deal-card";
        const string returnCard = "return-card";

        public Animator CardAnimator;
        public Collider CardDeckCollider;

        public Transform CardMovementAnchor;

        public bool Verbose;

        public Transform[] AllCards;

        public void Start()
        {
            ActiveCardIndex = -1;
            NextCardIndex = -1;
        }

        public void _DealCard()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            if (NextCardIndex >= AllCards.Length || NextCardIndex < 0)
            {
                DrawOrder = new int[AllCards.Length];
                for (int i = 0; i < AllCards.Length; i++)
                {
                    DrawOrder[i] = i;
                }

                for (int i = 0; i < AllCards.Length; i++)
                {
                    int swapping = UnityEngine.Random.Range(0, AllCards.Length);

                    int a = DrawOrder[i];
                    int b = DrawOrder[swapping];

                    DrawOrder[i] = b;
                    DrawOrder[swapping] = a;
                }
                NextCardIndex = 0;
            }

            if (ActiveCardIndex >= 0)
            {
                _ReturnCard();
                return;
            }

            ActiveCardIndex = NextCardIndex++;
            ActiveCardIndex = DrawOrder[ActiveCardIndex];


            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
             nameof(StartDealCardAnimation));

            if (Verbose)
                Debug.Log($"Cardmanager: Dealt card {AllCards[ActiveCardIndex].gameObject.name}");

            RequestSerialization();
            OnDeserialization();
        }

        public void StartDealCardAnimation()
        {
            CardAnimator.SetTrigger(dealCard);
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();

            if (Verbose)
                Debug.Log($"CardManager: OnDeserialization");

            foreach (var card in AllCards)
            {
                card.gameObject.SetActive(false);
            }

            if (ActiveCardIndex >= 0 && ActiveCardIndex < AllCards.Length)
            {
                Transform active = AllCards[ActiveCardIndex];
                active.gameObject.SetActive(true);


                active.SetParent(CardMovementAnchor, false);
                active.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                if (active.parent != CardMovementAnchor)
                {
                    Debug.LogError($"CardManager: Setting the card parent seems to have failed");
                }
            }
        }

        public void _ReturnCard()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ReturnCardRpc));
        }

        public void ReturnCardRpc()
        {
            ActiveCardIndex = -1;

            CardDeckCollider.enabled = true;
            CardAnimator.SetTrigger(returnCard);

            IssueFinishReset();
            RequestSerialization();
        }

        public void _FinishReset()
        {
            CardDeckCollider.enabled = true;
        }

        private void IssueFinishReset()
        {
            if (Verbose)
                Debug.Log("CardManager: Issuing Finish Reset");
            SendCustomEventDelayedSeconds(nameof(_FinishReset), 3.0f);
        }
    }

}