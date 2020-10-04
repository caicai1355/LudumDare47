using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject gameCanvas;
    public List<GameObject> dontDestroy;

    public AnimationClip EnemyDisappear;
    public AnimationClip EnemyHurt;

    public AudioSource bgmAudioSource;
    public AudioSource effectAudioSource;
    public AudioClip titleAudio;
    public AudioClip battleAudio;
    public AudioClip setAudio;
    public AudioClip effectClear;
    public AudioClip effectDead;
    public AudioClip effectHit;
    public AudioClip effectStep;
    public AudioClip effectWall;
    public AudioClip effectDog;
    public AudioClip effectOpenDoor;

    [HideInInspector] public bool isStart;
    [HideInInspector] public bool canControl;
    [HideInInspector] public List<int> stateList;
    [HideInInspector] public int currStateLength;
    [HideInInspector] public List<int> skillIndexList;

    [HideInInspector] public int currLevel;
    [HideInInspector] public bool firstFrame;
    [HideInInspector] public int currCameraLevel;
    [HideInInspector] public int audioType;
    [HideInInspector] public DirectionType currStartDirection;


    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        foreach (var obj in dontDestroy)
        {
            DontDestroyOnLoad(obj);
        }
        firstFrame = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(firstFrame)
        {
            firstFrame = false;
            OpenTitle();
        }

    }

    public void OpenTitle()
    {
        gameCanvas.SetActive(false);
        SceneManager.LoadScene("Title");
    }

    public void FirstInit()
    {
        gameCanvas.SetActive(true);
        currLevel = 1 - 1;
        currCameraLevel = 1;
        isStart = false;
        canControl = true;
        InitNext();
    }

    public void InitNext()
    {
        isStart = false;
        canControl = false;
        currLevel += 1;
        Init(currLevel);
        StartCoroutine(WaitStart());
    }

    public void MissionClear()
    {
        UIManager.instance.SetStartOrClear(false);
        StartCoroutine(WaitNext());
        canControl = false;
    }

    public IEnumerator WaitStart()
    {
        yield return new WaitForSeconds(2);
        canControl = true;
    }

    public IEnumerator WaitNext()
    {
        yield return new WaitForSeconds(2);
        InitNext();
    }

    public void Init(int level)
    {
        string sceneName = "Level" + level;
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            //SceneManager.LoadScene(sceneName);
            //UIManager.instance.Init();
            StopAudio();
            StartCoroutine(LoadSceneAsynByName(sceneName));
        }
        else
        {
            //Debug.LogError("没有关卡 "+sceneName);
            StartCoroutine(GameEnd());
        }
    }

    public IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(5);
        OpenTitle();
    }

    IEnumerator LoadSceneAsynByName(string sceneName)
    {
        SetSceneSkill();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        UIManager.instance.Init();
    }

    private void SetSceneSkill()
    {
        skillIndexList.Clear();
        switch (currLevel)
        {
            case 1:
                currStartDirection = DirectionType.up;
                currCameraLevel = 1;
                currStateLength = 1;
                audioType = 1;
                skillIndexList.Add(1);
                skillIndexList.Add(2);
                skillIndexList.Add(3);
                skillIndexList.Add(0);
                break;
            case 2:
                currStartDirection = DirectionType.up;
                currCameraLevel = 1;
                currStateLength = 5;
                audioType = 1;
                skillIndexList.Add(1);
                skillIndexList.Add(2);
                skillIndexList.Add(3);
                skillIndexList.Add(0);
                break;
            case 3:
                currStartDirection = DirectionType.up;
                currCameraLevel = 2;
                currStateLength = 15;
                audioType = 1;
                skillIndexList.Add(1);
                skillIndexList.Add(2);
                skillIndexList.Add(3);
                skillIndexList.Add(0);
                break;
            case 4:
                currStartDirection = DirectionType.up;
                currCameraLevel = 2;
                currStateLength = 3;
                audioType = 1;
                skillIndexList.Add(1);
                skillIndexList.Add(2);
                skillIndexList.Add(3);
                skillIndexList.Add(0);
                break;
            case 5:
                currStartDirection = DirectionType.up;
                currCameraLevel = 2;
                currStateLength = 2;
                audioType = 1;
                skillIndexList.Add(1);
                skillIndexList.Add(4);
                skillIndexList.Add(0);
                break;
            case 6:
                currStartDirection = DirectionType.down;
                currCameraLevel = 2;
                currStateLength = 12;
                audioType = 1;
                skillIndexList.Add(1);
                skillIndexList.Add(2);
                skillIndexList.Add(3);
                skillIndexList.Add(4);
                skillIndexList.Add(0);
                break;
            case 7:
                currStartDirection = DirectionType.left;
                currCameraLevel = 2;
                currStateLength = 15;
                audioType = 1;
                skillIndexList.Add(1);
                skillIndexList.Add(2);
                skillIndexList.Add(3);
                skillIndexList.Add(4);
                skillIndexList.Add(0);
                break;
            case 8:
                currStartDirection = DirectionType.up;
                currCameraLevel = 2;
                currStateLength = 15;
                audioType = 1;
                skillIndexList.Add(1);
                skillIndexList.Add(2);
                skillIndexList.Add(3);
                skillIndexList.Add(4);
                skillIndexList.Add(0);
                break;
            case 9:
                currStartDirection = DirectionType.up;
                currCameraLevel = 2;
                currStateLength = 2;
                audioType = 2;
                skillIndexList.Add(1);
                skillIndexList.Add(2);
                skillIndexList.Add(3);
                skillIndexList.Add(4);
                skillIndexList.Add(0);
                break;
            case 10:
                currStartDirection = DirectionType.up;
                currCameraLevel = 2;
                currStateLength = 2;
                audioType = 3;
                skillIndexList.Add(1);
                skillIndexList.Add(2);
                skillIndexList.Add(3);
                skillIndexList.Add(4);
                skillIndexList.Add(0);
                break;
        }
    }

    public void StopAudio()
    {
        bgmAudioSource.Stop();
    }

    public void PlaySetAudio()
    {
        if(audioType == 1)
        {
            bgmAudioSource.Stop();
            bgmAudioSource.clip = setAudio;
            bgmAudioSource.Play();
        }
        else if(audioType == 2)
        {
            bgmAudioSource.Stop();
            bgmAudioSource.clip = battleAudio;
            bgmAudioSource.Play();
        }
        else if(audioType == 3)
        {
            bgmAudioSource.Stop();
        }
    }

    public void StartAudio()
    {
        if (audioType != 2)
        {
            bgmAudioSource.Stop();
        }
    }

    public void PlayEffect(int index,bool isForce = false)
    {
        switch(index)
        {
            case 1:
                effectAudioSource.clip = effectClear;
                break;
            case 2:
                effectAudioSource.clip = effectDead;
                break;
            case 3:
                effectAudioSource.clip = effectHit;
                break;
            case 4:
                effectAudioSource.clip = effectStep;
                break;
            case 5:
                effectAudioSource.clip = effectWall;
                break;
            case 6:
                effectAudioSource.clip = effectDog;
                break;
            case 7:
                effectAudioSource.clip = effectOpenDoor;
                break;
            default:
                return;
        }
        effectAudioSource.Play();
    }
}
