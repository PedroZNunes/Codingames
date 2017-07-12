using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


class Player
{
    static private Link[] links;
    static private List<Node> nodeList;

    static private Node[] gateways;
    static private List<Link> gatewayLinks;
    static private List<Link> severedLinks;

    static private Dictionary<Node , Node> cameFrom; //bfs pathing. where did KEY node came from? VALUE

    static private int linkCount;
    static private int nodeCount;
    static private int gatewayCount;

    static void Main(string[] args)
    {
        int nodesIndex = 0;
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        nodeCount = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        linkCount = int.Parse(inputs[1]); // the number of links
        gatewayCount = int.Parse(inputs[2]); // the number of exit gateways

        nodeList = new List<Node> ();
        links = new Link[linkCount];
        gateways = new Node[gatewayCount];

        MapNodesAndLinks ();

        MapGateways ();

        nodeList.Sort (delegate ( Node n1 , Node n2 ) { return n2.gatewayLinksCount.CompareTo (n1.gatewayLinksCount); });

        Console.Error.Write ("nodeList:");
        for (int j = 0 ; j < nodeList.Count ; j++) {
            Console.Error.Write (" N{0}-{1}" , nodeList[j].index , nodeList[j].gatewayLinksCount);
        }
        Console.Error.WriteLine (".");

        severedLinks = new List<Link> ();

        // game loop
        while (true)
        {
            int agentIndex = int.Parse (Console.ReadLine ());
            Node agentNode = new Node();
            for (int i = 0 ; i < nodeList.Count ; i++) {
                if (nodeList[i].index == agentIndex)
                    agentNode = nodeList[i];
            }

            if (agentNode == new Node ()) {
                Console.Error.WriteLine ("Agente Node not found.");
            }
            
            Link linkToSevere = new Link();

            //if the agent node is present in the gatewaylinks list, just skip the rest.
            for (int i = 0 ; i < gatewayLinks.Count ; i++) {
                if (gatewayLinks[i].Contains (agentNode)) {
                    if (!severedLinks.Contains (gatewayLinks[i]))
                        linkToSevere = gatewayLinks[i];
                    break;
                }
            }

            if (linkToSevere == new Link ()) {
                Console.Error.WriteLine ("No immediate threat found.");
                Node nodeToSevere = new Node ();

                //now lets see which nodes can influence more edges on gateways simultaneously.
                for (int i = 0 ; i < nodeCount ; i++) {
                    
                    if (nodeList[i].gatewayLinksCount == 0)
                        continue;

                    //in case the node has not been set yet, set whatever is next.
                    if (nodeToSevere == new Node ()) {
                        Console.Error.WriteLine ("Node {0} chosen as first instance" , nodeList[i].index);
                        nodeToSevere = nodeList[i];
                    }
                    //take down the most dangerous nodes first, here considered as being the ones with most links to gateways.
                    else if (nodeList[i].gatewayLinksCount > nodeToSevere.gatewayLinksCount) {
                        Console.Error.WriteLine ("Node {0} to be severed. Has {1} connections, while the previous best, {1}, had {2}." , nodeList[i].index , nodeList[i].gatewayLinksCount , nodeToSevere.index , nodeToSevere.gatewayLinksCount);
                        nodeToSevere = nodeList[i];
                    }
                    //in case we have multiple nodes with the same number of connections to gateways, get the one closer to the agent (disconsidering nodes adjacent to gateways).
                    else if (nodeList[i].gatewayLinksCount == nodeToSevere.gatewayLinksCount) {
                        Console.Error.WriteLine ("Nodes {0} and {1} have the same count of gateways: {2}" , nodeList[i].index , nodeToSevere.index , nodeToSevere.gatewayLinksCount);
                        //check which is closer to the enemy
                        if (DistanceBetween (agentNode , nodeList[i]) < DistanceBetween (agentNode , nodeToSevere)) {
                            Console.Error.WriteLine ("Node {0} to be severed. {1} turns from the agent. previous most dangerous node was {2} turns away." , nodeList[i].index , nodeList[i].gatewayLinksCount , nodeToSevere.gatewayLinksCount);
                            nodeToSevere = nodeList[i];
                        }
                    }


                    for (int j = 0 ; j < gatewayLinks.Count ; j++) {
                        if (gatewayLinks[j].Contains (nodeToSevere)) {
                            if (!severedLinks.Contains (gatewayLinks[j])) {
                                linkToSevere = gatewayLinks[j];
                            }
                            else {
                                nodeToSevere = new Node ();
                            }
                        }
                    }
                }

            }

            severedLinks.Add (linkToSevere);
            Console.WriteLine(linkToSevere.ToString());
        }
    }

