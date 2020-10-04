using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public int type;   //0终点
    public GameObject relateObj;
    public List<Sprite> spriteList;

    [HideInInspector] int commonValue;

    private SpriteRenderer image;
    private new Collider2D collider;
    private new Animation animation;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        animation = GetComponent<Animation>();
        image = GetComponent<SpriteRenderer>();
        commonValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //type:0走路 1攻击 2其他关联
    public void Interact(PlayerController player,int actionType)
    {
        if(actionType == 0)
        {
            switch (type)
            {
                case 0:
                    //过关
                    player.GoAhead();
                    GameManager.instance.PlayEffect(1);
                    GameManager.instance.MissionClear();
                    break;
                case 1:
                    //遇到狗过不去
                    GameManager.instance.PlayEffect(6);
                    break;
                case 2:
                    //按钮按下
                    player.GoAhead();
                    if(commonValue == 0)
                    {
                        commonValue = 1;
                        Interactable door = relateObj.GetComponent<Interactable>();
                        image.sprite = spriteList[1];
                        collider.enabled = false;
                        if (door != null)
                        {
                            door.Interact(player, 2);
                        }
                    }
                    //GameManager.instance.PlayEffect(6);
                    break;
                case 3:
                    //门没开过不去
                    GameManager.instance.PlayEffect(5);
                    break;
                default:
                    break;
            }
        }
        else if (actionType == 1)
        {
            switch (type)
            {
                case 0:
                    break;
                case 1:
                    //打狗
                    GameManager.instance.PlayEffect(3);
                    Dead();
                    break;
                case 4:
                    //打boss
                    //GameManager.instance.PlayEffect(3);
                    //Dead();
                    GameManager.instance.PlayEffect(3);
                    if (commonValue < 5)
                    {
                        commonValue += 1;
                        hurt();
                    }
                    else
                    {
                        Dead();
                        GameManager.instance.StopAudio();
                    }
                    break;
                case 5:
                    //杀玩家
                    GameManager.instance.PlayEffect(3);
                    Dead();
                    GameManager.instance.MissionClear();
                    break;
                default:
                    break;
            }
        }
        else if (actionType == 2)
        {
            switch (type)
            {
                case 3:
                    //门收到按钮按下
                    if (commonValue == 0)
                    {
                        commonValue = 1;
                        image.sprite = spriteList[1];
                        collider.enabled = false;
                        GameManager.instance.PlayEffect(7);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    void hurt()
    {
        animation.clip = GameManager.instance.EnemyHurt;
        animation.Play();
    }

    void Dead()
    {
        collider.enabled = false;
        animation.clip = GameManager.instance.EnemyDisappear;
        animation.Play();
    }
}
