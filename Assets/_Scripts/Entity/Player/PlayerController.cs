using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using InputSystem;
using System.Collections.Generic;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    #region �������
    [ShowInInspector] public IFunction_Move Movement { get; private set; }
    [ShowInInspector] public AttackTrigger Attack { get; private set; }
    [ShowInInspector] public FaceMouse Facing { get; private set; }
    [ShowInInspector] public TrunBody BodyTurning { get; private set; }
    [ShowInInspector] public Runner Running { get; private set; }
    [ShowInInspector] public Inventory_HotBar Hotbar { get; private set; }
    //�������
    [ShowInInspector] public CameraFollowManager VirtualCameraManager { get; private set; }

    [ShowInInspector] public ItemDroper ItemDroper { get; private set; }

    public PlayerUIManager _playerUI;
    [SerializeField] private List<Canvas> _bagCanvases;
    [SerializeField] private List<Canvas> _settingCanvases;
    #endregion

    #region ����ϵͳ
    private PlayerInputActions _inputActions;
    private Camera _mainCamera;

    public bool CtrlIsDown;
    #endregion

    #region Unity��������
    private void Awake()
    {
        VirtualCameraManager = GetComponentInChildren<CameraFollowManager>();
        ItemDroper = GetComponentInChildren<ItemDroper>();
        SetupInputSystem();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        InitializeComponents();

         SwitchBag();
         SwitchEquip();
         SwitchCraft();
         SwitchSetting();

    }

    private void Update()
    {
        HandleCombatInput();
        HandleBodyTurning();
        HandleMovementInput();
    }
    #endregion

    #region ���봦��
    private void HandleCombatInput()
    {
        var mouseState = GetMouseKeyState();
        if (mouseState != KeyState.Void && Attack != null)
        {
            Attack.TriggerAttack(mouseState, GetMouseWorldPosition());
        }
    }

    private void HandleMovementInput()
    {
        if (Movement == null) return;

        Vector2 moveInput = _inputActions.Win10.Move_Player.ReadValue<Vector2>();
        Movement.Move(moveInput);
    }

    private void HandleBodyTurning()
    {
        if (BodyTurning == null) return;

        float horizontal = Input.GetAxis("Horizontal");
        if (!Mathf.Approximately(horizontal, 0f))
        {
            BodyTurning.TurnBodyToDirection(horizontal > 0 ? Vector2.right : Vector2.left);
        }
    }
    #endregion

    #region ��������
    private KeyState GetMouseKeyState()
    {
        if (Input.GetMouseButtonDown(0)) return KeyState.Start;
        if (Input.GetMouseButton(0)) return KeyState.Hold;
        if (Input.GetMouseButtonUp(0)) return KeyState.End;
        return KeyState.Void;
    }

    private Vector3 GetMouseWorldPosition()
    {
        return _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
    #endregion


    #region ��ʼ������
    private void InitializeComponents()
    {
        
        Attack = GetComponentInChildren<AttackTrigger>();
        Facing = GetComponentInChildren<FaceMouse>();
        BodyTurning = GetComponentInChildren<TrunBody>();
        Running = GetComponentInChildren<Runner>();
        Movement = GetComponentInChildren<IFunction_Move>();

        Hotbar = _playerUI.QuickAccessBar.GetComponent<Inventory_HotBar>();



    }

    private void SetupInputSystem()
    {
        //��ʼ������ϵͳ
        _inputActions = new PlayerInputActions();
        _inputActions.Win10.Enable();
        var win10 = _inputActions.Win10;
        //���������
        win10.SwitchHotBar_Player.performed += SwitchHotbar;
        //������Ʒ����
        win10.F.performed += PlayerDropItem;
        //��������
        win10.B.performed += SwitchBag;
        win10.B.performed += SwitchEquip;
        win10.B.performed += SwitchCraft;
        //�����������
        win10.ESC.performed += SwitchSetting;
        //�����ܲ�
        win10.Shift.started += Run;
        win10.Shift.canceled += StopRun;
        //�����ӽ��л�
        win10.CtrlMouse.performed  += PovValueChanged;
        //�������
        win10.Mouse.performed  += PlayerTakeItem_FaceMouse;   
        //����������
        win10.MouseScroll.performed  += SwitchHotbarByScroll;
        //����Ctrl��
        win10.Ctrl.started += (InputAction.CallbackContext context ) => { CtrlIsDown = true; };
        win10.Ctrl.canceled += (InputAction.CallbackContext context) => { CtrlIsDown = false; };





    }
    #endregion

    #region ��Ʒ����
    public void PlayerDropItem(InputAction.CallbackContext context = default)
    {
        ItemDroper.DropItem();
    }
    public void PovValueChanged(InputAction.CallbackContext context = default)
    {
        //��ȡ��������ֵ
        Vector2 scrollValue = (Vector2)context.ReadValueAsObject();
        Debug.Log(scrollValue.y);
        if (scrollValue.y > 0)
        {
            //TODO��Ұ����
            VirtualCameraManager.ChangeCameraView(-1);
        }
        else if (scrollValue.y < 0)
        {
            //TODO��Ұ����
            VirtualCameraManager.ChangeCameraView(1);
        }
    }

    public void PlayerTakeItem_FaceMouse(InputAction.CallbackContext context = default)
    {
        context.ReadValue<Vector2>();
        //��ȡ���
        Facing.FaceToMouse( GetMouseWorldPosition());
    }
    private void SwitchHotbar(InputAction.CallbackContext context)
    {
        if (context.control.device is Keyboard keyboard)
        {
            if (int.TryParse(context.control.displayName, out int keyNumber))
            {
                int targetIndex = keyNumber - 1;
                if (targetIndex != Hotbar.CurrentIndex)
                {
                    Hotbar.ChangeIndex(targetIndex);
                    return;
                }
            }
        }
    }
    private void SwitchHotbarByScroll(InputAction.CallbackContext context)
    {
        if (CtrlIsDown)
        {
            return;
        }
        Vector2 scrollValue = context.ReadValue<Vector2>();
        //Debug.Log(scrollValue);
        if (scrollValue.y > 0)
        {
            //Debug.Log(Hotbar.CurrentIndex);
            Hotbar.ChangeIndex(Hotbar.CurrentIndex - 1);
        }
        else if (scrollValue.y < 0)
        {
            Hotbar.ChangeIndex(Hotbar.CurrentIndex + 1);
        }
    }



    private void DropItem(InputAction.CallbackContext context = default)
    {
        // ʵ����Ʒ�����߼�
        Vector3 dropDirection = (GetMouseWorldPosition() - transform.position).normalized;
        // Hotbar.TryDropCurrentItem(dropDirection);
    }
    //���ر���
    public void SwitchBag(InputAction.CallbackContext context = default)
    {
        _playerUI.Bag.enabled = ! _playerUI.Bag.enabled;
    }

    //�����������
    public void SwitchSetting(InputAction.CallbackContext context = default)
    {
        _playerUI.Setting.enabled = ! _playerUI.Setting.enabled;
    }

    //����װ����
    public void SwitchEquip(InputAction.CallbackContext context = default)
    {
        _playerUI.Equip.enabled = ! _playerUI.Equip.enabled;
    }

    //����������
    public void SwitchCraft(InputAction.CallbackContext context = default)
    {
        _playerUI.Craft.enabled = ! _playerUI.Craft.enabled;
    }

    public void Run(InputAction.CallbackContext context = default)
    {
        if (Running != null)
        {
            Running.SwitchToRun(true);
            Debug.Log("Run");
        }
    }

    public void StopRun(InputAction.CallbackContext context = default)
    {
        if (Running != null)
        {
            Running.SwitchToRun(false);
            Debug.Log("Stop");
        }
    }
    #endregion
}

public enum KeyState { Start, Hold, End, Void }