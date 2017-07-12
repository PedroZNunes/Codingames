using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/*
 Goal: 
 Your mission is to reprogram the virus so it will sever links in such a way that the Skynet Agent is unable to
 access another sub-network thus preventing information concerning the presence of our virus to reach Skynet's central hub.
 
Initialization input
Line 1: 3 integers N L E
- N, the total number of nodes in the level, including the gateways.
- L, the number of links in the level.
- E, the number of exit gateways in the level.

Next L lines: 2 integers per line (N1, N2), indicating a link between the nodes indexed N1 and N2 in the network.

Next E lines: 1 integer EI representing the index of a gateway node.

Input for one game turn
Line 1: 1 integer SI, which is the index of the node on which the Skynet agent is positioned this turn.
Output for one game turn
A single line comprised of two integers C1 and C2 separated by a space. C1 and C2 are the indices of the nodes you wish to sever the link between.
 */
class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int nodeCount = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        int linkCount = int.Parse(inputs[1]); // the number of links
        int gatewayCount = int.Parse(inputs[2]); // the number of exit gateways
        
        List<Link> links = new List<Link> ();
        int[] gateways = new int[gatewayCount];
        
        for (int i = 0; i < linkCount; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse(inputs[1]);
            links.Add(new Link(N1, N2));
        }
        
        for (int i = 0; i < gatewayCount; i++)
        {
            gateways[i] = int.Parse(Console.ReadLine()); // the index of a gateway node
        }

        // game loop
        while (true)
        {
            int agentNode = int.Parse(Console.ReadLine()); // The index of the node on which the Skynet agent is positioned this turn
            Queue<int> frontier = new Queue<int>();
            frontier.Enqueue(agentNode);
            Dictionary<int,int> cameFrom = new Dictionary<int,int>();
            cameFrom[agentNode] = 0;
            
            while (frontier.Count > 0){
                int thisNode = frontier.Dequeue();
                //Find Neighbours and add to the frontier what has not been visited yet
                for (int i = 0; i < links.Count(); i++){
                    int next;
                    if (links[i].TryLink (thisNode, out next)){
                        if (!cameFrom.ContainsKey(next)){
                            frontier.Enqueue(next);
                            cameFrom[next]=thisNode;
                        }                        
                    }
                }
            }
            
            //Reverse Path so we can find the agent from the goal.
            int[] shortestPath = new int[0];
            for (int i = 0; i < gatewayCount; i++){
                List<int> thisPath = new List<int>();
                int current = gateways[i];
                thisPath.Add(current);
                bool pathExists = true;
                while (current != agentNode){
                    if (cameFrom.ContainsKey(current)){
                        Console.Error.WriteLine("Path[{0}]: {1}", i, cameFrom[current]);
                        current = cameFrom[current];
                        thisPath.Add(current);
                    } else{
                        pathExists = false;
                        break;
                    }
                }
                //Find shortest path
                if ((shortestPath.Length == 0 || shortestPath.Count() > thisPath.Count()) && pathExists){
                    thisPath.Reverse();
                    shortestPath = thisPath.ToArray();
                }
            }
            
            //pick a link to sever
            int closestNode= shortestPath[1];
            for (int i = 0; i < links.Count() ; i++){
                if (links[i].ContainsNode(closestNode) && links[i].ContainsNode(agentNode)){
                    links.RemoveAt(i);
                }
            }
            
            Console.WriteLine(closestNode + " " + agentNode);
            //implementar para vários paths.
            //lidar caso nenhum path seja encontrado.
            //testar o path menor
            //deletar o link que linka o primeiro node ao agente (console.writeline)
        }
        
    }
    
    class Link{
        
        public int[] nodes = new int[2];
        
        public Link (int node1, int node2){
            nodes[0] = node1;
            nodes[1] = node2;
        }
        
        public bool TryLink (int node, out int next){
            if (nodes[0] == node){
                next = nodes[1];
                return true;
            }else if (nodes[1] == node){
                next = nodes[0];
                return true;
            }
            next = 0;
            return false;
        }
        
        public bool ContainsNode (int node){
            if (this.nodes[0] == node || this.nodes[1] == node){
                return true;
            } else{
                return false;
            }
        }
        
    }
    
}
