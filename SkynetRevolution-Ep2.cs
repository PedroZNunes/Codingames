using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
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


        for (int i = 0; i < linkCount; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse(inputs[1]);

            //Console.Error.WriteLine ("input {0} {1}", N1, N2);

            Node nodeA = new Node (N1);
            Node nodeB = new Node (N2);

            bool containsNodeA = false;
            bool containsNodeB = false;
            for (int j = 0 ; j < nodeList.Count ; j++) {
                //Console.Error.WriteLine ("Checking if nodelist index {0} is {1} or {2}.", nodeList[j].index, nodeA.index, nodeB.index);
                if (nodeList[j].index == nodeA.index) {
                    nodeA = nodeList[j];
                    nodeList[j].gatewayLinksCount++;
                    containsNodeA = true;
                    //Console.Error.WriteLine ("node {0} connections improved to {1}.", nodeList[j].index, nodeList[j].gatewayLinksCount);
                }

                if (nodeList[j].index == nodeB.index) {
                    nodeB = nodeList[j];
                    nodeList[j].gatewayLinksCount++;
                    containsNodeB = true;
                    //Console.Error.WriteLine ("node {0} connections improved to {1}." , nodeList[j].index , nodeList[j].gatewayLinksCount);
                }
            }

            if (!containsNodeA)
                nodeList.Add (nodeA);

            if (!containsNodeB)
                nodeList.Add (nodeB);

            links[i] = new Link (nodeA, nodeB);
            //Console.Error.WriteLine (links[i].ToString ());
        }

        Console.Error.Write ("nodeList: ");
        for (int j = 0 ; j < nodeList.Count ; j++) {
            Console.Error.Write ("N{0}, C{1}.  " , nodeList[j].index , nodeList[j].gatewayLinksCount);
        }
        Console.Error.WriteLine ("");

        for (int i = 0; i < gatewayCount; i++)
        {
            gateways[i] = new Node (int.Parse(Console.ReadLine())); // the index of a gateway node
        }

        //get all links adjacent to the gateways, store and count them.
        gatewayLinks = new List<Link> ();
        for (int k = 0 ; k < gateways.Length ; k++) {
            for (int j = 0 ; j < linkCount ; j++) {

                if (links[j].Contains (gateways[k])) {
                    if (!gatewayLinks.Contains (links[j])) {
                        links[j].GetOtherNode (gateways[k]).gatewayLinksCount++;
                        gatewayLinks.Add (links[j]);
                    }
                }

            }
        }

        for (int i = 0 ; i < gatewayLinks.Count ; i++) {
            Console.Error.WriteLine ("Link {0}, node {1} count {2}" , gatewayLinks[i] , gatewayLinks[i].nodeA.index , gatewayLinks[i].nodeA.gatewayLinksCount);
            Console.Error.WriteLine ("Link {0}, node {1} count {2}" , gatewayLinks[i] , gatewayLinks[i].nodeB.index , gatewayLinks[i].nodeB.gatewayLinksCount);
        }

        for (int i = 0 ; i < nodeList.Count ; i++) {
            Console.Error.WriteLine ("{0} has {1} connections.", nodeList[i].index, nodeList[i].gatewayLinksCount);
        }

        severedLinks = new List<Link> ();

        // game loop
        while (true)
        {
            Node agentNode = new Node (int.Parse(Console.ReadLine())); // The index of the node on which the Skynet agent is positioned this turn
            Link linkToSevere = new Link();

            //if the agent node is present in the gatewaylinks list, just skip the rest.
            for (int i = 0 ; i < gatewayLinks.Count ; i++) {
                if (gatewayLinks[i].Contains (agentNode)) {
                    linkToSevere = gatewayLinks[i];
                    break;
                }
            }

            if (linkToSevere == new Link ()) {
                Console.Error.WriteLine ("Link to Severe is null");
                //now lets see which nodes can influence more edges on gateways simultaneously.
                Node nodeToSevere = new Node ();
                for (int i = 0 ; i < nodeCount ; i++) {
                    if (i == 0) {
                        nodeToSevere = nodeList[i];
                        continue;
                    }

                    //the higher the link count, higher the possibility of being take away.
                    if (nodeList[i].gatewayLinksCount > nodeToSevere.gatewayLinksCount) {
                        Console.Error.WriteLine ("Node {0} to be severed. Has {1} connections, while the previous best, {1} had {2} connections to gateways." , nodeList[i].index, nodeList[i].gatewayLinksCount , nodeToSevere.index, nodeToSevere.gatewayLinksCount);
                        nodeToSevere = nodeList[i];
                    }
                    else if (nodeList[i].gatewayLinksCount == nodeToSevere.gatewayLinksCount) {
                        Console.Error.WriteLine ("Nodes {0} and {1} have the same count of gateways: {2}" , nodeList[i].index , nodeToSevere.index , nodeToSevere.gatewayLinksCount);
                        //check which is closer to the enemy
                        if (DistanceBetween (agentNode , nodeList[i]) < DistanceBetween (agentNode , nodeToSevere)) {
                            Console.Error.WriteLine ("Node {0} to be severed. {1} turns from the agent. previous most dangerous node was {2} turns away.", nodeList[i].index , nodeList[i].gatewayLinksCount, nodeToSevere.gatewayLinksCount);
                            nodeToSevere = nodeList[i];
                        }
                    }
                }

                for (int i = 0 ; i < gatewayLinks.Count ; i++) {
                    if (gatewayLinks[i].Contains (nodeToSevere))
                        linkToSevere = gatewayLinks[i];
                }
            }

            severedLinks.Add (linkToSevere);
            Console.WriteLine(linkToSevere.ToString());
        }
    }

    static private int DistanceBetween ( Node a , Node b ) {

        //BFS from a to b
        Queue<Node> frontier = new Queue<Node> ();
        cameFrom = new Dictionary<Node , Node> ();
        bool[] hasVisited = new bool[nodeCount];

        Node current = a;
        frontier.Enqueue (current);
        hasVisited[current.index] = true;
        cameFrom[current] = new Node ();

        while (frontier.Count > 0) {
            current = frontier.Dequeue ();

            if (current == b) {
                Console.Error.WriteLine ("Target found on BFS.");
                break;
            }

            Node[] neighbours = GetNeighbours (current);
            for (int i = 0 ; i < neighbours.Length ; i++) {
                if (severedLinks.Contains (new Link ())) {
                    if (!hasVisited[neighbours[i].index]) {
                        frontier.Enqueue (neighbours[i]);
                        hasVisited[neighbours[i].index] = true;
                        cameFrom[neighbours[i]] = current;
                    }
                }
            }
        }

        //calculate distance
        int n = 0;
        current = a;
        while (current != b) {
            current = cameFrom[current];
            n++;
        }
        return n;
    }

    static private Node[] GetNeighbours ( Node a ) {
        List<Node> neighbours = new List<Node> ();

        for (int i = 0 ; i < linkCount ; i++) {
            if (links[i].Contains (a))
                neighbours.Add (links[i].GetOtherNode(a));
        }

        Console.Error.WriteLine ("{0} has {1} neighbours:" , a.index , neighbours.Count);
        for (int i = 0 ; i < neighbours.Count ; i++) {
            Console.Error.Write ("{0}, ", neighbours[i].index);
        }
        Console.Error.WriteLine("");
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
        Console.Error.WriteLine ("returning other {0}, given {1}" , other.index , node.index);
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
        gatewayLinksCount = 1;
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