    static private void MapNodesAndLinks () {
        string[] inputs;

        for (int i = 0 ; i < linkCount ; i++) {
            inputs = Console.ReadLine ().Split (' ');
            int N1 = int.Parse (inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse (inputs[1]);

            //Console.Error.WriteLine ("input {0} {1}", N1, N2);

            Node nodeA = new Node (N1);
            Node nodeB = new Node (N2);

            bool containsNodeA = false;
            bool containsNodeB = false;
            for (int j = 0 ; j < nodeList.Count ; j++) {
                //Console.Error.WriteLine ("Checking if nodelist index {0} is {1} or {2}.", nodeList[j].index, nodeA.index, nodeB.index);
                if (nodeList[j].index == nodeA.index) {
                    nodeA = nodeList[j];
                    //nodeList[j].gatewayLinksCount++;
                    containsNodeA = true;
                    //Console.Error.WriteLine ("node {0} connections improved to {1}.", nodeList[j].index, nodeList[j].gatewayLinksCount);
                }

                if (nodeList[j].index == nodeB.index) {
                    nodeB = nodeList[j];
                    //nodeList[j].gatewayLinksCount++;
                    containsNodeB = true;
                    //Console.Error.WriteLine ("node {0} connections improved to {1}." , nodeList[j].index , nodeList[j].gatewayLinksCount);
                }
            }

            if (!containsNodeA)
                nodeList.Add (nodeA);

            if (!containsNodeB)
                nodeList.Add (nodeB);

            links[i] = new Link (nodeA , nodeB);
            //Console.Error.WriteLine (links[i].ToString ());
        }
    }

    static private void MapGateways () {
        for (int i = 0 ; i < gatewayCount ; i++) {
            int G = int.Parse (Console.ReadLine ());
            for (int j = 0 ; j < nodeList.Count ; j++) {
                if (nodeList[j].index == G) {
                    gateways[i] = nodeList[j];
                    break;
                }
            }
        }

        //get all links adjacent to the gateways, store and count them.
        gatewayLinks = new List<Link> ();
        for (int i = 0 ; i < gateways.Length ; i++) {
            for (int j = 0 ; j < linkCount ; j++) {

                if (links[j].Contains (gateways[i])) {
                    Console.Error.WriteLine ("Link {0} contains Gateway {1}." , links[j] , gateways[i].index);
                    if (!gatewayLinks.Contains (links[j])) {
                        links[j].GetOtherNode (gateways[i]).gatewayLinksCount++;
                        gatewayLinks.Add (links[j]);
                    }
                }

            }
        }
    }

    /// <summary>
    /// Calculates the distance between 2 nodes, disconsidering any nodes adjacent to gateways, for they demand instant reaction.
    /// </summary>
    /// <param name="start">Starting search node</param>
    /// <param name="goal">Goal state</param>
    /// <returns></returns>
    static private int DistanceBetween ( Node start , Node goal ) {

        Console.Error.WriteLine ("Pathing {0} to {1} for distance." , start.index , goal.index);
        //BFS from a to b
        Queue<Node> frontier = new Queue<Node> ();
        cameFrom = new Dictionary<Node , Node> ();
        bool[] hasVisited = new bool[nodeCount];

        Node current = start;
        frontier.Enqueue (current);
        hasVisited[current.index] = true;
        cameFrom[current] = new Node ();

        while (frontier.Count > 0) {
            current = frontier.Dequeue ();

            if (current == goal) {
                Console.Error.WriteLine ("Target found on BFS. {0}", goal.index);
                break;
            }

            Node[] neighbours = GetNeighbours (current);

            for (int i = 0 ; i < neighbours.Length ; i++) {
                if (!hasVisited[neighbours[i].index]) {
                    if (!gateways.Contains (neighbours[i])) {
                        frontier.Enqueue (neighbours[i]);
                        hasVisited[neighbours[i].index] = true;
                        cameFrom[neighbours[i]] = current;
                    }
                }
            }
        }

        //calculate distance
        int n = 0;
        while (current != start) {
            current = cameFrom[current];
            for (int i = 0 ; i < gatewayLinks.Count; i++) {
                //each movement in a gateway link node will demand an instant reaction, which cannot be counted as distance
                if (gatewayLinks[i].Contains (current)) {
                    n--;
                    break;
                }
            }
            n++;
        }
        Console.Error.WriteLine ("Pathing distance: {0}." , n);
        return n;
    }

    /// <summary>
    /// get the nodes adjacent to this one.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    static private Node[] GetNeighbours ( Node node ) {
        List<Node> neighbours = new List<Node> ();

        for (int i = 0 ; i < linkCount ; i++) {
            if (links[i].Contains (node))
                if (!severedLinks.Contains (links[i]))
                    neighbours.Add (links[i].GetOtherNode(node));
        }

        Console.Error.Write ("{0} has {1} neighbours:" , node.index , neighbours.Count);
        for (int i = 0 ; i < neighbours.Count ; i++) {
            Console.Error.Write (" {0}", neighbours[i].index);
        }
        Console.Error.WriteLine(".");
        return neighbours.ToArray ();
    }
}


