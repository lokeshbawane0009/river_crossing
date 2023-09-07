using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raft : MonoBehaviour
{
    [SerializeField] Ease ease = Ease.InSine;
    [SerializeField] AnimationCurve curve;

    public static Raft Instance;

    public AudioClip raftSail;

    public List<Actor> passengers;
    public List<Transform> inRaftPositions;

    List<Transform> emptyPositions = new List<Transform>();
    public List<GameObject> drownFX;

    public float jumpTime=0.5f;

    //Must be equal to count of totalPassengers
    public List<Transform> rightIslandPositions;

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

            /*Rotate Characters to face the raft moving direction*/

            Vector3 rotationVectorforActor= Vector3.zero;

            if (OnRight)
            {
                rotationVectorforActor = new Vector3(0, 180, 0);
            }

            clonePassengers.ForEach(x => 
            {
                if (x.inanimated != true)
                {
                    x.transform.DORotate(rotationVectorforActor, 0.5f).OnStart(() =>
                    {
                        if (x.TryGetComponent<Animator>(out Animator anim))
                            anim.SetTrigger("Turn");
                    });
                }
            });


            //If timebased game check the rules
            if (GameplayManager.instance.TimeGame)
            {
                bool isSuccess = GameplayManager.instance.CheckForRules();

                if (!isSuccess)
                {
                    GameplayManager.instance.GameSet = true;
                    transform.DOLocalMove(GameplayManager.instance.midPoint, 5f).SetEase(Ease.Linear);
                    return;
                }
            }

            //bool to check if raft is OverWeight
            bool isOverWeight = isRaftOverWeight();
            AudioManager.instance.PlayOneShot(raftSail);
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
                // if raft is overweight then drown the raft
                GameplayManager.instance.GameSet = true;
                transform.DOLocalMove(GameplayManager.instance.midPoint, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    clonePassengers.ForEach(x => x.panicEvent?.Invoke());
                    drownFX.ForEach(x => x.SetActive(true));
                    transform.DOLocalMoveY(transform.position.y - 5f, 20f).SetEase(Ease.Linear);
                });
            }



            if (!GameplayManager.instance.TimeGame)
                StartCoroutine(StartCheckingIsland());
        }
    }

    public int CurrentCost 
    { 
        get => currentCost;
        set
        {
            currentCost = value;
            UIManager.Instance.SetWeights(value, maxLimit);
        }
    }
    public bool CanSteer
    {
        get => canSteer; set
        {
            canSteer = value;

            if (GameplayManager.instance.isHintActivated)
                return;

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
        originalYHeight = transform.localPosition.y;
        CurrentCost = 0;
        CanSteer = false;
    }

    public bool AddPassenger(Actor actor)
    {
        /*
        if (CurrentCost + actor.cost > maxLimit)
            return false;*/

        //Kill any tween running on actor
        DOTween.Kill(actor.transform);

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

        actor.inRaftPosition = pos;

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
            actor.transform.DOMove(pos.position, jumpTime).SetEase(curve)
             .OnStart(() =>
             {
                 if (actor.TryGetComponent<Animator>(out Animator anim))
                     anim.SetTrigger("Jump");
             })
            .OnComplete(() =>
            {
                OnRaftCompleteCallback(actor, pos);
            });
        }
        else
        {
            actor.transform.DOJump(pos.position, 1, 1, jumpTime).SetEase(curve)
            .OnComplete(() =>
            {
                OnRaftCompleteCallback(actor, pos);
            });
        }

        if (steeringActor == null && actor.canSteer)
            steeringActor = actor;

        return true;
    }

    /// <summary>
    /// Add actor on Raft CallBack
    /// </summary>
    /// <param name="actor">Actor which is going on Raft</param>
    /// <param name="pos">Position of jump</param>
    //Add Actor on Raft Callback
    public void OnRaftCompleteCallback(Actor actor,Transform pos)
    {
        actor.transform.position = pos.position;
        actor.transform.parent = transform;
        transform.DOLocalMoveY(transform.localPosition.y - 0.1f, 0.5f).SetEase(Ease.InOutBack).OnComplete(() => transform.DOLocalMoveY(originalYHeight, 0.5f));
        AudioManager.instance.RaftLandingFx();
        Vector3 shakeRot = orgRot;
        shakeRot.x = (pos == inRaftPositions[0]) ? -7f : 7f;
        transform.DORotate(shakeRot, 0.5f).OnComplete(() => transform.DORotate(orgRot, 0.5f)).SetDelay(0.25f);

        if (!GameplayManager.instance.isHintActivated)
            actor.EnableCollider();

        UpdateCanSteer();
    }

    public void RemovePassenger(Actor actor)
    {
        CurrentCost -= actor.cost;
        passengers.Remove(actor);

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
        float rotationTime = 0;
        if (!alternateRot.Equals(actor.transform.eulerAngles))
            rotationTime = 0.3f;

        UpdateEmptyPositions(actor.inRaftPosition);
        actor.inRaftPosition = null;

        if (!actor.inanimated)
        {
            actor.transform.DORotate(alternateRot, rotationTime)
            .OnStart(() =>
            {
                if (actor.TryGetComponent<Animator>(out Animator anim))
                    anim.SetTrigger("Turn");
            })
           .OnComplete(() =>
           {
               actor.transform.DOMove(finalPos, jumpTime).SetEase(curve)
               .OnStart(() =>
               {

                   actor.transform.parent = actor.originalParent;

                   if (actor.TryGetComponent<Animator>(out Animator anim))
                       anim.SetTrigger("Jump");

                   transform.DOLocalMoveY(transform.localPosition.y - 0.1f, 0.5f).SetEase(Ease.InOutBack).OnComplete(() => transform.DOLocalMoveY(originalYHeight, 0.5f));
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
                       if(!GameplayManager.instance.isHintActivated)
                        actor.EnableCollider();
                   });
               });
           });
        }
        else
        {
            actor.transform.DOJump(finalPos,1,1, jumpTime).SetEase(curve)
               .OnStart(() => actor.transform.parent = actor.originalParent)
               .OnComplete(() =>
               {
                   actor.transform.position = finalPos;

                   if (!GameplayManager.instance.isHintActivated)
                       actor.EnableCollider();
               });
        }

        if (actor == steeringActor)
            steeringActor = null;

        UpdateCanSteer();
    }

    IEnumerator StartCheckingIsland()
    {
        yield return new WaitForSeconds(1.2f);
        bool isSuccess = GameplayManager.instance.CheckForRules();
        
        if(!isSuccess)
            GameplayManager.instance.GameSet=true;
    }

    public void UpdateCanSteer()
    {
        CanSteer = false;
        var canSteerPassengers = passengers.Find(x => x.canSteer == true);

        if (canSteerPassengers != null)
            CanSteer = true;
    }

    void UpdateEmptyPositions(Transform pos)
    {
        emptyPositions.Add(pos);
    }

    Transform FindEmptyPosition()
    {
        if (emptyPositions.Count > 0)
        {
            Transform pos = emptyPositions[0];
            emptyPositions.Remove(pos);
            return pos;
        }
        return null;
    }

    Transform GetRightIslandPosition(Actor actor)
    {
        int index = GameplayManager.instance.totalActors.IndexOf(actor);
        return rightIslandPositions[index];
    }
}
