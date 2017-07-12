using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


class Player
{
    private enum Actions { SPEED, JUMP, SLOW, UP, DOWN, WAIT };

    private const int ROAD_COUNT = 4;

    static private bool[,] grid;
    static private Vector2 gridSize;

    static private Motorbike[] bikes;
    static private int bikesAliveMin;

    static private Stack<Actions> path;

    static void Main(string[] args)
    {
        int playerCount = int.Parse(Console.ReadLine()); // the amount of motorbikes to control
        bikesAliveMin = int.Parse(Console.ReadLine()); // the minimum amount of motorbikes that must survive
        bikes = new Motorbike[playerCount];

        //fill grid
        string[] road = new string[ROAD_COUNT];
        for (int i = 0 ; i < ROAD_COUNT ; i++) {
            road[i] = Console.ReadLine ();
        }
        gridSize = new Vector2 (road[0].Length , ROAD_COUNT);
        Console.Error.WriteLine ("gridSize: {0}", gridSize.ToString());

        grid = new bool[road[0].Length, ROAD_COUNT];
        for (int y = 0 ; y < gridSize.y ; y++) {
            char[] node = road[y].ToCharArray ();
            for (int x = 0 ; x < gridSize.x ; x++) {
                grid[x , y] = ( node[x] == '.' );
            }
        }
        path = new Stack<Actions> ();

        // game loop
        while (true)
        {
            Actions outputAction;
            int currentSpeed = int.Parse(Console.ReadLine()); // the motorbikes' speed

            for (int i = 0; i < playerCount; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                bikes[i].pos = new Vector2 (int.Parse (inputs[0]) , int.Parse (inputs[1]));
                bikes[i].isAlive = ( inputs[2] == "1" );
            }
            
            if (path.Count == 0) {
                path = new Stack<Actions> ();
                if (SimulateTurn (bikes , currentSpeed , out outputAction)) {
                    path.Push (outputAction);
                }
            }

            outputAction = path.Pop ();

            // A single line containing one of 6 keywords: SPEED, SLOW, JUMP, WAIT, UP, DOWN.
            Console.WriteLine (Enum.GetName (typeof (Actions) , outputAction));
        }
    }

    static private bool SimulateTurn (Motorbike[] bikes, int speed, out Actions outAction ) {
        if (bikes[0].pos.x + speed >= gridSize.x - 1) {
            outAction = Actions.JUMP;
            return true;
        }

        foreach (Actions action in Enum.GetValues (typeof (Actions))) {
            if (action == Actions.UP) {
                //if any player is at the top lane, skip the action
                for (int i = 0 ; i < bikes.Length ; i++) {
                    if (bikes[i].pos.y == 0)
                        continue;
                }
            }
            else if (action == Actions.DOWN) {
                //if any player is at the bottom lane, skip the action
                for (int i = 0 ; i < bikes.Length ; i++) {
                    if (bikes[i].pos.y == gridSize.y-1)
                        continue;
                }
            }
            else if (action != Actions.SPEED && speed == 0) {
                    continue;
            }
            else if (action == Actions.SLOW) {
                if (speed <= 1)
                    continue;
            }

            int actionSpeed = speed;

            Motorbike[] aliveBikes = SimulateAction (bikes , actionSpeed , action);

            if (aliveBikes.Length >= bikesAliveMin) {
                //turn successful.
                Console.Error.WriteLine ("Turn Successful. alive {0}, speed {1}, X0: {2}, x1: {3}, Action {4}" , aliveBikes.Length , actionSpeed , bikes[0].pos.x, aliveBikes[0].pos.x , action);
                outAction = action;

                if (outAction == Actions.SLOW)
                    actionSpeed--;
                else if (outAction == Actions.SPEED)
                    actionSpeed++;

                Actions outputAction;
                if (SimulateTurn (aliveBikes , actionSpeed , out outputAction)) {
                    Console.Error.WriteLine ("Pushing action to Stack: {0}" , outputAction);
                    path.Push (outputAction);

                    return true;
                }
            }

            Console.Error.WriteLine ("Turn Failed. alive {0}, speed {1}, X {2}, Action {3}" , aliveBikes.Length , actionSpeed , bikes[0].pos.x , action);
        }

        outAction = Actions.WAIT;
        return false;
    }


