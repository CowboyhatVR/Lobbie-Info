using BepInEx;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[BepInPlugin("com.cowboyhatvr.lobbyinfo", "Lobby Info", "1.0.1")]
public class Plugin : BaseUnityPlugin
{
    private GameObject textObj;
    private TextMesh textMesh;

    void Start()
    {
        Logger.LogInfo("Lobby Info loaded!");
        CreateText();
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
        if (textObj == null || textMesh == null) return;

        // Beste camera pakken (werkt beter in VR én soms in flatscreen)
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

        // Positie onderin je zicht (VR-friendly)
        textObj.transform.position = ct.position + ct.forward * 1.5f - ct.up * 0.6f;
        textObj.transform.rotation = Quaternion.LookRotation(textObj.transform.position - ct.position);

        // Tekst inhoud
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
}
