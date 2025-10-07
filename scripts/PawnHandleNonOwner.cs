
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PawnHandleNonOwner : UdonSharpBehaviour
{
    public Collider Collider;
    public MeshRenderer Renderer;
    private VRCPlayerApi OgOwner;
    public GameObject PawnBase;

    public void DoSetup(VRCPlayerApi newOwner)
    {
        bool setting = Networking.LocalPlayer == newOwner;
        OgOwner = newOwner;
        Collider.enabled = setting;
        Renderer.enabled = setting;
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        base.OnPlayerLeft(player);

        if (player == OgOwner)
        {
            Destroy(gameObject);
        }
    }

    public override void OnDeserialization()
    {
        DoSetup(Networking.GetOwner(PawnBase));
    }
}
