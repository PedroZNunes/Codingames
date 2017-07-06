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
class Solution {

    static private Grid grid;
    static private List<Lake> lakes = new List<Lake> ();

    static void Main ( string[] args ) {
        int W = int.Parse (Console.ReadLine ());
        int H = int.Parse (Console.ReadLine ());
        grid = new Grid (new Vector2 (W , H));

        Console.Error.WriteLine (grid.size.ToString());

        for (int y = 0 ; y < grid.size.y ; y++) {
            char[] terrainType = Console.ReadLine ().ToCharArray();

            //foreach (var temp in terrainType) {
            //    Console.Error.Write ("{0} ", temp);
            //}
            //Console.Error.WriteLine ("terrain length: {0}, n: {1}", terrainType.Length, y);

            for (int x = 0 ; x < grid.size.x ; x++) {
                bool isWater = terrainType[x].Equals ('O');
                grid.AddNode (x , y , isWater);
            }
        }

        int NodesToTestCount = int.Parse (Console.ReadLine ());
        Vector2[] pos = new Vector2[NodesToTestCount];

        for (int i = 0 ; i < NodesToTestCount ; i++) {
            string[] inputs = Console.ReadLine ().Split (' ');
            pos[i] = new Vector2 (int.Parse (inputs[0]) , int.Parse (inputs[1]));

            lakes.Add(FindLake (pos[i]));
        }

        int n = 0;
        for (int i = 0 ; i < NodesToTestCount ; i++) {
            if (n > 5) {
                n = 0;
                Console.Error.WriteLine ();
            }
            Console.Error.Write ("{0} s:{1} | " , pos[i].ToString () , lakes[i].Count);
            n++;
        }
        Console.Error.WriteLine ();

        for (int i = 0 ; i < NodesToTestCount ; i++) {
            Console.WriteLine (lakes[i].Count);
        }
    }


    static private Lake FindLake ( Vector2 pos ) {
        Node current = grid[pos];

        for (int i = 0 ; i < lakes.Count ; i++) {
            if (lakes[i].Contains (current)) {
                return lakes[i];
            }
        }

        if (!current.isWater)
            return new Lake ();

        List<Node> lakeArea = new List<Node> ();
        Queue<Node> frontier = new Queue<Node> ();

        frontier.Enqueue (current);

        while (frontier.Count > 0) {
            current = frontier.Dequeue ();
            Node[] next = grid.GetAllNeighbours (current);
            //Console.Error.WriteLine (next.Length);
            for (int i = 0 ; i < next.Length ; i++) {
                if (!lakeArea.Contains (next[i])) {
                    frontier.Enqueue (next[i]);
                    lakeArea.Add (next[i]);
                }
            }
        }
        return new Lake (lakeArea.ToArray ());
    }

    private class Lake {
        private Node[] nodes;

        public int Count { get { return nodes.Length; } }

        public Lake () {
            nodes = new Node[0];
        }

        public Lake (Node[] area ) {
            this.nodes = area;
        }

        public bool Contains ( Node core ) {
            for (int i = 0 ; i < nodes.Length ; i++) {
                if (nodes[i] == core) {
                    return true;
                }
            }
            return false;
        }

        public Node[] GetAllNodes () {
            return nodes;
        }

    }
}

public struct Node {
    public Vector2 pos;
    public bool isWater;
    //this can be int terrainType or an enum

    public Node ( int x , int y , bool isWater ) {
        pos.x = x;
        pos.y = y;
        this.isWater = isWater;
    }

    public Node ( Vector2 pos , bool isWater ) {
        this.pos = pos;
        this.isWater = isWater;
    }

    public bool IsAdjacentTo ( Node other ) {
        Vector2 distance = other.pos - this.pos;
        return ( Math.Abs (distance.x) + Math.Abs (distance.y) == 1 );
    }

    public static bool operator == ( Node a , Node b ) {
        return ( a.pos == b.pos && a.isWater == b.isWater );
    }

    public static bool operator != ( Node a , Node b ) {
        return ( a.pos != b.pos || a.isWater != b.isWater );
    }

    public bool Equals ( Node other ) {
        return Equals (other , this);
    }

    public override bool Equals ( object obj ) {
        if (obj == null || GetType () != obj.GetType ()) {
            return false;
        }

        Node other = (Node) obj;

        return (other.pos == pos && other.isWater == isWater);
    }

    public override int GetHashCode () {
        int water = (this.isWater) ? 10000 : 0;
        var id = water + ( 10000 * pos.x + pos.y );
        return id.GetHashCode ();
    }
}

public struct Grid {
    public Vector2 size;
    public Node[,] nodes { get; private set; }

    private const int MAX_NEIGHBOURS = 4;


    public Grid ( Vector2 size ) {
        this.size = size;
        nodes = new Node[size.x , size.y];
    }

    public Node this[Vector2 position] {
        get {
            Node a = GetNodeByPosition (position);
            return a;
        }
    }

    public void AddNode ( int x, int y, bool isWater ) {
        nodes[x , y] = new Node (x , y , isWater);
    }

    public Node[] GetAllNeighbours ( Node core ) {
        Vector2[] directions = new Vector2[]{
        new Vector2 (1 , 0) ,
        new Vector2 (0 , 1) ,
        new Vector2 (0 , -1) ,
        new Vector2 (-1 , 0)
        };
    
        //gets all nodes adjacent to the core
        List<Node> nodeList = new List<Node> ();
        
        for (int i = 0 ; i < MAX_NEIGHBOURS ; i++) {
            Vector2 pos = core.pos + directions[i];

            Node node = GetNodeByPosition (pos);
            if (core.pos == new Vector2 (36 , 0)) {
                Console.Error.WriteLine ("Core: {0}. NodePos: {1}. isWater? {2}" ,core.pos.ToString(), node.pos.ToString(), node.isWater );
            }
            if (node.isWater) {
                nodeList.Add (node);
            }
        }

        return nodeList.ToArray ();
    }

    public Node GetNodeByPosition ( Vector2 pos ) {
        if (pos.x < size.x && pos.x >= 0 && pos.y < size.y && pos.y >= 0) {
            return nodes[pos.x , pos.y];
        }
        return new Node ();
    }

}


public struct Vector2 {
    public int x;
    public int y;

    static public Vector2 zero { get { return new Vector2 (0 , 0); } }

    public Vector2 ( int x , int y ) {
        this.x = x;
        this.y = y;
    }

    public static Vector2 operator + ( Vector2 a , Vector2 b ) {
        return new Vector2 (a.x + b.x , a.y + b.y);
    }

    public static Vector2 operator - ( Vector2 a , Vector2 b ) {
        return new Vector2 (a.x - b.x , a.y - b.y);
    }

    public static bool operator == ( Vector2 a , Vector2 b ) {
        return ( a.x == b.x && a.y == b.y );
    }

    public static bool operator != ( Vector2 a , Vector2 b ) {
        return ( a.x != b.x || a.y != b.y );
    }

    public bool Equals ( Vector2 other ) {
        return Equals (other , this);
    }

    public override bool Equals ( object obj ) {
        if (obj == null || GetType () != obj.GetType ()) {
            return false;
        }

        Vector2 other = (Vector2) obj;

        return other.x == x && other.y == y;
    }

    public override int GetHashCode () {
        var id = 10 * x + y;
        return id.GetHashCode ();
    }

    public override string ToString () {
        return ( String.Format ("({0}, {1})" , x , y) );
    }

}
