using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SkillItem
{
    public int index;
    public Button button;
    public Image image;
}

public class StateItem
{
    public int index;
    public Button button;
    public Image image;
    public Image selectImage;
}

public class UIManager : MonoBehaviour
{
    public GameObject loopListObj;
    public GameObject skillListObj;
    public GameObject runingObj;
    public Text clearText;
    public Text startText;
    public Button startBtn;
    public Button restartBtn;
    public Button cameraBtn;
    public Image cameraSprite;
    public Sprite skillGoSprite;
    public Sprite skillLeftSprite;
    public Sprite skillRightSprite;
    public Sprite skillNoneSprite;
    public Sprite skillAtkSprite;
    public Sprite loopSprite;
    public Sprite selectSprite;
    public Sprite currStateSprite;
    public List<Sprite> cameraBtnSpriteList;


    //0空 1前进 2左90 3右90
    [HideInInspector] public static UIManager instance;

    private List<SkillItem> skillItemList;
    private List<StateItem> stateItemList;
    private StateItem currStateItem;
    private int currCameraMultiNum;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.skillIndexList = new List<int>();

        int i = 0;
        skillItemList = new List<SkillItem>();
        foreach (Transform obj in skillListObj.transform)
        {
            skillItemList.Add(SkillItemInit(obj,i));
            i++;
        }

        i = 0;
        stateItemList = new List<StateItem>();
        foreach (Transform obj in loopListObj.transform)
        {
            stateItemList.Add(StateItemInit(obj,i));
            i++;
        }

