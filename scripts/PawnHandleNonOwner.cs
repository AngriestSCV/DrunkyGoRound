using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AngryLabs.Props.DrunkyGoRound
{
    public class PawnHandleNonOwner : UdonSharpBehaviour
    {
        public Collider Collider;
        public MeshRenderer Renderer;
        public GameObject PawnBase;

        [UdonSynced]
        public int OwningPlayerId = -1;

        [UdonSynced]
        public Vector3 StartingPosition;

        public bool verbose = false;

        public void Start()
        {
            if (Networking.IsMaster)
            {
                if (PawnBase == null)
                {
                    Debug.LogError($"Found null pawn base on game object {gameObject}");
                }
                else
                {
                    StartingPosition = PawnBase.transform.position;
                }
                RequestSerialization();
            }
            OnDeserialization();
        }

        public void AssignPlayer(VRCPlayerApi newOwner)
        {
            if (newOwner == null)
            {
                PawnBase.SetActive(false);
                PawnBase.transform.SetPositionAndRotation(StartingPosition, Quaternion.identity);
                return;
            }

            if (verbose)
                Debug.Log($"Assigning player {newOwner} to {PawnBase.gameObject.name}");

            PawnBase.SetActive(true);
            OwningPlayerId = newOwner.playerId;

            RequestSerialization();
            OnDeserialization();
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (verbose)
                Debug.Log($"PawnHandleNonOwner::OnPlayerLeft - Player {player.displayName}:{player.playerId}");

            base.OnPlayerLeft(player);

            if (player.playerId == OwningPlayerId)
            {
                AssignPlayer(null);
                OwningPlayerId = -1;
            }
        }

        public override void OnDeserialization()
        {
            VRCPlayerApi local = Networking.LocalPlayer;
            if (verbose)
                Debug.Log($"PawnHandleNonOwner::OnDeserialization - Called on {local.displayName}:{local.playerId} for pawn: {PawnBase.name} owned by {OwningPlayerId}");

            bool setting = local.playerId == OwningPlayerId;
            Collider.enabled = setting;
            Renderer.enabled = setting;

            PawnBase.SetActive(OwningPlayerId != -1);
        }
    }
}