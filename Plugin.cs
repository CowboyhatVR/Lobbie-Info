using BepInEx;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR;

[BepInPlugin("com.cowboyhatvr.lobbyinfo", "Lobby Info", "1.0.3")]
public class Plugin : BaseUnityPlugin
{
    private GameObject textObj;
    private TextMesh textMesh;

    private bool hudEnabled = true;
    private bool comboWasHeld = false;

    void Start()
    {
        Logger.LogInfo("Lobby Info loaded!");
        CreateText();
        SetHudVisible(hudEnabled);
    }

    void CreateText()
    {
        textObj = new GameObject("LobbyInfo_Text");
        textMesh = textObj.AddComponent<TextMesh>();

        textMesh.fontSize = 40;
        textMesh.characterSize = 0.02f;
        textMesh.anchor = TextAnchor.LowerCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = Color.white;
    }

    void Update()
    {
        // VR combo toggle
        bool comboHeld = IsToggleComboHeld();
        if (comboHeld && !comboWasHeld)
        {
            hudEnabled = !hudEnabled;
            SetHudVisible(hudEnabled);
            Logger.LogInfo($"Lobby Info HUD: {(hudEnabled ? "ON" : "OFF")}");
        }
        comboWasHeld = comboHeld;

        if (!hudEnabled) return;
        if (textObj == null || textMesh == null) return;

        // Beste camera pakken
        Camera cam = Camera.main;
        if (cam == null)
        {
            var cams = Camera.allCameras;
            for (int i = 0; i < cams.Length; i++)
            {
                if (cams[i] != null && cams[i].isActiveAndEnabled)
                {
                    cam = cams[i];
                    break;
                }
            }
        }
        if (cam == null) return;

        Transform ct = cam.transform;

        // Onderin zicht
        textObj.transform.position = ct.position + ct.forward * 1.5f - ct.up * 0.6f;
        textObj.transform.rotation = Quaternion.LookRotation(textObj.transform.position - ct.position);

        // Lobby info
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
        {
            textMesh.text = "Not in lobby";
            return;
        }

        Room r = PhotonNetwork.CurrentRoom;
        string lobbyName = string.IsNullOrEmpty(r.Name) ? "(unknown)" : r.Name;
        string privacy = r.IsVisible ? "Public" : "Private";
        string players = $"{r.PlayerCount}/{r.MaxPlayers}";

        textMesh.text = $"Lobby: {lobbyName}\n{privacy} | Players: {players}";
    }

    private void SetHudVisible(bool visible)
    {
        if (textObj != null)
            textObj.SetActive(visible);
    }

    private bool IsToggleComboHeld()
    {
        // LEFT: both face buttons (primary + secondary)
        // RIGHT: joystick down (axis.y < -0.7)

        var left = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        var right = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (!left.isValid || !right.isValid) return false;

        bool leftPrimary = false;
        bool leftSecondary = false;
        left.TryGetFeatureValue(CommonUsages.primaryButton, out leftPrimary);
        left.TryGetFeatureValue(CommonUsages.secondaryButton, out leftSecondary);

        Vector2 rightStick = Vector2.zero;
        right.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightStick);

        bool rightDown = rightStick.y < -0.7f;

        return leftPrimary && leftSecondary && rightDown;
    }
}
