using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Select : MonoBehaviour
{
    private PlayerProgress playerProgress;

    private GameObject[] stages;

    [SerializeField]
    private GameObject levels;

    private int levelReveal;

    // Start is called before the first frame update
    void Start() {
        playerProgress = GameObject.Find("PlayerProgress").GetComponent<PlayerProgress>();
        levelReveal = 0;
        Debug.Log(playerProgress.getStagesComplet);
        for (int i = 0; i <= playerProgress.getStagesComplet; i++)
        {
            levels.transform.GetChild(i).GetComponent<Level_Manager>().unlock();
        }

        InvokeRepeating("RevealLevels", 0.8f, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LoadStage(int newStage)
    {
        if (!levels.transform.GetChild(newStage - 1).GetComponent<Level_Manager>().locked)
        {
            Debug.Log("unlocked");
            GameObject.Find("SFX").GetComponent<SFX_Manager>().PlayMenuClick();
            SceneManager.LoadScene(newStage);
        } else
        {
            // Could play different sound if stage is locked
            Debug.Log("is locked");
        }
        
    }

    public void RevealLevels()
    {
        if (levelReveal >7)
        {
            CancelInvoke();
        }
        else {
            levels.transform.GetChild(levelReveal).gameObject.SetActive(true);
            GameObject.Find("SFX").GetComponent<SFX_Manager>().PlayGroundHit();
            levelReveal++;
        }
        
    }
}
