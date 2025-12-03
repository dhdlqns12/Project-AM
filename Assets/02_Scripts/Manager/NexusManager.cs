using UnityEngine;
using UnityEngine.SceneManagement;

public class NexusManager : Singleton<NexusManager>
{
    public Nexus playerNexus;
    public Nexus enemyNexus;

    protected override void Init()
    {
        FindNexus();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindNexus();
    }

    public void FindNexus()
    {
        Nexus[] allNexuses = FindObjectsOfType<Nexus>();

        foreach( var nexus in allNexuses)
        {
            if(nexus.Team==Team.Player)
            {
                playerNexus = nexus;
                Debug.Log("플레이어 넥서스 초기화");
            }
            else if(nexus.Team==Team.Enemy)
            {
                enemyNexus = nexus;
                Debug.Log("적 넥서스 초기화");
            }
        }
    }
}

