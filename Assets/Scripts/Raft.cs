using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Raft : MonoBehaviour
{
    public static Raft Instance;
    public List<Actor> passengers;
    public RaftPath path;
    public List<Transform> inRaftPositions;
    List<Transform> emptyPositions = new List<Transform>();

    //Must be equal to count of totalPassengers
    public List<Transform> rightIslandPositions;

    public int posCounter;
    public int maxLimit;
    public Actor steeringActor;

    [SerializeField] int currentCost;
    [SerializeField] bool onRight;

    private bool canSteer;

    public bool OnRight
    {
        get => onRight;
        set
        {
            onRight = value;

            List<Actor> clonePassengers = new List<Actor>();
            clonePassengers.AddRange(this.passengers);

            if(GameplayManager.instance.TimeGame)
            {
                bool isSuccess = GameplayManager.instance.CheckForRules();

                if (!isSuccess)
                    UIManager.Instance.ActivateFailPanel();
            }

            if (onRight)
            {
                //transform.DORotate(new Vector3(0,-80,0), 1f,RotateMode.FastBeyond360).SetEase(Ease.Linear);
                //transform.DOPath(path.pointsForRight, 1f, path.pathType, path.pathMode, path.pathResolution, Color.green).SetEase(Ease.Linear).OnComplete(() =>
                //{
                //    clonePassengers.ForEach(x => x.OnRaft = false);
                //});
                transform.DOMove(GameplayManager.instance.raftRightPos, 1f).OnComplete(() =>
                {
                    clonePassengers.ForEach(x => x.OnRaft = false);
                });
            }
            else
            {
                //transform.DORotate(new Vector3(0,100,0), 1f).SetEase(Ease.Linear);
                //transform.DOPath(path.pointsForLeft, 1f, path.pathType, path.pathMode, path.pathResolution, Color.green).SetEase(Ease.Linear).OnComplete(() =>
                //{
                //    clonePassengers.ForEach(x => x.OnRaft = false);
                //});
                transform.DOMove(GameplayManager.instance.raftLeftPos, 1f).OnComplete(() =>
                {
                    clonePassengers.ForEach(x => x.OnRaft = false);
                });
            }

            if(!GameplayManager.instance.TimeGame)
                StartCoroutine(StartChecking());
        }
    }

    public int CurrentCost { get => currentCost; set => currentCost = value; }
    public bool CanSteer
    {
        get => canSteer; set
        {
            canSteer = value;

            if (canSteer)
            {
                UIManager.Instance.moveBtn.gameObject.SetActive(true);
            }
            else
            {
                UIManager.Instance.moveBtn.gameObject.SetActive(false);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        steeringActor = null;
        emptyPositions.AddRange(inRaftPositions);
        CanSteer = false;
    }

    public bool AddPassenger(Actor actor)
    {
        if (CurrentCost+actor.cost > maxLimit)
            return false;

        //Only incase of Dislike Game
        if (GameplayManager.instance.DislikeGame)
        {
            List<Actor> potential_Actors=new List<Actor>();
            potential_Actors.AddRange(passengers);
            potential_Actors.Add(actor);

            if (potential_Actors.Count >= 2)
            {
                bool likes = GameplayManager.instance.CheckForDislike(potential_Actors);
                if (!likes)
                {
                    return false;
                }
            }
        }

        var pos = FindEmptyPosition();

        if (pos == null)
            return false;

        CurrentCost += actor.cost;
        passengers.Add(actor);

        actor.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                actor.transform.position = pos.position;
                actor.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    actor.transform.parent = transform;
                    actor.EnableCollider();
                    UpdateCanSteer();
                });
            });

        posCounter++;

        if (steeringActor == null && actor.canSteer)
            steeringActor = actor;

        return true;
    }

    public void RemovePassenger(Actor actor)
    {
        CurrentCost -= actor.cost;
        passengers.Remove(actor);
        posCounter--;
        Vector3 finalPos = Vector3.zero;
        Vector3 finalRot= Vector3.zero;

        if (!OnRight)
        {
            actor.OnLeft = true;
            finalPos = actor.originalPos;
            finalRot = new Vector3(0,180,0);
        }
        else
        {
            actor.OnLeft = false;
            finalPos = GetRightIslandPosition(actor).position;
            finalRot = Vector3.zero;
        }

        actor.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                
                actor.transform.position = finalPos;
                actor.transform.eulerAngles = finalRot;

                actor.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    actor.transform.parent = actor.originalParent;
                    actor.EnableCollider();
                    CheckEmptyPositions();
                });
            });

        if (actor == steeringActor)
            steeringActor = null;

        UpdateCanSteer();
    }

    IEnumerator StartChecking()
    {
        yield return new WaitForSeconds(1.2f);
        bool isSuccess=GameplayManager.instance.CheckForRules();

        if (!isSuccess)
            UIManager.Instance.ActivateFailPanel();
    }

    public void UpdateCanSteer()
    {
        CanSteer = false;
        var canSteerPassengers = passengers.Find(x => x.canSteer == true);

        if (canSteerPassengers != null)
            CanSteer = true;
    }

    void CheckEmptyPositions()
    {
        emptyPositions.AddRange(inRaftPositions);
        emptyPositions = emptyPositions.Distinct().ToList();

        for (int i = 0; i < emptyPositions.Count; i++)
        {
            var colliders = Physics.CheckSphere(emptyPositions[i].position, 0.1f);

            if (colliders)
            {
                emptyPositions.Remove(emptyPositions[i]);
            }
        }
    }

    Transform FindEmptyPosition()
    {
        for (int i = 0; i < emptyPositions.Count; i++)
        {
            var colliders = Physics.CheckSphere(emptyPositions[i].position, 0.1f);

            if (!colliders)
            {
                var emptyPos = emptyPositions[i].transform;
                emptyPositions.Remove(emptyPositions[i]);
                return emptyPos;
            }
        }
        return null;
    }

    Transform GetRightIslandPosition(Actor actor)
    {
        int index = GameplayManager.instance.totalActors.IndexOf(actor);
        return rightIslandPositions[index];
    }
}
