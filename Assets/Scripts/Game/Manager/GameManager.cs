using ProjectFortrest.Game.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public static GameManager Instance {
        get {
            if(_instance != null) return _instance;
            return _instance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
    }

    public NetworkManager NetworkManager { get; }
    public KeyBindingsManager KeyBindings { get; }
    public LevelManager LevelManager { get; }
    public EventManager EventManager { get; }
    public BlockManager BlockManager;

    public GameObject TileLevelPrefab;

    public GameManager() {
        NetworkManager = new NetworkManager();
        EventManager = new EventManager();
        KeyBindings = new KeyBindingsManager();
        LevelManager = new LevelManager();

        NetworkManager.Init(this);
        EventManager.Init(this);
        KeyBindings.Init(this);
        LevelManager.Init(this);
    }

    void Awake() {
        NetworkManager.OnStart();
        EventManager.OnStart();
        KeyBindings.OnStart();
        LevelManager.OnStart();
        BlockManager.OnStart();
    }

    void Start() {
        
    }

    void Update() {
        
    }
}
