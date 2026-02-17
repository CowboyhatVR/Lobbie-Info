using BepInEx;
using UnityEngine;

[BepInPlugin("com.cowboyhatvr.lobbyinfo.dinguswalker", "DingusWalker (Phase A)", "0.1.0")]
public class Plugin : BaseUnityPlugin
{
    private GameObject _cube;
    private Transform _anchor;           // Waar we omheen lopen
    private float _t;                    // tijd
    private float _lastAnchorSearchTime; // zodat we niet constant zoeken

    private void Awake()
    {
        Logger.LogInfo("Phase A loaded!");
    }

    private void Update()
    {
        // 1) Af en toe (bv. 2x per sec) checken of we een 'stump anchor' kunnen vinden
        if (Time.time - _lastAnchorSearchTime > 0.5f)
        {
            _lastAnchorSearchTime = Time.time;
            _anchor = FindStumpAnchor();
        }

        // 2) Alleen actief als we in stump zijn (anchor gevonden)
        bool inStump = _anchor != null;

        if (!inStump)
        {
            // Als we de cube al hebben: verberg 'm
            if (_cube != null) _cube.SetActive(false);
            return;
        }

        // 3) Zorg dat cube bestaat
        EnsureCube();
        _cube.SetActive(true);

        // 4) Rondlopen in een cirkel om de anchor
        _t += Time.deltaTime;

        float radius = 1.2f;
        float speed = 0.7f; // lager = langzamer
        float bob = 0.08f;

        // tijdelijk: spawn bij speler
        Transform cam = Camera.main.transform;
        Vector3 center = cam.position + cam.forward * 1.5f - cam.up * 0.4f;
        float angle = _t * Mathf.PI * 2f * speed;

        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
        offset.y = Mathf.Sin(_t * 2.5f) * bob;

        _cube.transform.position = center + offset;

        // Laat de cube “kijken” waar hij heen loopt
        Vector3 forward = new Vector3(-Mathf.Sin(angle), 0f, Mathf.Cos(angle));
        if (forward.sqrMagnitude > 0.001f)
            _cube.transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
    }

    private void EnsureCube()
    {
        if (_cube != null) return;

        _cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _cube.name = "DingusPhaseA_Cube";
        _cube.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);

        // Optioneel: kleur
        var renderer = _cube.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Standard"));
            renderer.material.color = new Color(0.2f, 0.9f, 0.9f);
        }

        // Optioneel: collider uitzetten (zodat hij niets “raakt”)
        var col = _cube.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Logger.LogInfo("Spawned Phase A cube.");
    }

    // “Stump-only” detectie:
    // We proberen een object te vinden dat (waarschijnlijk) in de stump-map zit.
    private Transform FindStumpAnchor()
    {
        // 1) Snelste gok: object heet letterlijk "Stump"
        var direct = GameObject.Find("Stump");
        if (direct != null) return direct.transform;

        // 2) Iets ruimer: zoek op naam die "stump" bevat (niet elke frame!)
        // Let op: dit is wat zwaarder, daarom doen we het maar 2x per seconde.
        var all = GameObject.FindObjectsOfType<Transform>();
        foreach (var t in all)
        {
            if (t == null || string.IsNullOrEmpty(t.name)) continue;

            // veel mods/maps hebben "Stump" ergens in de hierarchy
            if (t.name.ToLowerInvariant().Contains("stump"))
                return t;
        }

        return null;
    }
}