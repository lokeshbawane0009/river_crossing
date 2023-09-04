using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ActorAction
{
    public Actor actor;
    public bool RequiredOnRaft;
    public bool RequiredOnLeftSide;
}

[Serializable]
public struct HintStep
{
    public List<ActorAction> actions;
}

public class Hints : MonoBehaviour
{
    public List<HintStep> hintSteps;

    bool buttonShown;
    public IEnumerator StartSolution()
    {
        while (hintSteps.Count > 0)
        {
            var currentHint = hintSteps[0];
            UIManager.Instance.HideMoveBtn();
            buttonShown = false;
            GameplayManager.instance.DisableAllColliders();

            //Check if On Raft
            yield return new WaitUntil(() =>
            {
                for (int i = 0; i < currentHint.actions.Count; i++)
                {
                    if (currentHint.actions[i].actor.OnRaft != currentHint.actions[i].RequiredOnRaft)
                    {
                        UIManager.Instance.hintHand.gameObject.SetActive(true);

                        TweenHand();

                        UIManager.Instance.hintHand.anchoredPosition
                        = UIManager.Instance.WorldSpaceToCanvas(currentHint.actions[i].actor.transform.position) + Vector2.up * 100f;

                        currentHint.actions[i].actor.EnableCollider();
                        return false;
                    }
                    currentHint.actions[i].actor.DisableCollider();
                }
                return true;
            });



            if (!buttonShown)
            {
                buttonShown = true;

                UIManager.Instance.ShowMoveBtn();

            }
            yield return new WaitForSeconds(0.1f);

            yield return new WaitUntil(() =>
            {
                UIManager.Instance.hintHand.position
                        = UIManager.Instance.moveBtn.transform.position;

                if (UIManager.Instance.moveBtn.gameObject.activeInHierarchy)
                    return false;
                else
                    return true;
            });

            DontTweenHand();
            UIManager.Instance.hintHand.gameObject.SetActive(false);

            //Check if Actors position
            yield return new WaitUntil(() =>
            {
                for (int i = 0; i < currentHint.actions.Count; i++)
                {
                    currentHint.actions[i].actor.DisableCollider();
                    if (currentHint.actions[i].actor.OnLeft != currentHint.actions[i].RequiredOnLeftSide)
                    {
                        return false;
                    }
                }
                return true;
            });

            hintSteps.RemoveAt(0);
        }
        PlayerPrefs.SetInt("RC_HintPlay", 0);
    }

    public void TweenHand()
    {
        if (!DOTween.IsTweening(UIManager.Instance.hintHand))
        {
            UIManager.Instance.hintHand.localScale = Vector3.one;
            UIManager.Instance.hintHand.DOScale(Vector3.one * 0.7f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void DontTweenHand()
    {
        DOTween.Kill(UIManager.Instance.hintHand);
        UIManager.Instance.hintHand.localScale = Vector3.one;
    }

}


