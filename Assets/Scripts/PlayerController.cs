using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionType
{
    up,
    left,
    down,
    right
}

public class PlayerController : MonoBehaviour
{
    public float stepTime = 0.5f;
    public List<Sprite> playerSpriteList;


    [HideInInspector]public static PlayerController instance;
    [HideInInspector]public int currIndex;

    private float leftTime;
    private DirectionType direction;
    private SpriteRenderer playerSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        instance = this;
        GameManager.instance.isStart = false;
        currIndex = -1;
        leftTime = stepTime;
        direction = GameManager.instance.currStartDirection;
        RefreshSprite();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isStart || !GameManager.instance.canControl) return;
        leftTime -= Time.deltaTime;
        if(leftTime <= 0)
        {
            leftTime += stepTime;
            playerAction();
        }
    }

    void playerAction()
    {
        if(currIndex >= 0)
        {
            UIManager.instance.SetRuningState(currIndex, false);
        }
        currIndex = currIndex + 1;
        if (currIndex == GameManager.instance.stateList.Count)
        {
            UIManager.instance.SetRuningState(currIndex, true);
            return;  //循环到末尾的步骤
        }
        if (currIndex > GameManager.instance.stateList.Count) currIndex = 0;
        UIManager.instance.SetRuningState(currIndex, true);
        int state = GameManager.instance.stateList[currIndex];
        switch (state)
        {
            case 0:     //空
                break;
            case 1:     //前进
                DetectAndAction(0);
                break;
            case 2:     //左90
                DirectionRotate(1);
                break;
            case 3:     //右90
                DirectionRotate(2);
                break;
            case 4:     //攻击
                DetectAndAction(1);
                break;
            default:
                break;
        }
    }

    //ret:0畅通 1被阻挡 type:0走路 1攻击
    int DetectAndAction(int type)
    {
        Vector2 targetPoint = getDirVector(direction);
        Vector2 myPoint = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(myPoint, myPoint + targetPoint, LayerMask.GetMask("Interactable", "Wall"));
        if (hit)
        {
            int layer = hit.transform.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Wall"))
            {
                GameManager.instance.PlayEffect(5);
                return 1;
            }
            else if (layer == LayerMask.NameToLayer("Interactable"))
            {
                Interactable interactable = hit.transform.GetComponent<Interactable>();
                interactable.Interact(this, type);
                return 2;
            }
        }
        if(type == 0)
        {
            GoAhead();
        }
        else if(type == 1)
        {
            GameManager.instance.PlayEffect(3);
        }
        return 0;
    }

    private Vector2 getDirVector(DirectionType type)
    {
        Vector2 targetPoint = new Vector2(0, 0);
        switch (type)
        {
            case DirectionType.up:
                targetPoint = new Vector2(0, 1);
                break;
            case DirectionType.down:
                targetPoint = new Vector2(0, -1);
                break;
            case DirectionType.left:
                targetPoint = new Vector2(-1, 0);
                break;
            case DirectionType.right:
                targetPoint = new Vector2(1, 0);
                break;
        }
        return targetPoint;
    }

    //1逆时针 2顺时针
    private void DirectionRotate(int type)
    {
        switch (direction)
        {
            case DirectionType.up:
                switch(type)
                {
                    case 1:
                        direction = DirectionType.left;
                        break;
                    case 2:
                        direction = DirectionType.right;
                        break;
                }
                break;
            case DirectionType.down:
                switch (type)
                {
                    case 1:
                        direction = DirectionType.right;
                        break;
                    case 2:
                        direction = DirectionType.left;
                        break;
                }
                break;
            case DirectionType.left:
                switch (type)
                {
                    case 1:
                        direction = DirectionType.down;
                        break;
                    case 2:
                        direction = DirectionType.up;
                        break;
                }
                break;
            case DirectionType.right:
                switch (type)
                {
                    case 1:
                        direction = DirectionType.up;
                        break;
                    case 2:
                        direction = DirectionType.down;
                        break;
                }
                break;
        }
        RefreshSprite();
    }

    void RefreshSprite()
    {
        //刷新立绘
        playerSpriteRenderer.sprite = playerSpriteList[(int)direction];
    }

    public void GoAhead()
    {
        Vector2 targetPoint = getDirVector(direction);
        Vector2 myPoint = transform.position;
        transform.position = myPoint + targetPoint;
        GameManager.instance.PlayEffect(4);
    }

}
