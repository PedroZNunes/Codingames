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

    static private bool[,] grid;
    static private List<Lake> lakes = new List<Lake> ();
    static private Vector2 gridSize;


    static void Main ( string[] args ) {
        gridSize.x = int.Parse (Console.ReadLine ());
        gridSize.y = int.Parse (Console.ReadLine ());

        grid = new bool[gridSize.x , gridSize.y];

        for (int y = 0 ; y < gridSize.y ; y++) {
            char[] terrainType = new char[gridSize.x];
            terrainType = Console.ReadLine ().ToCharArray ();
            for (int x = 0 ; x < gridSize.x ; x++) {
                bool isWater = terrainType[x].Equals ('O');
                grid[x,y] = isWater;
            }
        }

        int NodesToTestCount = int.Parse (Console.ReadLine ());
        Vector2[] pos = new Vector2[NodesToTestCount];

        for (int i = 0 ; i < NodesToTestCount ; i++) {
            string[] inputs = Console.ReadLine ().Split (' ');
            pos[i] = new Vector2 (int.Parse (inputs[0]) , int.Parse (inputs[1]));
            lakes.Add (FindLake (pos[i]));
        }

        ///int n = 0;
        ///for (int i = 0 ; i < NodesToTestCount ; i++) {
        ///    if (n > 5) {
        ///        n = 0;
        ///        Console.Error.WriteLine ();
        ///    }
        ///    Console.Error.Write ("{0} s:{1} | " , pos[i].ToString () , lakes[i].Count);
        ///    n++;
        ///}
        ///Console.Error.WriteLine ();

        for (int i = 0 ; i < NodesToTestCount ; i++) {
            Console.WriteLine (lakes[i].Count);
        }

        Console.ReadKey ();
        ///int[] expectedOut = new int[] {42,38,3,5,13,38,16,42,8,40,13,39,6,8,42,13,38,4,2,28,31,16,42,38,38,20,28,10,39,42,40,
        ///    11,20,40,20,20,7,10,6,16,34,42,16,2,16,13,31,39,13,40,20,38,28,39,20,34,28,40,9,20,20,34,6,6,4,40,6,9,39,3,39,16,
        ///    38,6,31,10,3,16,11,4,40,10,16,16,13,28,1,12,40,11,10,16,42,20,2,42,11,20,16,4
        ///};
        ///
        ///for (int i = 0 ; i < NodesToTestCount ; i++) {
        ///    int count = lakes[i].Count;
        ///    if (count != expectedOut[i]) {
        ///        Console.Error.WriteLine ("{0} expected, {1} found, index {2}", expectedOut[i], count, i);
        ///        Console.ReadKey ();
        ///    }
        ///}

    }

    static private Vector2[] GetAllNeighbours ( Vector2 corePos ) {
        Vector2[] directions = new Vector2[]{
        new Vector2 (1 , 0) ,
        new Vector2 (0 , 1) ,
        new Vector2 (0 , -1) ,
        new Vector2 (-1 , 0)
        };

        //gets all nodes adjacent to the core
        List<Vector2> nodes = new List<Vector2> ();

        for (int i = 0 ; i < 4 ; i++) {
            Vector2 pos = corePos + directions[i];

            if (pos.x < gridSize.x && pos.x >= 0 && pos.y < gridSize.y && pos.y >= 0) {
                if (grid[pos.x , pos.y] == true) {
                    nodes.Add (pos);
                }
            }
        }

        return nodes.ToArray();
    }


    static private Lake FindLake ( Vector2 pos ) {
        Vector2 current = pos;

        for (int i = 0 ; i < lakes.Count ; i++) {
            if (lakes[i].Contains (current)) {
                return lakes[i];
            }
        }

        if (!grid[current.x, current.y])
            return new Lake (new Vector2[0]);

        bool[,] visited = new bool[gridSize.x, gridSize.y];
        List<Vector2> lakeArea = new List<Vector2> ();
        Queue<Vector2> frontier = new Queue<Vector2> ();

        frontier.Enqueue (current);
        lakeArea.Add (current);
        visited[current.x, current.y] = true;

        while (frontier.Count > 0) {
            current = frontier.Dequeue ();
            Vector2[] next = GetAllNeighbours (current);
            for (int i = 0 ; i < next.Length ; i++) {
                if (!visited[next[i].x, next[i].y]) {
                    frontier.Enqueue (next[i]);
                    visited[next[i].x, next[i].y] = true;
                    lakeArea.Add (current);
                }
            }
        }

        return new Lake (lakeArea.ToArray ());
    }

    private struct Lake {
        private Vector2[] positions;

        public int Count { get { return positions.Length; } }

        public Lake (Vector2[] area ) {
            this.positions = area;
        }

        public bool Contains ( Vector2 pos) {
            for (int i = 0 ; i < positions.Length ; i++) {
                if (positions[i] == pos) {
                    return true;
                }
            }
            return false;
        }

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
