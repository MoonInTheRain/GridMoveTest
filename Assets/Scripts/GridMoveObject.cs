using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridMoveObject : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// 移動先のグリッドのターゲット
    /// </summary>
    public GameObject TargetGrid;

    /// <summary>
    /// 移動にかかるフレーム数
    /// </summary>
    public int MoveFlame = 10;

    /// <summary>
    /// 移動後
    /// </summary>
    private Vector3 _targetPosition;

    /// <summary>
    /// このオブジェクトのRaycastのターゲット
    /// </summary>
    private Graphic _thisRayCast;

    /// <summary>
    /// 移動のコルーチン
    /// </summary>
    private IEnumerator _moveCoroutine;

    /// <summary>
    /// ドラッグ中、ほかのGridMoveSystemと差別化するためのEnable
    /// </summary>
    private bool _isDrag;

    /// <summary>
    /// 移動中かどうか
    /// </summary>
    public bool IsMoving { get; set; }

    /// <summary>
    /// 並び順のINDEXを取得
    /// </summary>
    public int Index { get { return TargetGrid.transform.GetSiblingIndex(); } }

    private bool _isInitial;

    private void Start()
    {
        _thisRayCast = this.GetComponent<Graphic>();
        _isInitial = true;
    }

    public void Init()
    {
        this.transform.position = TargetGrid.transform.position;
        this.transform.localScale = Vector3.one;
        _targetPosition = this.transform.position;
    }

    private void Update()
    {
        if (_isInitial)
        {
            Init();
            _isInitial = false;
        }
        else if (_isDrag == false)
        {
            // ドラッグ中はUpdateでは移動させない。
            MoveThisObject();
        }
    }

    /// <summary>
    /// このオブジェクトを移動させる
    /// </summary>
    /// <param name="force"></param>
    private void MoveThisObject(bool force = false)
    {
        if (force || _targetPosition != TargetGrid.transform.position)
        {
            IsMoving = true;

            _targetPosition = TargetGrid.transform.position;

            CoroutineDestroy();

            _moveCoroutine = MoveCoroutine();
            StartCoroutine(_moveCoroutine);
        }
    }

    private void OnDestroy()
    {
        CoroutineDestroy();
        Destroy(TargetGrid);
    }

    /// <summary>
    /// 移動コルーチンの削除
    /// </summary>
    private void CoroutineDestroy()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    /// <summary>
    /// 位置移動のためのコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveCoroutine()
    {
        var moveVector = (TargetGrid.transform.position - this.transform.position) / MoveFlame;

        for (int i = 0; i < MoveFlame; i++)
        {
            this.transform.position += moveVector;
            yield return 0;
        }

        this.transform.position = TargetGrid.transform.position;
        IsMoving = false;
        yield return 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDrag == false) { return; }

        // 掴んだ場所をそのまま保持する場合はこっち
        // this.transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0f);
        this.transform.position = eventData.position;

        var raycastTarget = eventData.pointerCurrentRaycast.gameObject;
        if (raycastTarget != null)
        {
            var target = raycastTarget.GetComponent<GridMoveObject>();
            if (target != null && target.IsMoving == false)
            {
                TargetGrid.transform.SetSiblingIndex(target.Index);

                // MoveThisObject()内でもtrueになるが、次フレームになってしまうのでここで設定する。
                target.IsMoving = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDrag = true;

        this.transform.position = eventData.position;

        // ドラッグ中に他のオブジェクトにRayが通るようにする。
        _thisRayCast.raycastTarget = false;

        // 他のオブジェクトより手前に設置する。
        this.transform.SetSiblingIndex(this.transform.parent.GetComponentsInChildren<GridMoveObject>().Length);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDrag = false;

        // 再びRayが通るようにする
        _thisRayCast.raycastTarget = true;

        // Targetが移動していない場合、元の位置に戻す。
        MoveThisObject(true);
    }
}
