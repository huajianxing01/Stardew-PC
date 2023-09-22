using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCPath))]
public class NPCMovement : MonoBehaviour
{
    public SceneName npcCurrentScene;
    [HideInInspector] public SceneName npcTargetScene;
    [HideInInspector] public Vector3Int npcCurrentGridPosition;
    [HideInInspector] public Vector3Int npcTargetGridPosition;
    [HideInInspector] public Vector3 npcTargetWorldPosition;
    public Direction npcFacingDirectionAtDestination;
    private SceneName npcPreviousMovementStepScene;
    private Vector3Int npcNextGridPosition;
    private Vector3 npcNextWorldPosition;
    
    [Header("NPC Movement")] public float npcNormalSpeed = 2f;
    [SerializeField] private float npcMinSpeed = 1f;
    [SerializeField] private float npcMaxSpeed = 3f;
    private bool npcIsMoving = false;

    [Header("NPC Animation")][SerializeField] private AnimationClip blankAnimation = null;
    [HideInInspector] public AnimationClip npcTargetAnimationClip;
    private Grid grid;
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;
    private WaitForFixedUpdate waitForFixedUpdate;
    private Animator animator;
    private AnimatorOverrideController overrideController;
    private int lastMoveAnimationParameter;
    private NPCPath npcPath;
    private bool npcInitialised = false;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public bool npcActiveInScene = false;

