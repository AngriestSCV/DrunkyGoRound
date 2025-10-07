
using System;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PawnSpawner : UdonSharpBehaviour
{
    public GameObject core;
    public Transform PawnSpawnLocation;

    public Mesh[] meshes;

    public bool Verbose;

    public GameObject Spawn(VRCPlayerApi player)
    {
        GameObject obj = Instantiate(core);
        Networking.SetOwner(player, obj);

        var phno = obj.GetComponentInChildren<PawnHandleNonOwner>();
        if (phno == null)
        {
            Debug.Log($"When building clone of {obj.name} could not find a {nameof(PawnHandleNonOwner)}");
            Destroy(obj);
            return null;
        }

        phno.DoSetup(player);

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

        return obj;
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        base.OnPlayerJoined(player);

        if (Networking.IsOwner(gameObject))
        {
            if (Verbose)
            {
                VRCPlayerApi local = Networking.LocalPlayer;
                Debug.Log($"Player {local.displayName} spawning pawn for {player.displayName}");
            }
            GameObject pawn = Spawn(player);
            pawn.transform.parent = PawnSpawnLocation;
            pawn.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

}
