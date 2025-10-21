using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AngryLabs.Props.DrunkyGoRound
{
    public class PawnHandleNonOwner : UdonSharpBehaviour
    {
        public GameObject PawnBase;

        [UdonSynced]
        public int MeshChoice = -1;

        [UdonSynced]
        public float HueChoice = -1.0f;

        public Mesh[] Meshes;

        public bool verbose = false;

        public void Start()
        {
            if (Networking.IsMaster)
            {
                if (PawnBase == null)
                {
                    Debug.LogError($"Found null pawn base on game object {gameObject}");
                }
                RequestSerialization();
            }
            OnDeserialization();
        }

        public override void OnDeserialization()
        {
            if (MeshChoice < 0)
                return;

            MeshRenderer mr = PawnBase.GetComponentInChildren<MeshRenderer>();
            if (mr == null)
            {
                Debug.Log($"When building clone of {PawnBase.name} could not find a MeshRenderer");
            }

            var mf = PawnBase.GetComponentInChildren<MeshFilter>();
            if (mf == null)
            {
                Debug.Log($"When building clone of {PawnBase.name} cound not find MeshFilter");
            }
            if (mr.materials.Length != 2)
            {
                Debug.Log($"When building clone of {PawnBase.name} found {mr.materials.Length} materials instead of 2");
            }

            Mesh toSpawn = Meshes[MeshChoice];
            mf.mesh = toSpawn;
            Material mainMat = mr.materials[1];

            mainMat.color = Color.HSVToRGB(HueChoice, 1.0f, 1.0f);
        }

        public void SetStats(int meshChoice, float hueChoice)
        {
            if (meshChoice >= Meshes.Length || meshChoice < 0)
            {
                Debug.LogError($"Refusing to set mesh to value of {meshChoice}");
                return;
            }

            MeshChoice = meshChoice;
            HueChoice = hueChoice;
            RequestSerialization();
            OnDeserialization();
        }
    }
}