        startBtn.onClick.AddListener(StartClick);
        restartBtn.onClick.AddListener(RestartClick);
        cameraBtn.onClick.AddListener(CameraClick);
    }

    void StartClick()
    {
        StartClickUpdate(false);
    }

    void RestartClick()
    {
        RestartClickUpdate(false);
    }

    void CameraClick()
    {
        CameraClickUpdate(false);
    }

    void StartClickUpdate(bool notPress = false)
    {
        if (notPress == false && GameManager.instance.canControl == false) return;
        GameManager.instance.isStart = true;
        if (currStateItem != null)
        {
            StateItemSelectSet(currStateItem.selectImage, 0);
        }
        GameManager.instance.StartAudio();
    }

    void RestartClickUpdate(bool notPress = false)
    {
        if (notPress == false && GameManager.instance.canControl == false) return;
        //GameManager.instance.StopAudio();
        //GameManager.instance.isStart = false;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Init();
        GameManager.instance.Init(GameManager.instance.currLevel);
    }

    void CameraClickUpdate(bool notPress = false)
    {
        if (notPress == false && GameManager.instance.canControl == false) return;
        currCameraMultiNum += 1;
        if(currCameraMultiNum > cameraBtnSpriteList.Count)
        {
            currCameraMultiNum = 1;
        }
        switch(currCameraMultiNum)
        {
            case 1:
                Camera.main.orthographicSize = 6;
                break;
            case 2:
                Camera.main.orthographicSize = 8;
                break;
            case 3:
                Camera.main.orthographicSize = 12;
                break;
        }
        cameraSprite.sprite = cameraBtnSpriteList[currCameraMultiNum - 1];
    }

    // Update is called once per frame
    void Update()
    {
        runingObj.SetActive(GameManager.instance.isStart);
    }

    public void Init()
    {
        int skillNum = GameManager.instance.skillIndexList.Count;
        for(int i = 0;i < skillItemList.Count;i++)
        {
            SkillItem skillItem = skillItemList[i];
            if (skillItem.index < skillNum)
            {
                skillItem.button.gameObject.SetActive(true);
                SkillItemSetData(skillItem, GameManager.instance.skillIndexList[i]);
            }
            else
            {
                skillItem.button.gameObject.SetActive(false);
            }
        }

        GameManager.instance.stateList.Clear();

        for (int i = 0; i < stateItemList.Count; i++)
        {
            StateItem stateItem = stateItemList[i];
            if (stateItem.index < GameManager.instance.currStateLength)
            {
                stateItem.button.gameObject.SetActive(true);
                StateItemSetData(stateItem, 0,false);
                GameManager.instance.stateList.Add(0);
            }
            else if(stateItem.index == GameManager.instance.currStateLength)
            {
                stateItem.button.gameObject.SetActive(true);
                stateItem.button.onClick.RemoveAllListeners();
                stateItem.image.sprite = loopSprite;
            }
            else
            {
                stateItem.button.gameObject.SetActive(false);
            }
            if (i == 0)
            {
                OnStateBtnClick(stateItemList[0],true);
            }
        }

        currCameraMultiNum = GameManager.instance.currCameraLevel - 1;
        CameraClickUpdate(true);
        SetStartOrClear(true);
        StartCoroutine(StartTextClose());
    }

    void SkillItemSetData(SkillItem item, int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:
                item.image.sprite = skillNoneSprite;
                break;
            case 1:
                item.image.sprite = skillGoSprite;
                break;
            case 2:
                item.image.sprite = skillLeftSprite;
                break;
            case 3:
                item.image.sprite = skillRightSprite;
                break;
            case 4:
                item.image.sprite = skillAtkSprite;
                break;
            default:
                break;
        }
        item.button.onClick.RemoveAllListeners();
        item.button.onClick.AddListener(() => OnSkillBtnClick(skillIndex));
    }

    void StateItemSetData(StateItem item, int skillIndex,bool isSelected)
    {
        switch (skillIndex)
        {
            case 0:
                item.image.sprite = skillNoneSprite;
                break;
            case 1:
                item.image.sprite = skillGoSprite;
                break;
            case 2:
                item.image.sprite = skillLeftSprite;
                break;
            case 3:
                item.image.sprite = skillRightSprite;
                break;
            case 4:
                item.image.sprite = skillAtkSprite;
                break;
            default:
                break;
        }
        StateItemSelectSet(item.selectImage, isSelected ? 1 : 0);
        item.button.onClick.RemoveAllListeners();
        item.button.onClick.AddListener(() => OnStateBtnClick(item));
    }

    //0无 1选中 2状态跳到
    void StateItemSelectSet(Image image,int state)
    {
        switch(state)
        {
            case 0:
                image.gameObject.SetActive(false);
                break;
            case 1:
                image.sprite = selectSprite;
                image.gameObject.SetActive(true);
                break;
            case 2:
                image.sprite = currStateSprite;
                image.gameObject.SetActive(true);
                break;
        }
    }

    SkillItem SkillItemInit(Transform obj, int index)
    {
        SkillItem item = new SkillItem();
        item.button = obj.GetComponent<Button>();
        item.image = obj.GetComponent<Image>();
        item.index = index;
        return item;
    }

    StateItem StateItemInit(Transform obj, int index)
    {
        StateItem item = new StateItem();
        item.button = obj.GetComponent<Button>();
        item.image = obj.GetComponent<Image>();
        item.selectImage = obj.Find("Select").GetComponent<Image>();
        item.index = index;
        return item;
    }

    void OnSkillBtnClick(int skillIndex)
    {
        if (GameManager.instance.isStart || GameManager.instance.canControl == false) return;
        GameManager.instance.stateList[currStateItem.index] = skillIndex;
        StateItemSetData(currStateItem, skillIndex, true);
        
        int nextIndex = currStateItem.index + 1;
        if (nextIndex >= GameManager.instance.stateList.Count) nextIndex = 0;
        OnStateBtnClick(stateItemList[nextIndex],true);  //跳到下一个
    }
    void OnStateBtnClick(StateItem item, bool notPress = false)
    {
        if (notPress == false && GameManager.instance.canControl == false) return;
        if (GameManager.instance.isStart) return;
        if (currStateItem != null)
        {
            StateItemSelectSet(currStateItem.selectImage, 0);
        }
        StateItemSelectSet(item.selectImage, 1);
        currStateItem = item;
    }

    public void SetRuningState(int index,bool isGetting)
    {
        StateItemSelectSet(stateItemList[index].selectImage, isGetting ? 2 : 0);
    }

    //true 开始 false过关
    public void SetStartOrClear(bool isStart)
    {
        if(isStart)
        {
            startText.gameObject.SetActive(true);
            if(GameManager.instance.audioType == 1)
            {
                startText.text = "Level " + GameManager.instance.currLevel;
            }
            else if (GameManager.instance.audioType == 2)
            {
                startText.text = "Level " + GameManager.instance.currLevel + "\nDefeat Boss";
            }
            else if (GameManager.instance.audioType == 3)
            {
                startText.text = "Now,save yourself";
            }
            clearText.gameObject.SetActive(false);
        }
        else
        {
            clearText.gameObject.SetActive(true);
            if(GameManager.instance.audioType != 3)
            {
                clearText.text = "Clear!!!";
            }
            else
            {
                clearText.text = "The robot kills you\nYou die, but robot wins";
            }
            startText.gameObject.SetActive(false);
        }
    }

    IEnumerator StartTextClose()
    {
        yield return new WaitForSeconds(2);
        startText.gameObject.SetActive(false);
        GameManager.instance.PlaySetAudio();
    }
}
