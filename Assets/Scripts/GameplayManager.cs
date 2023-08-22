using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    public Vector3 raftLeftPos;
    public Vector3 raftRightPos;
    public Vector3 midPoint;

    public List<Actor> totalActors;

    [SerializeField] List<Actor> actorsOnLeft;
    [SerializeField] List<Actor> actorsOnRight;

    public bool NoRuleGame;
    public bool DislikeGame;
    public bool WinRule;
    public WinRule winRule;

    [Header("Time Game Parameters")]
    public bool TimeGame;
    public int totalTime;

    public Rules rules;

    private void Awake()
    {
        instance = this;
        midPoint = (raftLeftPos + raftRightPos) / 2;
    }

    public void SetPosition(bool onLeft, Actor actor)
    {
        if (onLeft)
        {
            if (actorsOnRight.Contains(actor))
            {
                actorsOnRight.Remove(actor);
            }

            if (!actorsOnLeft.Contains(actor))
                actorsOnLeft.Add(actor);
        }
        else
        {
            if (actorsOnLeft.Contains(actor))
            {
                actorsOnLeft.Remove(actor);
            }

            if (!actorsOnRight.Contains(actor))
                actorsOnRight.Add(actor);

            if(actorsOnRight.Count==totalActors.Count && !WinRule)
            {
                UIManager.Instance.ActivateSuccessPanel();
            }
        }

        if (WinRule)
        {
            if (winRule.CheckForWin())
            {
                UIManager.Instance.ActivateSuccessPanel();
            }
        }
    }

    public bool CheckForRules()
    {
        if (NoRuleGame)
            return true;

        if (TimeGame)
        {
            totalTime -= (int)CheckTimeToCross();
            
            if(totalTime<=0)
                return false;

            return true;
        }

        if (!CheckLeftSide()) return false;
        if (!CheckRightSide()) return false;
        return true;
    }

    public List<Rule> FindApplicableRule(RuleObject ruleObject)
    {
        var myRules = rules.rules.FindAll(x => x.firstObj == ruleObject || x.forObjects.Contains(ruleObject));
        return myRules;
    }

    public bool CheckLeftSide()
    {
        bool isCorrect = true;
        for (int i = 0; i < actorsOnLeft.Count; i++)
        {
            Debug.Log(i+"). OnLeft :"+actorsOnLeft[i].name);

            isCorrect = isRuleSatisfied(actorsOnLeft[i],actorsOnLeft);

            if (!isCorrect)
            {
                Debug.Log(actorsOnLeft[i].name + " rule not satisfied");
                actorsOnLeft[i].failEvent?.Invoke();
                return false;
            }
        }
        return true;
    }

    public bool CheckRightSide()
    {
        bool isCorrect = true;
        for (int i = 0; i < actorsOnRight.Count; i++)
        {
            Debug.Log(i+"). OnRight :"+actorsOnRight[i].name);
            isCorrect = isRuleSatisfied(actorsOnRight[i],actorsOnRight);

            if (!isCorrect)
            {
                Debug.Log(actorsOnRight[i].name + " rule not satisfied");
                actorsOnRight[i].failEvent?.Invoke();
                return false;
            }
        }
        return true;
    }

    public bool CheckForDislike(List<Actor> actors)
    {
        bool isCorrect = true;
        for (int i = 0; i < actors.Count; i++)
        {
            Debug.Log(i + "). OnRaft :" + actors[i].name);
            isCorrect = isRuleSatisfied(actors[i], actors);

            if (!isCorrect)
            {
                Debug.Log(actors[i].name + " rule not satisfied");
                return false;
            }
        }
        return true;
    }

    public bool isRuleSatisfied(Actor actor,List<Actor> actors)
    {
        List<Rule> myRules = FindApplicableRule(actor.ruleObject);
        myRules = myRules.Distinct().ToList();
        for (int i = 0; i < myRules.Count; i++)
        {
            if(!ConditionSatisifed(myRules[i],actors))
                return false;
        }
        return true;
    }

    public bool ConditionSatisifed(Rule rule, List<Actor> actors)
    {
        List<RuleObject> ruleObjectsInActors= new List<RuleObject>();
        
        for(int i=0;i < actors.Count;i++)
        {
            ruleObjectsInActors.Add(actors[i].ruleObject);
        }

        switch (rule.condition)
        {
            case Condition.conditionStatement.must_be_together:
                {
                    if (ruleObjectsInActors.Contains(rule.firstObj) && ruleObjectsInActors.Contains(rule.secondObj))
                    {
                        Debug.Log(rule.firstObj.name + " and " + rule.secondObj.name + " must be together");
                        return true;
                    }
                    return false;
                }
            case Condition.conditionStatement.cannot_be_together: 
                {
                    if (ruleObjectsInActors.Contains(rule.firstObj) && !ruleObjectsInActors.Contains(rule.secondObj))
                    {
                        return true;
                    }

                    if (rule.inAbsenceOf)
                    {
                        if (!ruleObjectsInActors.Contains(rule.inAbsenceOf))
                        {
                            Debug.Log(rule.firstObj.name + " and " + rule.secondObj.name + " cannot be together");
                            return false;
                        }
                        else
                            return true;
                    }
                    else
                    {
                        if (ruleObjectsInActors.Count > 2)
                            return true;
                        else
                        {
                            Debug.Log(rule.firstObj.name + " and " + rule.secondObj.name + " cannot be together");
                            return false;
                        }
                    }
                    
                }

            case Condition.conditionStatement.cannot_be_alone:
                {
                    if (ruleObjectsInActors.Contains(rule.firstObj) && ruleObjectsInActors.Count == 1)
                    {
                        Debug.Log(rule.firstObj.name + " cannot be alone");
                        return false;
                    }
                    return true;
                }
            case Condition.conditionStatement.cannot_outnumber:
                {

                    int count_A=ruleObjectsInActors.FindAll(x=>x==rule.firstObj).Count();
                    int count_B=ruleObjectsInActors.FindAll(x=>x==rule.secondObj).Count();
                    
                    //If no secondObject
                    if(count_B==0)
                        return true;

                    if (count_A > count_B)
                    {
                        Debug.Log(rule.firstObj.name + " outnumbered " + rule.secondObj);
                        return false;
                    }
                    return true;
                }
        }

        //No rules found
        return false;
    }

    public float CheckTimeToCross()
    {
        float[] values = new float[Raft.Instance.passengers.Count];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = Raft.Instance.passengers[i].GetComponent<TimeGame>().timeValue;
        }

        if (values.Length >= 2)
            return FindMaxTime(values);
        else
            return values[0];
    }


    float FindMaxTime(float[] values)
    {
        return Mathf.Max(values);
    }
}
