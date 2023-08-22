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

    private void Start()
    {
        //If on left we can set the position , otherwise use the position set in inspector
        if(SetOnLeft)
            originalPos = transform.position;
        
        originalParent = transform.parent;
        collider = GetComponent<Collider>();
        OnLeft = SetOnLeft;
    }

    public void EnableCollider()
    {
        collider.enabled = true;
    }

    private void OnValidate()
    {
        canSteer = ruleObject.canSteer;
        cost = ruleObject.cost;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Raft.Instance.OnRight == OnLeft)
            return;

        OnRaft = !OnRaft;

        //if (!DOTween.IsTweening(transform.GetInstanceID()))
        //    transform.DOPunchScale(Vector3.one * 1.02f, 0.15f, 1, 0)
        //        .SetEase(Ease.InOutBack)
        //        .OnComplete(() => transform.localScale = Vector3.one)
        //        .SetId(transform.GetInstanceID());
    }
}
