using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


class Player {

    static private char[,] grid;
    static private Vector2 gridSize;
    static private Vector2 objectivePos = Vector2.invalidPos;
    static private Vector2 playerPos;
    static private Vector2 basePos;
    static private bool isRetreating; // tag if the player is going back to the base

    static private int alarmCountdown; //turns between getting to the control room and going back to base

    static void Main ( string[] args ) {
        string[] inputs;
        inputs = Console.ReadLine ().Split (' ');
        gridSize.y = int.Parse (inputs[0]); // number of rows.
        gridSize.x = int.Parse (inputs[1]); // number of columns.
        grid = new char[gridSize.x , gridSize.y];
        Console.Error.WriteLine ("grid size: {0}" , gridSize.ToString ());

        alarmCountdown = int.Parse (inputs[2]); // number of rounds between the time the alarm countdown is activated and the time the alarm goes off.
        isRetreating = false;
        // game loop
        while (true) {
            inputs = Console.ReadLine ().Split (' ');
            playerPos = new Vector2 (int.Parse (inputs[1]) , int.Parse (inputs[0])); // column where Kirk is located.
            //fill grid
            for (int row = 0 ; row < gridSize.y ; row++) {
                char[] cells = Console.ReadLine ().ToCharArray (); // characters in '#.TC?' (i.e. one line of the ASCII maze).
                for (int column = 0 ; column < gridSize.x ; column++) {
                    if (cells[column] == 'T')
                        basePos = new Vector2 (column , row);
                    else if (cells[column] == 'C')
                        objectivePos = new Vector2 (column , row);

                    grid[column , row] = cells[column];
                }
            }

            Vector2 targetPos = new Vector2 ();
            
            if (objectivePos == Vector2.invalidPos) //no objective
                targetPos = Explore ();
            else if (isRetreating) //retreating to base
                targetPos = MoveTowards (basePos);
            else  //objective found
                targetPos = MoveTowards (objectivePos);
 

            Console.WriteLine (MoveTo (targetPos)); // Kirk's next move (UP DOWN LEFT or RIGHT).
        }

    }

    //get all adjacent nodes to corePos
    static private Vector2[] GetAllNeighbours ( Vector2 corePos ) {
        Vector2[] directions = new Vector2[]{
        new Vector2 (-1 , 0) ,
        new Vector2 (0 , -1) ,
        new Vector2 (0 , 1) ,
        new Vector2 (1 , 0)
        };

        //gets all nodes adjacent to the core
        List<Vector2> nodes = new List<Vector2> ();

        for (int i = 0 ; i < 4 ; i++) {
            Vector2 pos = corePos + directions[i];

            if (pos.x < gridSize.x && pos.x >= 0 && pos.y < gridSize.y && pos.y >= 0) {
                if (grid[pos.x , pos.y] != '#') {
                    nodes.Add (pos);
                }
            }
        }

        return nodes.ToArray ();
    }


    static private string MoveTo ( Vector2 targetPos ) {
        string output;

        Vector2 dir = targetPos - playerPos;
        if (dir.x > 0)
            output = "RIGHT";
        else if (dir.x < 0)
            output = "LEFT";
        else if (dir.y > 0)
            output = "DOWN";
        else
            output = "UP";

        return output;
    }

    static private Vector2 Explore () {
        return MoveTowards (Vector2.invalidPos);
    }

