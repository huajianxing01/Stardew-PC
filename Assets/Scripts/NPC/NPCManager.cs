using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonobehaviour<NPCManager>
{
    [SerializeField] private SO_SceneRouteList so_sceneRouteList = null;
    private Dictionary<string, SceneRoute> sceneRouteDictionary;
    [HideInInspector] public NPC[] npcArray;
    private AStar aStar;

    protected override void Awake()
    {
        base.Awake();
        aStar = GetComponent<AStar>();
        npcArray = FindObjectsOfType<NPC>();

        sceneRouteDictionary = new Dictionary<string, SceneRoute>();
        if(so_sceneRouteList.sceneRouteList.Count > 0)
        {
            foreach(var route in so_sceneRouteList.sceneRouteList)
            {
                //如果字典中已有相同路径，跳过这次循环
                if (sceneRouteDictionary.ContainsKey(route.fromSceneName.ToString() + route.toSceneName.ToString()))
                {
                    continue;
                }
                sceneRouteDictionary.Add(route.fromSceneName.ToString() + route.toSceneName.ToString(), route);
            }
        }
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void AfterSceneLoad()
    {
        SetNPCsActiveStatus();
    }

    private void SetNPCsActiveStatus()
    {
        foreach (NPC npc in npcArray)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();
            if (npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            }
            else
            {
                npcMovement.SetNPCInactiveInScene();
            }
        }
    }

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        if (aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        SceneRoute sceneRoute;
        if (sceneRouteDictionary.TryGetValue(fromSceneName + toSceneName, out sceneRoute))
        {
            return sceneRoute;
        }
        else
        {
            return null;
        }
    }
}