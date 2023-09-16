﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSDK.Core;
using SSDK.Core.Structures.Graphs;
using SSDK.Core.Structures.Primitive;

namespace SSDK.AI.Solvers
{
    /// <summary>
    /// Depicts a Greedy Best-First Search solver for an AI agent.
    /// <br/>
    /// Greedy Best-First Search has the following: <br/>
    /// * To be used for solving a problem all at once (resulting in a set of subsequently achievable states) <br/>
    /// * Assumes that actions are deterministic <br/>
    /// * Requires Hash and Equals to be implemented in problem space <br/>
    /// + Path detection, but not always optimal <br/>
    /// - Heavy memory consumption for exponential states, however guiding the agent into sub-problems may alleviate this problem.  <br/>
    /// - Depends on exact state computation <br/>
    /// - An informed search algorithm requiring a heuristically developed DistanceTo function.
    /// </summary>
    public class GBFSSolver : AgentSolver
    {

        public override bool Check(Agent agent, AgentOperation operation)
        {
            // Always believe that an operation resulting from Greedy Best First Search is accurate.
            return true;
        }

        /// <summary>
        /// Solves the agent by using Greedy Best First Search to generate an operation that computes
        /// the closest path while accounting for action costs.
        /// </summary>
        /// <param name="agent">the agent to solve</param>
        /// <returns>an operation attempts to lead the agent to the desired space</returns>
        public override AgentOperation Solve(Agent agent)
        {
            // Solve GBFS by constructing expanding graph.
            Graph<AgentProblemSpace> graph = new Graph<AgentProblemSpace>();
            GraphVertex<AgentProblemSpace> startingNode = graph.Add(agent.CurrentProblemSpace);

            // Generate queue for problem spaces
            PriorityQueue<GraphVertex<AgentProblemSpace>, UncontrolledNumber> frontierQueue = new ();

            // Initialise the explore set and frontier set.
            Dictionary<AgentProblemSpace, GraphVertex<AgentProblemSpace>> exploredSet = new ();
            Dictionary<AgentProblemSpace, GraphVertex<AgentProblemSpace>> frontierSet = new ();
            frontierSet.Add(agent.CurrentProblemSpace, startingNode);
            
            frontierQueue.Enqueue(startingNode, 0);
            
            // Explore BFS until distance from desired node to starting node is completed.
            // Form edges and graph
            while(frontierQueue.Count > 0)
            {
                // Dequeue highest priority (lowest-weighted vertex)
                GraphVertex<AgentProblemSpace> explorationVertex = frontierQueue.Dequeue();

                UncontrolledNumber explorationCost = explorationVertex.LeadingWeight;
                AgentProblemSpace explorationSpace = explorationVertex.Value;

                frontierSet.Remove(explorationSpace);
                exploredSet.Add(explorationSpace, explorationVertex);

                // Check if desired state is found.
                double dist = explorationSpace.DistanceTo(agent.DesiredProblemSpace);
                if (dist <= MatchTolerance)
                {
                    // Generate operation based on found path, where all edges to should have one element only.
                    AgentOperation newOperation = new AgentOperation();
                    while (explorationVertex != startingNode)
                    {
                        newOperation.Merge(explorationVertex.LeadingEdge.Tag as AgentOperation);
                        explorationVertex = explorationVertex.LeadingEdge.VertexFrom;
                    }
                    newOperation.Reverse();
                    return newOperation;
                }

                // Compute every state branch from current state using list of current available operations.
                foreach(AgentOperation operation in AllOperations)
                {
                    AgentProblemSpace newSpace = explorationSpace.Predict(agent, operation);

                    UncontrolledNumber cost = operation.CalculateTotalCost(agent);

                    dist = explorationSpace.DistanceTo(newSpace);

                    if (dist <= MatchTolerance) continue;// Only add new spaces for queue

                    UncontrolledNumber estimatedCost = newSpace.Heuristic(agent.DesiredProblemSpace);
                    GraphVertex<AgentProblemSpace> existingVertex = null;

                    if (exploredSet.TryGetValue(newSpace, out existingVertex) || frontierSet.TryGetValue(newSpace, out existingVertex))
                    {
                        // Our action leads to an already explored/queued vertex, so check to see if this path is closer in cost.
                        if (estimatedCost < existingVertex.LeadingWeight)
                        {
                            // Update the leading edge and weight of the vertex.
                            GraphEdge<AgentProblemSpace> edge = graph.CreatePath(explorationVertex, existingVertex, cost);
                            existingVertex.LeadingEdge = edge;
                            existingVertex.LeadingWeight = estimatedCost;
                        }
                    }
                    else
                    {
                        // Create node in graph and create path from start to new
                        GraphVertex<AgentProblemSpace> newVertex = graph.Add(newSpace);
                        GraphEdge<AgentProblemSpace> edge = graph.CreatePath(explorationVertex, newVertex, cost);
                        newVertex.LeadingEdge = edge;
                        newVertex.LeadingWeight = estimatedCost;
                        edge.Tag = operation;
                        frontierQueue.Enqueue(newVertex, estimatedCost);
                        frontierSet.Add(newSpace, newVertex);
                    }
                }
            }

            // Could not find a way to the desired state, so just use empty operation.
            return new AgentOperation();
        }

        private List<AgentOperation> AllOperations = null;
        public override void UpdateProblem(Agent agent)
        {
            // Simply populate list of actions, assuming that agent actions can't change.
            AllOperations = agent.ActionSpace.AllSingleStepOperations;
        }

        public override string ToString()
        {
            return "Greedy Best-First Search";
        }
    }
}
