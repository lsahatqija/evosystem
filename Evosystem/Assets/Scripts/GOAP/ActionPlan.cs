using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IGoapPlanner
{
       ActionPlan CreatePlan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null);
}

public class GoapPlanner : IGoapPlanner
{
    public ActionPlan CreatePlan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null)
    {
        // order goals by priority
        List<AgentGoal> orderedGoals = goals
            .Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
            .OrderByDescending(g => g == mostRecentGoal ? g.Priority - 0.01 : g.Priority)
            .ToList();

        foreach (AgentGoal goal in orderedGoals)
        {
            Node goalNode = new Node(null, null, goal.DesiredEffects, 0f);

            // if we can find a path to the goal, return the plan
            if (FindPath(goalNode, agent.actions))
            {
                // if the goal node has no leaves, it means no path was found, so no action can achieve the goal so we try a different goal
                if (goalNode.IsLeafDead) continue;

                Stack<AgentAction> actionStack = new Stack<AgentAction>();
                while (goalNode.Leaves.Count > 0)
                {
                    // get the cheapest leaf
                    Node cheapestLeaf = goalNode.Leaves.OrderBy(leaf => leaf.Cost).First();
                    goalNode = cheapestLeaf;
                    actionStack.Push(cheapestLeaf.Action);
                }

                return new ActionPlan(goal, actionStack, goalNode.Cost);
            }
        }

        Debug.LogWarning("GOAP Planner: No valid plan found for any goal.");
        return null;
    }

    bool FindPath(Node Parent, HashSet<AgentAction> actions)
    {
        var orderedActions = actions.OrderBy(a => a.Cost).ToList();

        foreach (var action in orderedActions)
        {
            var requiredEffects = Parent.RequiredEffects;

            // remove trueeffects from required effects
            requiredEffects.RemoveWhere(b => b.Evaluate());
            if (requiredEffects.Count == 0)
            {
                // goal achieved
                return true;
            }

            if (action.Effects.Any(requiredEffects.Contains))
            {
                var newRequiredEffects = new HashSet<AgentBelief>(requiredEffects);
                newRequiredEffects.ExceptWith(action.Effects);
                newRequiredEffects.UnionWith(action.Preconditions);

                var newAvailableActions = new HashSet<AgentAction>(actions);
                newAvailableActions.Remove(action);

                var newNode = new Node(Parent, action, newRequiredEffects, Parent.Cost + action.Cost);

                if (FindPath(newNode, newAvailableActions))
                {
                    Parent.Leaves.Add(newNode);
                    newRequiredEffects.ExceptWith(newNode.Action.Preconditions);
                    return true;
                }

                if (newRequiredEffects.Count == 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

public class Node
{
    public Node Parent { get; }
    public AgentAction Action { get; }
    public HashSet<AgentBelief> RequiredEffects { get; }
    public List<Node> Leaves { get; }
    public float Cost { get; }

    public bool IsLeafDead => Leaves.Count == 0 && Action == null;

    public Node(Node parent, AgentAction action, HashSet<AgentBelief> requiredEffects, float cost)
    {
        Parent = parent;
        Action = action;
        RequiredEffects = new HashSet<AgentBelief>(requiredEffects);
        Leaves = new List<Node>();
        Cost = cost;
    }
}

public class ActionPlan
{
    public AgentGoal AgentGoal { get; }
    public Stack<AgentAction> Actions { get; }
    public float TotalCost { get; set; }

    public ActionPlan(AgentGoal goal, Stack<AgentAction> actions, float totalCost)
    {
        AgentGoal = goal;
        Actions = actions;
        TotalCost = totalCost;
    }
}