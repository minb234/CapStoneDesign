using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HydraController : MonoBehaviour
{
    public GameObject indicator;           //좌표를 가져오는 클래스
    //OrderController orderController;        //캔버스에 그려져 있는 버튼
    Animation anim;                         //명령에 대한 애니메이션 적용

    public GameObject Bullet;               //날아갈 오브젝트
    public Transform FirePos;               //발사될 위치
    private Transform characterTrans;       //캐릭터의 위치

    Transform MoveDesPose;                       //MoveBtn일 때, 좌표 갱신
    Transform AttackDesPose;                     //AttackBtn일 때, 좌표 갱신
    Vector3 desVector;                      
    int menu = 0;                           //선택지 구분을 위해 사용
    bool mState = false;
    bool aState = false;
    float Times = 0.0f;
    public Text debug3;
    // Start is called before the first frame update
    void Start()
    {
        //indicator = GetComponent<ARTaptoPlaceObject>();
        //anim = GetComponent<Animation>();
        //orderController = GetComponent<OrderController>();

    }

    // Update is called once per frame
    void Update()
    {
        
        //명령내린 메뉴
        if (menu == 1)           //이동
        {
            debug3.text = MoveDesPose.ToString();
            this.transform.LookAt(MoveDesPose.position);
            //this.transform.Translate(transform.forward * 5);

           // this.transform.Translate(desVector.forward, Times.deltaTime);

            this.transform.position = Vector3.MoveTowards(this.transform.position, MoveDesPose.position, 0.005f);

            if (this.transform.position == MoveDesPose.position)
                SetMenu(0);

        } else if(menu == 2)    //단일공격 
        {
            Fire1();
            SetMenu(0);
        } else if(menu == 3)    //여러개공격
        {
            //Fire2();
            //anim.Play("attacked");          //지금 애니메이션들어가나 확인 
            SetMenu(0);
        }

        ////Apply Animation Parts
        //if (aState)
        //{
        //    anim.Play("attack");
        //    Times += Time.deltaTime;

        //    if (Times > 1.0f)
        //    {
        //        aState = false;
        //        Times = 0.0f;
        //    }

        //}

        //if (mState)
        //{
        //    anim.Play("walk");
        //    if (this.transform.position == MoveDesPose.position)
        //        mState = false;
        //}

        //if (!mState && !aState)
        //{
        //    anim.Play("idle");
        //}

    }//여기까지 업데이트

    public void SetMenu(int menu)
    {
        this.menu = menu;
    }

    public void BtnMove()
    {

        MoveDesPose = indicator.transform;
        mState = true;
        aState = false;
        SetMenu(1);
    }
    public void BtnAttack1()
    {
        SetMenu(2);
        AttackDesPose = indicator.transform;
        mState = false;
        aState = true;

    }
    public void BtnAttack2()
    {
        SetMenu(3);
        AttackDesPose = indicator.transform;
        mState = false;
        aState = true;
    }

    private void Fire1()
    {
        transform.LookAt(AttackDesPose.position);
        //Instantiate(Bullet, FirePos.transform.position, FirePos.transform.rotation);  //forward -> position
        SetMenu(0);
    }

    private void Fire2()
    {
        transform.LookAt(AttackDesPose.position);
    }
}
