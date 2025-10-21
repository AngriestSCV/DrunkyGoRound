using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AngryLabs.Props.DrunkyGoRound
{
    public class PawnSpawner : UdonSharpBehaviour
    {
        public int NumMeshes = 3;
        public GameObject[] AllPawns;

        public void Start()
        {
            if (Networking.IsMaster)
            {
                var pawnCount = AllPawns.Length;
                float colorFraction = 1.0f / pawnCount;

                for(int i=0; i<pawnCount; i++)
                {
                    GameObject pawn = AllPawns[i];
                    var nl = pawn.GetComponentInChildren<PawnHandleNonOwner>();

                    nl.SetStats(i % NumMeshes, colorFraction * i);
                }
            }
        }
    }
}