    static private Motorbike[] SimulateAction ( Motorbike[] bikes, int speed, Actions action ) {

        List<Motorbike> bikesList = new List<Motorbike> (bikes);

        for (int j = 0 ; j < bikesList.Count ; j++) {
            Motorbike bike = bikesList[j];
            switch (action) {
                case Actions.WAIT:
                    for (int i = 1 ; i <= speed ; i++) {
                        if (!IsGround (bike.pos + Vector2.right * i)) {
                            bike.pos.x += i;
                            bike.isAlive = false;
                            break;
                        }
                    }
                    if (bike.isAlive)
                        bike.pos.x += speed;

                    break;

                case Actions.JUMP:
                    bike.isAlive = ( IsGround (bike.pos + Vector2.right * speed) );
                    bike.pos.x += speed;
                    //Console.Error.WriteLine ("Trying to jump. X0: {0}, X1{1}, speed {2}, isGround at x1 {3}. output: {4}", bike.pos.x, bike.pos.x + speed, )
                    break;

                case Actions.SPEED:
                    int increasedSpeed = speed + 1;
                    for (int i = 1 ; i <= increasedSpeed ; i++) {
                        if (!IsGround (bike.pos + Vector2.right * i)) {
                            bike.pos.x += i;
                            bike.isAlive = false;
                            break;
                        }
                    }
                    if (bike.isAlive)
                        bike.pos.x += increasedSpeed;
                    
                    break;

                case Actions.SLOW:
                    int decreasedSpeed = speed - 1;
                    for (int i = 1 ; i <= decreasedSpeed ; i++) {
                        if (!IsGround (bike.pos + Vector2.right * i)) {
                            bike.pos.x += i;
                            bike.isAlive = false;
                            break;
                        }
                    }
                    if (bike.isAlive)
                        bike.pos.x += decreasedSpeed;

                    break;

                case Actions.UP:
                    //check speed-1 in y
                    for (int i = 1 ; i < speed ; i++) {
                        if (!IsGround (bike.pos + Vector2.right * i)) {
                            bike.isAlive = false;
                            bike.pos += Vector2.right * i;
                            break;
                        }
                    }

                    //check speed in y-1
                    for (int i = 1 ; i <= speed ; i++) {
                        if (!IsGround (bike.pos + Vector2.right * i + Vector2.up)) {
                            bike.isAlive = false;
                            bike.pos += Vector2.right * i + Vector2.up;
                            break;
                        }
                    }

                    if (bike.isAlive)
                        bike.pos += Vector2.right * speed + Vector2.up;
                                        
                    break;

                case Actions.DOWN:
                    //check speed-1 in y
                    for (int i = 1 ; i < speed ; i++) {
                        if (!IsGround (bike.pos + Vector2.right * i)) {
                            bike.isAlive = false;
                            bike.pos += Vector2.right * i;
                            break;
                        }
                    }

                    //check speed in y-1
                    for (int i = 1 ; i <= speed ; i++) {
                        if (!IsGround (bike.pos + Vector2.right * i + Vector2.down)) {
                            bike.isAlive = false;
                            bike.pos += Vector2.right * i + Vector2.down;
                            break;
                        }
                    }

                    if (bike.isAlive)
                        bike.pos += Vector2.right * speed + Vector2.down;

                    break;

                default:
                    Console.Error.WriteLine ("Default Action triggered by pos {0}" , bike.pos.ToString ());
                    break;
            }
            bikesList[j] = bike;
        }

        for (int i = 0 ; i < bikesList.Count ; i++) {
            if (!bikesList[i].isAlive) {
                bikesList.RemoveAt (i);
                i--;
            }
        }

        return bikesList.ToArray ();
    }


    static private bool IsGround ( Vector2 pos ) {
        if (pos.x >= gridSize.x || pos.x < 0 || pos.y >= gridSize.y || pos.y < 0)
            return false;

        return ( grid[pos.x , pos.y] );
    }

    static private Motorbike[] GetAliveBikes (Motorbike[] bikes) {
        List<Motorbike> bikeList = new List<Motorbike> ();
        for (int i = 0 ; i < bikes.Length ; i++) {
            if (bikes[i].isAlive)
                bikeList.Add (bikes[i]);
        }
        return bikeList.ToArray();
    }



}

public struct Motorbike {
    public Vector2 pos;
    public bool isAlive;
}

//custom struct to represent a pair of integer coordinates.
public struct Vector2 {
    public int x;
    public int y;

    public int lengthInNodes {
        get {
            return Math.Abs (x) + Math.Abs (y);
        }
    }

    static public Vector2 up { get { return new Vector2 (0 , -1); } }
    static public Vector2 down { get { return new Vector2 (0 , 1); } }
    static public Vector2 left { get { return new Vector2 (-1 , 0); } }
    static public Vector2 right { get { return new Vector2 (1 , 0); } }
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

    public static Vector2 operator * ( Vector2 a , int b ) {
        return new Vector2 (a.x * b , a.y * b);
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