    static private Vector2 MoveTowards ( Vector2 target ) {

        bool[,] visited = new bool[gridSize.x , gridSize.y];
        List<Vector2> AccessibleUnknown = new List<Vector2> ();
        Dictionary<Vector2 , Vector2> cameFrom = new Dictionary<Vector2 , Vector2> ();
        Queue<Vector2> frontier = new Queue<Vector2> ();

        Console.Error.WriteLine ("BFS starting at {0}. Target {1}" , playerPos.ToString(), target.ToString());
        Vector2 current = playerPos;
        frontier.Enqueue (current);
        cameFrom[current] = Vector2.invalidPos;
        visited[current.x , current.y] = true;

        bool pathFound = false;

        while (frontier.Count > 0) {
            current = frontier.Dequeue ();
            //Console.Error.WriteLine ("BFS current {0} - {1}", current.ToString(), grid[current.x, current.y]);

            if (grid[current.x , current.y] == '?') {
                AccessibleUnknown.Add (current);
                //Console.Error.WriteLine ("unknown added: {0}" , current.ToString ());
                continue;
            }
            //if the target is found, we check a bool and skip it altogether so that no node comes from the objective.
            //this is so that player can explore after finding the portal (in case the path found so far is too long) without going through the portal by mistake
            if (current == target) {
                pathFound = true;
                continue;
            }
            //gets all neighbours and throw them into the queue
            Vector2[] neighbours = GetAllNeighbours (current);
            for (int i = 0 ; i < neighbours.Length ; i++) {
                if (!visited[neighbours[i].x , neighbours[i].y]) {
                    if (grid[neighbours[i].x , neighbours[i].y] != '#' ) {
                        frontier.Enqueue (neighbours[i]);
                        visited[neighbours[i].x , neighbours[i].y] = true;
                        cameFrom[neighbours[i]] = current;
                    }
                }
            }
        }

        //where do I go
        //to the nearest unknown cell if theres no target set
        if (target == Vector2.invalidPos) {
            Vector2 nearestUnknown = Vector2.invalidPos;
            int shortestDistance = int.MaxValue;

            for (int i = 0 ; i < AccessibleUnknown.Count ; i++) {
                Vector2 temp = AccessibleUnknown[i];
                int distance = 0;

                while (temp != playerPos) {
                    temp = cameFrom[temp];
                    distance++;
                }

                Console.Error.WriteLine ("distance between {0} and {1} is {2}." , playerPos.ToString () , AccessibleUnknown[i].ToString () , distance);
                if (distance <= shortestDistance) {
                    nearestUnknown = AccessibleUnknown[i];
                    shortestDistance = distance;
                }
            }

            target = nearestUnknown;
            Console.Error.WriteLine ("No target found yet. From {0} to {1} - distance {2}", playerPos.ToString(), target.ToString(), shortestDistance);
        }
        //to the nearest unknown cell if theres no pathing to the target
        else if (!pathFound) {

            //calculate distance between unknowns and the target and move towards the closest one.
            Vector2 nearestUnknown = Vector2.invalidPos;
            int shortestDistance = int.MaxValue;

            for (int i = 0 ; i < AccessibleUnknown.Count ; i++) {
                int distance = ( target - AccessibleUnknown[i] ).lengthInNodes;
                if (distance < shortestDistance) {
                    nearestUnknown = AccessibleUnknown[i];
                    shortestDistance = distance;
                }
            }

            Console.Error.WriteLine ("Target found, but not accessible {0}. Going for the NUC to the target at {1}" , target.ToString (), nearestUnknown);
            target = nearestUnknown;
        }
        //to the target is retreating or if target not too far away.
        else {
            Console.Error.WriteLine ("Target found. {0}" , target.ToString ());
            if (!isRetreating) {
                
                int distance = 0;

                //if the known path is enough
                if (CanReturnSafely ()) {
                    target = objectivePos;
                }
                else {
                    //found the target, however the path back to the base is too long. must find a shorter path.
                    //calculate distance between unknowns and the target and move towards the closest one.
                    Vector2 nearestUnknown = Vector2.invalidPos;
                    int shortestDistance = int.MaxValue;

                    for (int i = 0 ; i < AccessibleUnknown.Count ; i++) {
                        distance = ( target - AccessibleUnknown[i] ).lengthInNodes;
                        if (distance < shortestDistance) {
                            nearestUnknown = AccessibleUnknown[i];
                            shortestDistance = distance;
                        }
                    }

                    Console.Error.WriteLine ("Target found, but not accessible {0}. Going for the NUC to the target at {1}" , target.ToString () , nearestUnknown);
                    target = nearestUnknown;
                }

            }
            else {
                target = basePos;
            }
            
        }

        Vector2 previous = new Vector2 ();
        if (playerPos == cameFrom[target]) {
            isRetreating = true;
            Console.Error.WriteLine ("Retreat! {0}" , playerPos);
        }

        while (target != playerPos) {
            previous = target;
            target = cameFrom[previous];
            Console.Error.WriteLine ("{0} came from {1}.", previous, target);
        }
        
        //the target is a cell adjacent to the player, the first step
        return previous;
    }

    //checks if the player can return from the control room in time
    static private bool CanReturnSafely () {
        //BFS from base to portal.
        bool[,]visited = new bool[gridSize.x , gridSize.y];
        Dictionary<Vector2 , Vector2> cameFrom = new Dictionary<Vector2 , Vector2> ();
        Queue<Vector2> frontier = new Queue<Vector2> ();

        Vector2 current = basePos;
        frontier.Enqueue (current);
        cameFrom[current] = Vector2.invalidPos;
        visited[current.x , current.y] = true;

        while (frontier.Count > 0) {
            current = frontier.Dequeue ();

            if (grid[current.x , current.y] == '?')
                continue;

            //this will always be triggered, because we know we can see the objective at this point.
            if (current == objectivePos)
                break;

            Vector2[] neighbours = GetAllNeighbours (current);
            for (int i = 0 ; i < neighbours.Length ; i++) {
                if (!visited[neighbours[i].x , neighbours[i].y]) {
                    if (grid[neighbours[i].x , neighbours[i].y] != '#') {
                        frontier.Enqueue (neighbours[i]);
                        visited[neighbours[i].x , neighbours[i].y] = true;
                        cameFrom[neighbours[i]] = current;
                    }
                }
            }
        }
        int distance = 0;
        while (current != basePos) {
            current = cameFrom[current];
            distance++;
        }

        Console.Error.WriteLine ("distance between {0} and {1} is {2}. AlarmCountdown is {3}" , basePos , current.ToString () , distance , alarmCountdown);

        return (distance <= alarmCountdown);
    }


}

//custom struct to represent a pair of integer coordinates.
public struct Vector2 {
    public int x;
    public int y;

    public int lengthInNodes {
        get {
            return Math.Abs(x) + Math.Abs(y);
        }
    }

    static public Vector2 zero { get { return new Vector2 (0 , 0); } }
    static public Vector2 invalidPos { get { return new Vector2 (-1 , -1); } }

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

