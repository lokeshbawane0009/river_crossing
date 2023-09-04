using MoreMountains.NiceVibrations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Actor : MonoBehaviour
{
    public RuleObject ruleObject;
    public bool canSteer;
    public int cost;
    [SerializeField] bool onRaft;
    public bool SetOnLeft=true;
    bool onLeft;
    public Vector3 originalPos;
    [HideInInspector] public Transform originalParent;
    new Collider collider;
    public UnityEvent failEvent;

    public UnityEvent panicEvent;
    public bool inanimated;
    public Transform inRaftPosition;

    public bool OnRaft
    {
        get => onRaft;
        set
        {
            onRaft = value;
            collider.enabled = false;

            if (onRaft)
            {
                bool success = Raft.Instance.AddPassenger(this);
                if (!success)
                {
                    onRaft = false;
                    collider.enabled = true;
                }
            }
            else
            {
                Raft.Instance.RemovePassenger(this);
            }
        }
    }

    public bool OnLeft 
    { 
        get => onLeft;
        set 
        { 
            onLeft = value;
            GameplayManager.instance.SetPosition(onLeft, this);
        }
    }

    private void OnEnable()
    {
        collider = GetComponent<Collider>();
    }

    private void Start()
    {
        //If on left we can set the position , otherwise use the position set in inspector
        if (SetOnLeft)
            originalPos = transform.position;
        
        originalParent = transform.parent;
        
        OnLeft = SetOnLeft;

        if(TryGetComponent<Animator>(out Animator animator))
            animator.Play("Idle", -1, Random.Range(0.0f, 1.0f));

    }

    public void EnableCollider()
    {
        collider.enabled = true;
    }

    public void DisableCollider()
    {
        collider.enabled = false;
    }

    private void OnValidate()
    {
        canSteer = ruleObject.canSteer;
        cost = ruleObject.cost;
        inanimated = ruleObject.inanimated;
    }

    private void OnMouseDown()
    {
        if (IsPointerOverUIObject() || DialoguePanel.instructionPlaying || GameplayManager.instance.GameSet)
            return;

        if (Raft.Instance.OnRight == OnLeft)
            return;

        MMVibrationManager.Haptic(HapticTypes.Selection);

        OnRaft = !OnRaft;

        //if (!DOTween.IsTweening(transform.GetInstanceID()))
        //    transform.DOPunchScale(Vector3.one * 1.02f, 0.15f, 1, 0)
        //        .SetEase(Ease.InOutBack)
        //        .OnComplete(() => transform.localScale = Vector3.one)
        //        .SetId(transform.GetInstanceID());
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
