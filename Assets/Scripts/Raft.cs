using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Raft : MonoBehaviour
{
    [SerializeField] Ease ease = Ease.InSine;
    [SerializeField] AnimationCurve curve;
    public static Raft Instance;
    public List<Actor> passengers;
    public RaftPath path;
    public List<Transform> inRaftPositions;
    List<Transform> emptyPositions = new List<Transform>();
    public List<GameObject> drownFX;

    //Must be equal to count of totalPassengers
    public List<Transform> rightIslandPositions;

    public int posCounter;
    public int maxLimit;
    public Actor steeringActor;

    [SerializeField] int currentCost;
    [SerializeField] bool onRight;
    [SerializeField] bool canSteer;

    Vector3 orgRot;
    float originalYHeight;

    public bool isRaftOverWeight()
    {
        if (currentCost > maxLimit)
            return true;

        return false;
    }

    public bool OnRight
    {
        get => onRight;
        set
        {
            onRight = value;

            List<Actor> clonePassengers = new List<Actor>();
            clonePassengers.AddRange(this.passengers);

            if (GameplayManager.instance.TimeGame)
            {
                bool isSuccess = GameplayManager.instance.CheckForRules();

                if (!isSuccess)
                    UIManager.Instance.ActivateFailPanel();
            }

            bool isOverWeight = isRaftOverWeight();

            if (!isOverWeight)
            {
                if (onRight)
                {
                    transform.DOLocalMove(GameplayManager.instance.raftRightPos, 1f).OnComplete(() =>
                    {
                        clonePassengers.ForEach(x => x.OnLeft = false);
                        UpdateCanSteer();
                    });
                }
                else
                {
                    transform.DOLocalMove(GameplayManager.instance.raftLeftPos, 1f).OnComplete(() =>
                    {
                        clonePassengers.ForEach(x => x.OnLeft = true);
                        UpdateCanSteer();
                    });
                }
            }
            else
            {
                transform.DOLocalMove(GameplayManager.instance.midPoint, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    clonePassengers.ForEach(x => x.panicEvent?.Invoke());
                    drownFX.ForEach(x => x.SetActive(true));
                    transform.DOLocalMoveY(transform.position.y - 5f, 30f).SetEase(Ease.Linear);
                });
            }



            if (!GameplayManager.instance.TimeGame)
                StartCoroutine(StartCheckingIsland());
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
                UIManager.Instance.ShowMoveBtn();
            }
            else
            {
                UIManager.Instance.HideMoveBtn();
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
        orgRot = transform.eulerAngles;
        originalYHeight = transform.position.y;
        CanSteer = false;
    }

    public bool AddPassenger(Actor actor)
    {
        /*
        if (CurrentCost + actor.cost > maxLimit)
            return false;*/

        //Only incase of Dislike Game
        if (GameplayManager.instance.DislikeGame)
        {
            List<Actor> potential_Actors = new List<Actor>();
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

        /* Old Implement
        //actor.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack)
        //    .OnComplete(() =>
        //    {
        //        actor.transform.position = pos.position;
        //        actor.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
        //        {
        //            actor.transform.parent = transform;
        //            actor.EnableCollider();
        //            UpdateCanSteer();
        //        });
        //    });
        */

        if (!actor.inanimated)
        {
            actor.transform.DOMove(pos.position, .75f).SetEase(curve)
             .OnStart(() =>
             {
                 if (actor.TryGetComponent<Animator>(out Animator anim))
                     anim.SetTrigger("Jump");
             })
            .OnComplete(() =>
            {
                actor.transform.position = pos.position;
                actor.transform.parent = transform;
                transform.DOLocalMoveY(transform.position.y - 0.1f, 0.5f).SetEase(Ease.InOutBack).OnComplete(() => transform.DOLocalMoveY(originalYHeight, 0.5f));

                Vector3 shakeRot = orgRot;
                shakeRot.x = (pos == inRaftPositions[0]) ? -7f : 7f;
                transform.DORotate(shakeRot, 0.5f).OnComplete(() => transform.DORotate(orgRot, 0.5f)).SetDelay(0.25f);

                actor.EnableCollider();
                UpdateCanSteer();
            });
        }
        else
        {
            actor.transform.DOJump(pos.position, 1, 1, 0.75f).SetEase(curve)
            .OnComplete(() =>
            {
                actor.transform.position = pos.position;
                actor.transform.parent = transform;
                transform.DOLocalMoveY(transform.position.y - 0.1f, 0.5f).SetEase(Ease.InOutBack).OnComplete(() => transform.DOLocalMoveY(originalYHeight, 0.5f));

                Vector3 shakeRot = orgRot;
                shakeRot.x = (pos == inRaftPositions[0]) ? -7f : 7f;
                transform.DORotate(shakeRot, 0.5f).OnComplete(() => transform.DORotate(orgRot, 0.5f)).SetDelay(0.25f);

                actor.EnableCollider();
                UpdateCanSteer();
            });
        }


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
        Vector3 finalRot = Vector3.zero;
        Vector3 alternateRot = Vector3.zero;

        if (!OnRight)
        {
            actor.OnLeft = true;
            finalPos = actor.originalPos;
            finalRot = new Vector3(0, 180, 0);
            alternateRot = Vector3.zero;
        }
        else
        {
            actor.OnLeft = false;
            finalPos = GetRightIslandPosition(actor).position;
            finalRot = Vector3.zero;
            alternateRot = new Vector3(0, 180, 0);
        }

        /*Old Implement
        //actor.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack)
        //    .OnComplete(() =>
        //    {

        //        actor.transform.position = finalPos;
        //        actor.transform.eulerAngles = finalRot;

        //        actor.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
        //        {
        //            actor.transform.parent = actor.originalParent;
        //            actor.EnableCollider();
        //            CheckEmptyPositions();
        //        });
        //    });
        */

        //Check facing direction
        float jumpTime = 0;
        if (!alternateRot.Equals(actor.transform.eulerAngles))
            jumpTime = 1;

        if (!actor.inanimated)
        {
            actor.transform.DORotate(alternateRot, jumpTime)
            .OnStart(() =>
            {
                if (actor.TryGetComponent<Animator>(out Animator anim))
                    anim.SetTrigger("Turn");
            })
           .OnComplete(() =>
           {
               actor.transform.DOMove(finalPos, 0.75f).SetEase(curve)
               .OnStart(() =>
               {

                   actor.transform.parent = actor.originalParent;

                   if (actor.TryGetComponent<Animator>(out Animator anim))
                       anim.SetTrigger("Jump");

                   transform.DOLocalMoveY(transform.position.y - 0.1f, 0.5f).SetEase(Ease.InOutBack).OnComplete(() => transform.DOLocalMoveY(originalYHeight, 0.5f));
               })
               .OnComplete(() =>
               {
                   actor.transform.position = finalPos;
                   actor.transform.DORotate(finalRot, 1f)
                   .OnStart(() =>
                   {
                       if (actor.TryGetComponent<Animator>(out Animator anim))
                           anim.SetTrigger("Turn");
                   })
                   .OnComplete(() =>
                   {
                       actor.EnableCollider();
                       UpdateEmptyPositions();
                   });

               });
           });
        }
        else
        {
            actor.transform.DOJump(finalPos,1,1, 0.75f).SetEase(curve)
               .OnStart(() => actor.transform.parent = actor.originalParent)
               .OnComplete(() =>
               {
                   actor.transform.position = finalPos;
                    actor.EnableCollider();
                    UpdateEmptyPositions();
               });
        }



        if (actor == steeringActor)
            steeringActor = null;

        UpdateCanSteer();
    }

    IEnumerator StartCheckingIsland()
    {
        yield return new WaitForSeconds(3f);
        bool isSuccess = GameplayManager.instance.CheckForRules();

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

    void UpdateEmptyPositions()
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
