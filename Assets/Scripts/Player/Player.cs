using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : SingletonMonobehaviour<Player>,ISaveable
{
    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds pickAnimationPause;
    private WaitForSeconds afterPickAnimationPause;

    private bool playerToolUseDisabled = false;
    private AnimationOverrides animationOverrides;
    private GridCursor gridCursor;
    private Cursor cursor;

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
    //���˸�������壬��Ҫֱ�ӿ���transform���ƶ����ø����MovePosition����
    private Rigidbody2D rigidbody2;

    private Direction playerDirection;
    private List<CharacterAttribute> characterAttributesCustomisationList;
    private float movementSpeed;

    [Tooltip("prefab��Ҫ��sprite renderer�������")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;
    
    //�ɸı��Player Attributes
    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;

    public GameObject canyonOakTreePrefab;

    //_playerInputIsDisabled�����ԣ����������ͨ�����Ի���ֶε���Ϣ
    private bool _playerInputIsDisabled = false;
    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        rigidbody2 = GetComponent<Rigidbody2D>();

        animationOverrides = GetComponentInChildren<AnimationOverrides>();
        
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColour.none, PartVariantType.none);
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColour.none, PartVariantType.none);
        
        characterAttributesCustomisationList = new List<CharacterAttribute>();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();
        
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPause);
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

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndRestMovement;
        EventHandler.AfterSceneLoadEvent += EnablePlayerInput;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndRestMovement;
        EventHandler.AfterSceneLoadEvent -= EnablePlayerInput;
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
        //�����palyer�������������ӿ����꣬�ӿ���������Ļ����ĵ�λ��
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

    private void PlayerClickInput()
    {
        if(!playerToolUseDisabled)
        {
            if (Input.GetMouseButton(0))
            {
                if (gridCursor.CursorIsEnable || cursor.CursorIsEnable)
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
                        ProcessPlayerClickInputSeed(gridPropertyDetails, itemDetails);
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
                case ItemType.Reaping_tool:
                case ItemType.Breaking_tool:
                case ItemType.Collecting_tool:
                case ItemType.Chopping_tool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;
                case ItemType.none:
                case ItemType.count:
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


    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (itemDetails.canBeCarried && gridCursor.CursorPositionIsValid &&
           gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }
        else if (itemDetails.canBeCarried && gridCursor.CursorPositionIsValid)
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
            case ItemType.Reaping_tool:
                if (cursor.CursorPositionIsValid)
                {
                    playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCenterPosition());
                    ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                }
                break;
            case ItemType.Collecting_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.Chopping_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.Breaking_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    BreakInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            default:
                break;
        }
    }

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerCenterPosition)
    {
        if (cursorPosition.x > playerCenterPosition.x &&
            cursorPosition.y < (playerCenterPosition.y + cursor.ItemUseRadius / 2) &&
            cursorPosition.y > (playerCenterPosition.y - cursor.ItemUseRadius / 2))
        {
            return Vector3Int.right;
        }
        else if(cursorPosition.x < playerCenterPosition.x &&
            cursorPosition.y < (playerCenterPosition.y + cursor.ItemUseRadius / 2) &&
            cursorPosition.y > (playerCenterPosition.y - cursor.ItemUseRadius / 2))
        {
            return Vector3Int.left;
        }
        else if(cursorPosition.y> playerCenterPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(HoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(WaterGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }

    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(CollectInPlayerDirectionRoutine(playerDirection, equippedItemDetails, gridPropertyDetails));
    }

    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ChopInPlayerDirectionRoutine(playerDirection, gridPropertyDetails, itemDetails));
    }

    private void BreakInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(BreakInPlayerDirectionRoutine(playerDirection, gridPropertyDetails, itemDetails));
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
    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set Reap animation
        toolCharacterAttribute.partVariantType = PartVariantType.scythe;
        characterAttributesCustomisationList.Clear();
        characterAttributesCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributesCustomisationList);
        
        // reap in player direction
        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAnimationPause;

        playerToolUseDisabled = false;
        PlayerInputIsDisabled = false;
    }

    private void UseToolInPlayerDirection(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Reaping_tool:
                    if (playerDirection == Vector3Int.right)
                    {
                        isSwingingToolRight = true;
                    }
                    else if (playerDirection == Vector3Int.left)
                    {
                        isSwingingToolLeft = true;
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        isSwingingToolUp = true;
                    }
                    else if (playerDirection == Vector3Int.down)
                    {
                        isSwingingToolDown = true;
                    }
                    break;
                default:
                    break;
            }
            //reap's center point
            Vector2 point = new Vector2(GetPlayerCenterPosition().x + (playerDirection.x * (itemDetails.itemUseRadius / 2)),
                GetPlayerCenterPosition().y + playerDirection.y * (itemDetails.itemUseRadius / 2));
            //reap's size
            Vector2 size = new Vector2(itemDetails.itemUseRadius, itemDetails.itemUseRadius);
            //get item at box location
            Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerReapSwing, point, size, 0);
            int reapableItemCount = 0;

            for (int i = itemArray.Length - 1; i >= 0; i--)
            {
                if (itemArray[i] != null)
                {
                    if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.Reapable_scenery)
                    {
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x,
                            itemArray[i].transform.position.y + Settings.gridCellSize / 2, itemArray[i].transform.position.z);
                        EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);

                        Destroy(itemArray[i].gameObject);
                        reapableItemCount++;

                        //�ݻ������ﵽ�ո�����ʱ������ѭ��
                        if (reapableItemCount >= Settings.maxTargetComponentsToDestroyPerReapSwing)
                        {
                            break;
                        }
                    }
                }
            }

        }
    }

    private IEnumerator CollectInPlayerDirectionRoutine(Vector3Int playerDirection, ItemDetails equippedItemDetails, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return pickAnimationPause;

        yield return afterPickAnimationPause;

        playerToolUseDisabled = false;
        PlayerInputIsDisabled = false;
    }

    private IEnumerator ChopInPlayerDirectionRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set Reap animation
        toolCharacterAttribute.partVariantType = PartVariantType.axe;
        characterAttributesCustomisationList.Clear();
        characterAttributesCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributesCustomisationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, itemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private IEnumerator BreakInPlayerDirectionRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set Break animation
        toolCharacterAttribute.partVariantType = PartVariantType.pickaxe;
        characterAttributesCustomisationList.Clear();
        characterAttributesCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributesCustomisationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, itemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void ProcessCropWithEquippedItemInPlayerDirection(Vector3Int playerDirection, ItemDetails equippedItemDetails, GridPropertyDetails gridPropertyDetails)
    {
        switch (equippedItemDetails.itemType)
        {
            case ItemType.Collecting_tool:
                if (playerDirection == Vector3Int.right)
                {
                    isPickingRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isPickingLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isPickingUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isPickingDown = true;
                }
                break;
            case ItemType.Breaking_tool:
            case ItemType.Chopping_tool:
                if(playerDirection == Vector3Int.right)
                {
                    isUsingToolRight = true;
                }
                else if(playerDirection == Vector3Int.left)
                {
                    isUsingToolLeft = true;
                }
                else if(playerDirection == Vector3Int.up)
                {
                    isUsingToolUp = true;
                }
                else if(playerDirection == Vector3Int.down)
                {
                    isUsingToolDown = true;
                }
                break;    
            case ItemType.none:
            default:
                break;
        }

        Crop crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);
        if (crop != null)
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.Collecting_tool:
                    crop.ProcessToolAction(equippedItemDetails, isPickingRight, isPickingLeft, isPickingUp, isPickingDown);
                    break;
                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                    crop.ProcessToolAction(equippedItemDetails, isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown);
                    break;
                case ItemType.none:
                default:
                    break;
            }
        }
    }

    public Vector3 GetPlayerCenterPosition()
    {
        //position��Ӧ����ԭ��pivot
        return new Vector3(transform.position.x,
            transform.position.y + Settings.playerCenterOffset, transform.position.z);
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode) != null)
        {
            gridPropertyDetails.seedItemCode = itemDetails.itemCode;
            gridPropertyDetails.growthDays = 0;
            GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
            EventHandler.CallRemoveSelectedItemFromInventoryEvent();
        }
        else
        {
            Debug.Log("Crops���ݿ�û��itemCode��Ӧ������");
        }
    }

    private void PlayerTestInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //��T���Է���
            TimeManager.Instance.TestAdvanceGameMinute();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            //��G��������
            TimeManager.Instance.TestAdvanceGameDay();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            //��Y���Գ�������
            SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        }
        if (Input.GetMouseButtonDown(1))
        {
            //������Ҽ�����pool��objectʹ��
            GameObject tree = PoolManager.Instance.ReuseObject(canyonOakTreePrefab, mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -mainCamera.transform.position.z)), Quaternion.identity);
            tree.SetActive(tree);
        }
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //����ڳ��������£�����Ҫ�洢scene
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //����ڳ��������£�����Ҫ�洢scene
    }

    public GameObjectSave ISaveableSave()
    {
        //�洢֮ǰɾ����Ӧ�ɵ����ݣ��������ݳ�����
        GameObjectSave.sceneData.Remove(Settings.persistentScene);
        
        SceneSave sceneSave = new SceneSave();
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave.stringDictionary = new Dictionary<string, string>();
        Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);
        
        sceneSave.vector3Dictionary.Add("playerPosition", vector3Serializable);
        sceneSave.stringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);
        sceneSave.stringDictionary.Add("playerDirection", playerDirection.ToString());
        GameObjectSave.sceneData.Add(Settings.persistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.GameData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave)) 
        {
            GameObjectSave = gameObjectSave;
            if (GameObjectSave.sceneData.TryGetValue(Settings.persistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("playerPosition",
                    out Vector3Serializable playerPosition))
                {
                    transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                }
                if(sceneSave.stringDictionary != null)
                {
                    if (sceneSave.stringDictionary.TryGetValue("currentScene", out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }
                    if(sceneSave.stringDictionary.TryGetValue("playerDirection", out string playerDir))
                    {
                        bool tmp = Enum.TryParse<Direction>(playerDir, true, out Direction direction);
                        if (tmp)
                        {
                            playerDirection = direction;
                            SetPlayerDirection(playerDirection);
                        }
                    }
                }

            }
        }
    }

    private void SetPlayerDirection(Direction playerDirection)
    {
        switch (playerDirection)
        {
            case Direction.up:
                //��Ϊ���Ϸ���״̬
                EventHandler.CallMovementEvent(0, 0, false, false, false, false, ToolEffect.none, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false);
                break;
            case Direction.down:
                EventHandler.CallMovementEvent(0, 0, false, false, false, false, ToolEffect.none, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true);
                break;
            case Direction.left:
                EventHandler.CallMovementEvent(0, 0, false, false, false, false, ToolEffect.none, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false);
                break;
            case Direction.right:
                EventHandler.CallMovementEvent(0, 0, false, false, false, false, ToolEffect.none, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false);
                break;
            default:
                //Ĭ�����·���״̬
                EventHandler.CallMovementEvent(0, 0, false, false, false, false, ToolEffect.none, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true);
                break;
        }
    }
}
