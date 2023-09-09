using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonobehaviour<Player>
{
    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;
    
    private bool playerToolUseDisabled = false;
    private AnimationOverrides animationOverrides;
    private GridCursor gridCursor;

    //movement parameters
    private float inputX;
    private float inputY;
    private bool isWalking;
    private bool isRunning;
    private bool isIdle;
    private bool isCarrying = false;
    private ToolEffect toolEffect = ToolEffect.none;

    private bool isUsingToolRight;
    private bool isUsingToolLeft;
    private bool isUsingToolUp;
    private bool isUsingToolDown;

    private bool isLiftingToolRight;
    private bool isLiftingToolLeft;
    private bool isLiftingToolUp;
    private bool isLiftingToolDown;

    private bool isPickingRight;
    private bool isPickingLeft;
    private bool isPickingUp;
    private bool isPickingDown;

    private bool isSwingingToolRight;
    private bool isSwingingToolLeft;
    private bool isSwingingToolUp;
    private bool isSwingingToolDown;

    private Camera mainCamera;

    private Rigidbody2D rigidbody2;

    private Direction playerDirection;
    private List<CharacterAttribute> characterAttributesCustomisationList;
    private float movementSpeed;

    [Tooltip("prefab��Ҫ��sprite renderer�������")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;
    
    //�ɸı��Player Attributes
    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;

    private bool _playerInputIsDisabled = false;

    //_playerInputIsDisabled�����ԣ����������ͨ�����Ի���ֶε���Ϣ
    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }

    protected override void Awake()
    {
        base.Awake();
        rigidbody2 = GetComponent<Rigidbody2D>();

        animationOverrides = GetComponentInChildren<AnimationOverrides>();

        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColour.none, PartVariantType.none);
        characterAttributesCustomisationList = new List<CharacterAttribute>();

        mainCamera = Camera.main;
    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
    }

    private void Update()
    {
        #region Player Input

        if(!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers();
            PlayerMovementInput();
            PlayerClickInput();
            PlayerTestInput();

            EventHandler.CallMovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying,
                toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);
        }

        #endregion Player Input

    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector2 move = new Vector2(inputX * movementSpeed * Time.fixedDeltaTime, inputY * movementSpeed * Time.fixedDeltaTime);
        rigidbody2.MovePosition(rigidbody2.position + move);
    }

    private void ResetAnimationTriggers()
    {
        toolEffect = ToolEffect.none;
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;

        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;

        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;

        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
    }

    private void PlayerMovementInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if(inputX != 0 && inputY != 0)
        {
            inputX = inputX * 0.71f;
            inputY = inputY * 0.71f;
        }

        if(inputX != 0 || inputY != 0 )
        {
            //�ж������߻��ǰ�shift����
            WalkOrRun();
            if(inputX < 0)
            {
                playerDirection = Direction.left;
            }
            else if(inputX > 0)
            {
                playerDirection = Direction.right;
            }
            else if(inputY < 0)
            {
                playerDirection = Direction.down;
            }
            else
            {
                playerDirection = Direction.up;
            }
        }
        else if(inputX ==0 && inputY ==0)
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }
    }

    public void DisablePlayerInputAndRestMovement()
    {
        DisablePlayerInput();
        RestMovement();

        EventHandler.CallMovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying,
                toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);
    }

    private void WalkOrRun()
    {
        if (Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;
        }
        else
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = Settings.walkingSpeed;
        }
        
    }

    public Vector3 GetPlayerViewportPosition()
    {
        //(0,0)�ӽ����£�(1,1)�ӽ�����
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    public void EnablePlayerInput()
    {
        PlayerInputIsDisabled = false;
    }

    public void DisablePlayerInput()
    {
        PlayerInputIsDisabled = true;
    }

    private void RestMovement()
    {
        inputX = 0f;
        inputY = 0f;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }

    public void ShowCarriedItem(int Code)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(Code);
        if(itemDetails != null)
        {
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1, 1, 1, 1);

            armsCharacterAttribute.partVariantType = PartVariantType.carry;
            characterAttributesCustomisationList.Clear();
            characterAttributesCustomisationList.Add(armsCharacterAttribute);
            animationOverrides.ApplyCharacterCustomisationParameters(characterAttributesCustomisationList);

            isCarrying = true;
        }
    }
    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1, 1, 1, 0);

        armsCharacterAttribute.partVariantType = PartVariantType.none;
        characterAttributesCustomisationList.Clear();
        characterAttributesCustomisationList.Add(armsCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributesCustomisationList);

        isCarrying = false;
    }

    private void PlayerTestInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //���Է���
            TimeManager.Instance.TestAdvanceGameMinute();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            //��������
            TimeManager.Instance.TestAdvanceGameDay();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            //���Գ�������
            SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        }
    }

    private void PlayerClickInput()
    {
        if(!playerToolUseDisabled)
        {
            if (Input.GetMouseButton(0))
            {
                if (gridCursor.CursorIsEnable)
                {
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
                    Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();
                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            }
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(itemDetails);
                    }
                    break;
                case ItemType.Commodity:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;
                case ItemType.Watering_tool:
                case ItemType.Hoeing_tool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;
                case ItemType.none:
                    break;
                case ItemType.count:
                    break;
                default:
                    break;
            }
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition,Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if(cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if(cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private void ResetMovement()
    {
        inputX = 0;
        inputY = 0;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if(itemDetails.canBeCarried && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }


    private void ProcessPlayerClickInputSeed(ItemDetails itemDetails)
    {
        if (itemDetails.canBeCarried && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        switch(itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.Watering_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            default:
                break;
        }
    }

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(WaterGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(HoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set hoe animation
        toolCharacterAttribute.partVariantType = PartVariantType.hoe;
        characterAttributesCustomisationList.Clear();
        characterAttributesCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributesCustomisationList);
        // set player direction
        if (playerDirection == Vector3Int.right) { isUsingToolRight = true; }
        else if (playerDirection == Vector3Int.left) { isUsingToolLeft = true; }
        else if (playerDirection == Vector3Int.up) { isUsingToolUp = true; }
        else { isUsingToolDown = true; }

        yield return useToolAnimationPause;

        //gird is dug ground
        if(gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX,
            gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        playerToolUseDisabled = false;
        PlayerInputIsDisabled = false;

    }

    private IEnumerator WaterGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set water animation
        toolCharacterAttribute.partVariantType = PartVariantType.wateringCan;
        characterAttributesCustomisationList.Clear();
        characterAttributesCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributesCustomisationList);
        //watercan have water effect
        toolEffect = ToolEffect.watering;
        // set player direction
        if (playerDirection == Vector3Int.right) { isLiftingToolRight = true; }
        else if (playerDirection == Vector3Int.left) { isLiftingToolLeft = true; }
        else if (playerDirection == Vector3Int.up) { isLiftingToolUp = true; }
        else { isLiftingToolDown = true; }

        yield return liftToolAnimationPause;

        //gird is dug&&water ground
        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX,
            gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertiesManager.Instance.DisplayWaterGround(gridPropertyDetails);

        yield return afterLiftToolAnimationPause;

        playerToolUseDisabled = false;
        PlayerInputIsDisabled = false;
    }
}