    private bool sceneLoaded = false;
    private Coroutine moveToGridPositionRoutine;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloaded;
    }

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        npcPath = GetComponent<NPCPath>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;

        npcTargetScene = npcCurrentScene;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = transform.position;
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
        SetIdleAnimation();
    }

 
    private void FixedUpdate()
    {
        if(sceneLoaded)
        {
            if(!npcIsMoving)
            {
                npcCurrentGridPosition = GetGridPosition(transform.position);
                npcNextGridPosition = npcCurrentGridPosition;

                if (npcPath.npcMovementStepStack.Count > 0)
                {
                    //peek是获取栈顶元素但不删除，pop是获取并删除
                    NPCMovementStep npcMovementStep = npcPath.npcMovementStepStack.Peek();
                    npcCurrentScene = npcMovementStep.SceneName;
                    //如果npc当前场景和下一步场景不是同一个
                    if(npcCurrentScene != npcPreviousMovementStepScene)
                    {
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);
                        npcPreviousMovementStepScene = npcCurrentScene;
                        npcPath.UpdateTimeOnPath();
                    }
                    
                    //如果npc在当前场景有移动，设置动作为可见的
                    if(npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                    {
                        SetNPCActiveInScene();
                        
                        npcMovementStep = npcPath.npcMovementStepStack.Pop();
                        npcNextGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);
                        MoveToGridPosition(npcNextGridPosition, npcMovementStepTime, TimeManager.Instance.GetGameTime());
                    }
                    else
                    {
                        SetNPCInactiveInScene();

                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);
                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);
                        TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                        if(npcMovementStepTime < gameTime)
                        {
                            npcMovementStep = npcPath.npcMovementStepStack.Pop();
                            npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                            npcNextGridPosition = npcCurrentGridPosition;
                            transform.position = GetWorldPosition(npcCurrentGridPosition);
                        }
                    }
                }
                else
                {
                    ResetMoveAnimation();
                    SetNPCFacingDirection();
                    SetNPCEventAnimation();
                }
            }
        }
    }

    private void MoveToGridPosition(Vector3Int npcNextGridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        moveToGridPositionRoutine = StartCoroutine(MoveToGridPositionRoutine(npcNextGridPosition, npcMovementStepTime, gameTime));
    }

    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        npcIsMoving = true;
        SetMoveAnimation(gridPosition);
        npcNextWorldPosition = GetWorldPosition(gridPosition);
        //如果某种原因游戏时间大于npc移动时间，直接跳过移动，把npc瞬移到目标位置
        if (npcMovementStepTime > gameTime)
        {
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);
            float npcMoveSpeed = Mathf.Max(npcMinSpeed, Vector3.Distance(transform.position, npcNextWorldPosition) / timeToMove / Settings.secondsPerGameSecond);

            if(npcMoveSpeed <= npcMaxSpeed)
            {
                while (Vector3.Distance(transform.position, npcNextWorldPosition) > Settings.pixelSize)
                {
                    //获得归一化的单位向量，长度为1
                    Vector3 unitVector = Vector3.Normalize(npcNextWorldPosition - transform.position);
                    Vector2 move = new Vector2(unitVector.x * npcMoveSpeed * Time.fixedDeltaTime,
                        unitVector.y * npcMoveSpeed * Time.fixedDeltaTime);
                    rigidBody2D.MovePosition(rigidBody2D.position + move);

                    yield return waitForFixedUpdate;
                }
            }
        }

        rigidBody2D.position = npcNextWorldPosition;
        npcCurrentGridPosition = gridPosition;
        npcNextGridPosition = npcCurrentGridPosition;
        npcIsMoving = false;
    }

    private void SetIdleAnimation()
    {
        animator.SetBool(Settings.idleDown, true);
    }

    private void SetMoveAnimation(Vector3Int gridPosition)
    {
        ResetIdleAnimation();
        ResetMoveAnimation();

        Vector3 toWorldPosition = GetWorldPosition(gridPosition);
        Vector3 directionVector = toWorldPosition - transform.position;

        if (MathF.Abs(directionVector.x) >= MathF.Abs(directionVector.y))
        {
            if (directionVector.x > 0) animator.SetBool(Settings.npcWalkRight, true);
            else animator.SetBool(Settings.npcWalkLeft, true);
        }
        else
        {
            if (directionVector.y > 0) animator.SetBool(Settings.npcWalkUp, true);
            else animator.SetBool(Settings.npcWalkDown, true);
        }
    }

    private void SetNPCEventAnimation()
    {
        if(npcTargetAnimationClip != null)
        {
            ResetIdleAnimation();
            overrideController[blankAnimation] = npcTargetAnimationClip;
            animator.SetBool(Settings.npcEventAnimation, true);
        }
        else
        {
            overrideController[blankAnimation] = blankAnimation;
            animator.SetBool(Settings.npcEventAnimation, false);
        }
    }

    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        npcTargetScene = npcScheduleEvent.toSceneName;
        npcTargetGridPosition = (Vector3Int)npcScheduleEvent.toGridCoordinate;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);
        npcTargetAnimationClip = npcScheduleEvent.animationAtDestination;
        npcFacingDirectionAtDestination = npcScheduleEvent.npcFacingDirectionAtDestination;
        ClearNPCEventAnimation();
    }

    private void ClearNPCEventAnimation()
    {
        overrideController[blankAnimation] = blankAnimation;
        animator.SetBool(Settings.npcEventAnimation, false);
        transform.rotation = Quaternion.identity;
    }

    private void SetNPCFacingDirection()
    {
        ResetIdleAnimation();
        switch (npcFacingDirectionAtDestination)
        {
            case Direction.up:
                animator.SetBool(Settings.idleUp, true);
                break;
            case Direction.down:
                animator.SetBool(Settings.idleDown, true);
                break;
            case Direction.left:
                animator.SetBool(Settings.idleLeft, true);
                break;
            case Direction.right:
                animator.SetBool(Settings.idleRight, true);
                break;
            case Direction.none:
            default:
                break;
        }

    }

    private void ResetMoveAnimation()
    {
        animator.SetBool(Settings.npcWalkDown, false);
        animator.SetBool(Settings.npcWalkUp, false);
        animator.SetBool(Settings.npcWalkLeft, false);
        animator.SetBool(Settings.npcWalkRight, false);
    }

    private void ResetIdleAnimation()
    {
        animator.SetBool(Settings.idleDown, false);
        animator.SetBool(Settings.idleUp, false);
        animator.SetBool(Settings.idleLeft, false);
        animator.SetBool(Settings.idleRight, false);
    }

    public void SetNPCActiveInScene()
    {
        spriteRenderer.enabled = true;
        boxCollider2D.enabled = true;
        npcActiveInScene = true;
    }

    public void SetNPCInactiveInScene()
    {
        spriteRenderer.enabled = false;
        boxCollider2D.enabled = false;
        npcActiveInScene = false;
    }

    private void AfterSceneLoad()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        if(!npcInitialised)
        {
            InitialiseNPC();
            npcInitialised = true;
        }
        sceneLoaded = true;
    }

    private void BeforeSceneUnloaded()
    {
        sceneLoaded = false;
    }

    private void InitialiseNPC()
    {
        if(npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            SetNPCActiveInScene();
        }
        else
        {
            SetNPCInactiveInScene();
        }

        npcPreviousMovementStepScene = npcCurrentScene;
        npcCurrentGridPosition = GetGridPosition(transform.position);
        npcNextGridPosition = npcCurrentGridPosition;
        npcNextWorldPosition = GetWorldPosition(npcCurrentGridPosition);
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);
    }

    private Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        //获得中心点位置
        return new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y + Settings.gridCellSize / 2, worldPosition.z);
    }

    private Vector3Int GetGridPosition(Vector3 gridPosition)
    {
        if (grid != null) 
        {
            return grid.WorldToCell(gridPosition);
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    public void CancelNPCMovement()
    {
        npcPath.ClearPath();
        npcNextGridPosition = Vector3Int.zero;
        npcNextWorldPosition = Vector3.zero;
        npcIsMoving = false;

        if(moveToGridPositionRoutine != null)
        {
            StopCoroutine(moveToGridPositionRoutine);
        }

        ResetMoveAnimation();
        ClearNPCEventAnimation();
        npcTargetAnimationClip = null;
        ResetIdleAnimation();
        SetIdleAnimation();
    }
}