public class Link {
    public Node nodeA { get; private set; }
    public Node nodeB { get; private set; }

    public Link () {
        nodeA = new Node ();
        nodeB = new Node ();
    }

    public Link ( int x , int y ) {
        this.nodeA = new Node (x);
        this.nodeB = new Node (y);
    }

    public Link (Node nodeA, Node nodeB ) {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
    }

    public Node GetOtherNode (Node node ) {
        Node other = ( node == nodeA ) ? nodeB : nodeA;
        //Console.Error.WriteLine ("Get Other Node returning {0}, given {1}" , other.index , node.index);
        return other;
    }

    public bool Contains ( int index ) {
        return ( this.nodeA.index == index || this.nodeB.index == index );
    }

    public bool Contains ( Node node ) {
        return ( this.nodeA == node || this.nodeB == node );
    }

    public static bool operator == ( Link a , Link b ) {
        return ( a.Contains (b.nodeA) && a.Contains (b.nodeB) );
    }

    public static bool operator != ( Link a , Link b ) {
        return ( !a.Contains (b.nodeA) || !a.Contains (b.nodeB) );
    }

    public bool Equals ( Link other ) {
        return Equals (other , this);
    }

    public override bool Equals ( object obj ) {
        if (obj == null || GetType () != obj.GetType ()) {
            return false;
        }

        Link other = (Link) obj;

        return ( other.Contains (nodeA) && other.Contains (nodeB) );
    }

    public override int GetHashCode () {
        var id = nodeA * nodeA + nodeB * nodeB;
        return id.GetHashCode ();
    }

    public override string ToString () {
        string output = String.Format("{0} {1}", nodeA.index, nodeB.index);
        return output;
    }

}

public class Node {
    public int index;

    public int gatewayLinksCount;
    private int randomSeed;
    private Random r;

    public Node () {
        r = new Random ();
        this.index = -1;
        gatewayLinksCount = 0;
        randomSeed = r.Next (0 , 10000);
    }

    public Node (int index ) {
        r = new Random ();
        this.index = index;
        gatewayLinksCount = 0;
        randomSeed = r.Next (0 , 10000);
    }

    public static int operator * ( Node a , Node b ) {
        return ( a.index * b.index );
    }

    public static bool operator == ( Node a, Node b ) {
        return ( a.index == b.index );
    }

    public static bool operator != ( Node a, Node b ) {
        return ( a.index != b.index );
    }

    public bool Equals ( Node other ) {
        return Equals (other , this);
    }

    public override bool Equals ( object obj ) {
        if (obj == null || GetType () != obj.GetType ()) {
            return false;
        }

        Node other = (Node) obj;

        return ( other.index == index );
    }

    public override int GetHashCode () {
        var id = randomSeed;
        return ( id.GetHashCode () );
    }

    

}
