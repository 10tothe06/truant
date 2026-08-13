using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum TextMode {
    HoverPermanent,
    HoverTemporary,
}

// WIP
// this is a class that handles everything debug related
// rendering debug objects, the debug menu, etc.

// probably what I'll do is make debug rendering separate but idk
public class Db : MonoBehaviour
{
    // SINGLETON MAKES A COMEBACK
    private static Db _instance;

    public static Db Instance {
        get => _instance;
        private set {
            if (_instance == null) {
                _instance = value;
            }
            else if (_instance != value) {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    private Color debugColor;
    public Shader debugShader;
    public Transform debugObjectContainer;
    public Mesh sphereMesh;
    public Material debugRendering;
    public Texture debugRenderTexture;
    public List<GameObject> activeDebugObjects;

    // current entries in the debug menu
    public List<TextMeshProUGUI> entries;
    public List<float> spawnTimes;
    public GameObject textPrefab;
    // the object under which all the text objects are
    public Transform textContainer;
    // how long can an object go without an update before it gets deleted
    [Space(8)]
    [Header("Organization")]
    public float entrySpacing;
    [Space(8)]
    [Header("Persistence")]
    public bool despawnWhenInactive;
    public float despawnTimer;

    public bool shouldDrawDebugStuff;

    private void Awake()
    {
        Instance = this;
        entries = new List<TextMeshProUGUI>();
    }

    public static void Put(string key, float msg) {
        Put(key, msg.ToString());
    }

    public static void Put(string key, Vector3 msg) {
        Put(key, msg.ToString());
    }

    public static void Put(string key, string msg) {
        for (int i = 0; i < _instance.entries.Count; i++) {
            if (_instance.entries[i].gameObject.name == key) {
                _instance.entries[i].text = key + ": " + msg;
                return;
            }
        }

        // text obj not found, so make a new one
        GameObject newTextObj = Instantiate(_instance.textPrefab, Vector3.zero, Quaternion.identity);
        newTextObj.transform.SetParent(_instance.textContainer);

        newTextObj.name = key;

        _instance.entries.Add(newTextObj.GetComponent<TextMeshProUGUI>());
        _instance.spawnTimes.Add(Time.time);

        newTextObj.GetComponent<TextMeshProUGUI>().text = key + ": " + msg;

        newTextObj.transform.position = _instance.textContainer.transform.position + -Vector3.up * _instance.entrySpacing * _instance.entries.Count;
    }

    void Update() {
        if (despawnWhenInactive) {
            for (int i = 0; i < entries.Count; i++) {
                if (Time.time > spawnTimes[i] + despawnTimer) {
                    Destroy(entries[i].gameObject);

                    spawnTimes.RemoveAt(i);
                    entries.RemoveAt(i);

                    break;
                }
            }
        }

        // destroy any debug objects that are out of time
        for (int i = 0; i < activeDebugObjects.Count; i++) {
            if (activeDebugObjects[i].name[0] == "P"[0] && activeDebugObjects[i].name[1] == "_"[0]) {
                // the object is a parent object, used for attaching debug things to other objects
                // i.e, it serves a different purpose

                if (activeDebugObjects[i].transform.childCount == 0) {
                    // parent object w/ no children = die
                    Destroy(activeDebugObjects[i]);
                    activeDebugObjects.RemoveAt(i);
                    break;
                }

                // remove the P_ from the name
                char[] rawName = activeDebugObjects[i].name.ToCharArray();
                char[] correctedName = new char[rawName.Length - 2];
                for (int j = 2; j < rawName.Length; j++) {
                    correctedName[j - 2] = rawName[j];
                }

                string correctedNameString = new string(correctedName);

                // set the parent object's position to the object it is tracking
                if (GameObject.Find(correctedNameString) != null) {
                    activeDebugObjects[i].transform.position =
                    GameObject.Find(correctedNameString).transform.position;
                }

                // TEMP code for making text look at player
                for (int j = 0; j < activeDebugObjects[i].transform.childCount; j++) {
                    activeDebugObjects[i].transform.GetChild(j).forward =
                    activeDebugObjects[i].transform.GetChild(j).position - GameObject.Find("player").transform.position;
                }
            }
            else if (activeDebugObjects[i].name.Contains("(")) {
                if (Time.time > float.Parse(activeDebugObjects[i].name.Substring(activeDebugObjects[i].name.IndexOf("(") + 1, activeDebugObjects[i].name.IndexOf(")") - activeDebugObjects[i].name.IndexOf("(") - 1))) {
                    Destroy(activeDebugObjects[i]);
                    activeDebugObjects.RemoveAt(i);
                    break;
                }
            }
            else {
                if (Time.time > float.Parse(activeDebugObjects[i].name)) {
                    Destroy(activeDebugObjects[i]);
                    activeDebugObjects.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void SetDebugColor(Color newColor) {
        debugColor = newColor;
    }

    public void ShowCollisionPoint(Vector3 _point) {
        // debugColor = Color.blue;
        // DrawDebugSphere(_point, 0.3f, 0.6f); // draw a sphere at the collision point and delete it after 0.6 seconds
    }

    public void DrawDebugText(TextMode drawMode, Transform _parent, float _heightOffset, string _data, string _title) {
        string fullTitle = drawMode == TextMode.HoverPermanent ? _title : "(" + (Time.time + 2).ToString() + ")" + _title;

        GameObject newParent;
        // create a parent object for the text
        if (!TryGetParent(_parent.gameObject.name)) {
            newParent = new GameObject();
            newParent.name = "P_" + _parent.gameObject.name;
            newParent.transform.SetParent(debugObjectContainer);
            activeDebugObjects.Add(newParent);
        }
        else {
            newParent = GetParent(_parent.gameObject.name);
        }

        // create the text object
        GameObject newObject;

        if (!TryGetTextObject(newParent, _title)) {
            newObject = new GameObject();

            newObject.transform.SetParent(newParent.transform);
            newObject.transform.localPosition = Vector3.up * _heightOffset;
            
            TextMeshPro textComponent = newObject.AddComponent<TextMeshPro>();

            textComponent.text = _data;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = 2;
            newObject.name = _title;

            newObject.layer = 9;

            if (drawMode == TextMode.HoverTemporary) {
                // 1 second timer for now
                newObject.name = fullTitle;
                activeDebugObjects.Add(newObject);
            }
        }
        else {
            newObject = GetTextObject(newParent, _title);

            TextMeshPro textComponent = newObject.GetComponent<TextMeshPro>();
            textComponent.text = _data;
        }
    }

    public void DrawDebugSphere(Vector3 _centre, float _radius, float _activeTime) {
        GameObject newObject = new GameObject();
        newObject.transform.SetParent(debugObjectContainer);
        newObject.transform.position = _centre;
        newObject.transform.localScale = Vector3.one * _radius;

        newObject.AddComponent<MeshFilter>().mesh = sphereMesh;
        Material newMaterial = new Material(debugShader);
        newMaterial.SetColor("_Albedo", debugColor);
        newObject.AddComponent<MeshRenderer>().material = newMaterial;
        newObject.name = (Time.time + _activeTime).ToString();
        activeDebugObjects.Add(newObject);

        newObject.layer = 9;
    }

    void OnRenderImage(RenderTexture source, RenderTexture mod) {
        if (shouldDrawDebugStuff)
        {
            Graphics.Blit(source, mod, debugRendering);
        }
        else
        {
            Graphics.Blit(source, mod);
        }
    }

    public string RemoveTimerSegment(string _input) {
        return _input.Substring(_input.IndexOf(")") + 1, _input.Length - _input.IndexOf(")") - 1);
    }

    public bool TryGetTextObject(GameObject _parent, string _title) {
        for (int i = 0; i < _parent.transform.childCount; i++) {
            if (RemoveTimerSegment(_parent.transform.GetChild(i).gameObject.name) == _title) {
                return true;
            }
        }

        return false;
    }

    public GameObject GetTextObject(GameObject _parent, string _title) {
        for (int i = 0; i < _parent.transform.childCount; i++) {
            if (RemoveTimerSegment(_parent.transform.GetChild(i).gameObject.name) == _title) {
                return _parent.transform.GetChild(i).gameObject;
            }
        }

        return null;
    }

    public bool TryGetParent(string _name) {
        for (int i = 0; i < activeDebugObjects.Count; i++) {
            // check if parent object
            if (activeDebugObjects[i].name[0] == "P"[0] && activeDebugObjects[i].name[1] == "_"[0]) {
                // remove the P_ from the name
                char[] rawName = activeDebugObjects[i].name.ToCharArray();
                char[] correctedName = new char[rawName.Length - 2];
                for (int j = 2; j < rawName.Length; j++) {
                    correctedName[j - 2] = rawName[j];
                }

                string correctedNameString = new string(correctedName);

                if (correctedNameString == _name) {
                    return true;
                }
            }
        }

        return false;
    }

    public GameObject GetParent(string _name) {
        for (int i = 0; i < activeDebugObjects.Count; i++) {
            // check if parent object
            if (activeDebugObjects[i].name[0] == "P"[0] && activeDebugObjects[i].name[1] == "_"[0]) {
                // remove the P_ from the name
                char[] rawName = activeDebugObjects[i].name.ToCharArray();
                char[] correctedName = new char[rawName.Length - 2];
                for (int j = 2; j < rawName.Length; j++) {
                    correctedName[j - 2] = rawName[j];
                }

                string correctedNameString = new string(correctedName);

                if (correctedNameString == _name) {
                    return activeDebugObjects[i];
                }
            }
        }

        return null;
    }
}
