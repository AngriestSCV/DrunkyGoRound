using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AngryLabs.Props.DrunkyGoRound
{
    public class PawnSpawner : UdonSharpBehaviour
    {
        public Transform PawnSpawnLocation;

        public Mesh[] meshes;

        public bool Verbose;

        public GameObject[] AllPawns;

        public GameObject Spawn(VRCPlayerApi player)
        {
            GameObject obj = FindPawn();
            if (obj == null)
            {
                Debug.LogError("Could not find a pawn");
                return null;
            }

            Networking.SetOwner(player, obj);

            var phno = obj.GetComponentInChildren<PawnHandleNonOwner>();
            if (phno == null)
            {
                Debug.Log($"When building clone of {obj.name} could not find a {nameof(PawnHandleNonOwner)}");
                Destroy(obj);
                return null;
            }

            phno.AssignPlayer(player);

            var mr = obj.GetComponentInChildren<MeshRenderer>();
            if (mr == null)
            {
                Debug.Log($"When building clone of {obj.name} could not find a MeshRenderer");
                Destroy(obj);
                return null;
            }

            var mf = obj.GetComponentInChildren<MeshFilter>();
            if (mf == null)
            {
                Debug.Log($"When building clone of {obj.name} cound not find MeshFilter");
                Destroy(obj);
                return null;
            }

            if (mr.materials.Length != 2)
            {
                Debug.Log($"When building clone of {obj.name} found {mr.materials.Length} materials instead of 2");
                Destroy(obj);
                return null;
            }

            int index = UnityEngine.Random.Range(0, meshes.Length);
            Mesh toSpawn = meshes[index];
            mf.mesh = toSpawn;

            Material mainMat = mr.materials[1];

            // TODO make this synced
            mainMat.color = Color.HSVToRGB(UnityEngine.Random.Range(0, 1.0f), 1.0f, 1.0f);

            obj.transform.parent = PawnSpawnLocation;
            obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            return obj;
        }

        private GameObject FindPawn()
        {
            foreach (var pawn in AllPawns)
            {
                // var phno = pawn.GetComponentInChildren<PawnHandleNonOwner>();
                // if (phno == null)
                // {
                //     Debug.LogError($"When building clone of {pawn.name} could not find a {nameof(PawnHandleNonOwner)}");
                //     continue;
                // }
                // if( phno.OwningPlayerId < 0)
                //     return pawn;

                if (!pawn.activeSelf)
                    return pawn;
            }
            return null;
        }


        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            base.OnPlayerJoined(player);

            if (Verbose)
            {
                VRCPlayerApi local = Networking.LocalPlayer;
                Debug.Log($"Player {local.displayName} spawning pawn for {player.displayName}");
            }

            GameObject pawn = Spawn(player);
            if (pawn == null)
                Debug.LogWarning("Failed to spawn pawn");
            else if (Verbose)
                Debug.Log($"Spaned pawn: {pawn.name}");
        }
    }
}