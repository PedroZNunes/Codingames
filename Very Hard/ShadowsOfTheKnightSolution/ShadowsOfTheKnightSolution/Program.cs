using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player {

    static private Vector2 previousPos;
    static private Vector2 currentPos;
    static private Vector2 nextJumpPos;

    static private int W = 0;
    static private int H = 0;

    static private bool isXSet = false;
    static private bool firstCheckY = true;

    static void Main ( string[] args ) {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        W = int.Parse(inputs[0]); // width of the building.
        H = int.Parse(inputs[1]); // height of the building.
        int N = int.Parse(Console.ReadLine()); // maximum number of turns before game over.
        
        inputs = Console.ReadLine().Split(' ');
        int X0 = int.Parse(inputs[0]);
        int Y0 = int.Parse(inputs[1]);

        int[,] grid = new int[W , H];

        Vector2 low = new Vector2 (0, 0);
        Vector2 high = new Vector2 (W-1 , H-1);

        currentPos = new Vector2 (X0 , Y0);
        previousPos = currentPos;
        nextJumpPos = currentPos;

        Console.Error.WriteLine ("Grid Size {0}." , new Vector2 (W , H));
        // game loop
        while (true) {

            string bombDir = Console.ReadLine(); // Current distance to the bomb compared to previous distance (COLDER, WARMER, SAME or UNKNOWN)

            Console.Error.WriteLine ("Starting turn. previousPos {0}, currentPos {1}, low {2}, high {3}, bombDirection {4}", previousPos, currentPos , low , high , bombDir);

            if (!isXSet) {
                Console.Error.WriteLine ("Working in X.");
                if (bombDir == "UNKNOWN") {
                    nextJumpPos.x = high.x - X0;
                    Console.Error.WriteLine ("Unknown. flipping x from {0} to {1}." , X0 , nextJumpPos.x);
                } 
                else { 
                    if (bombDir == "WARMER") {
                    if (currentPos.x > previousPos.x) {
                        low.x = currentPos.x - (int) Math.Floor ((double) ( currentPos.x - ( previousPos.x + 1 ) ) / 2f);
                    }
                    else {
                        high.x = currentPos.x + (int) Math.Floor ((double) ( previousPos.x - ( currentPos.x + 1 ) ) / 2f );
                    }
                }
                else if (bombDir == "COLDER") {
                    if (currentPos.x > previousPos.x) {
                        high.x = previousPos.x + (int) Math.Floor ((double) ( currentPos.x - ( previousPos.x + 1 ) ) / 2f);
                    }
                    else {
                        low.x = previousPos.x - (int) Math.Floor ((double) ( previousPos.x - ( currentPos.x + 1 ) ) / 2f);
                    }
                }
                else if (bombDir == "SAME") {
                    if (currentPos.x > previousPos.x) {
                        high.x = currentPos.x;
                        low.x = previousPos.x;
                    }
                    else {
                        high.x = previousPos.x;
                        low.x = currentPos.x;
                    }
                }
             
                    Console.Error.WriteLine ("Calculating NextJumpPos in X. low.x {0}, high.x {1}" , low.x , high.x);

                    nextJumpPos.x = low.x + ( high.x - low.x ) / 2;
                    if (nextJumpPos == currentPos) {
                        Console.Error.WriteLine ("jumping to the same window. {0} {1}" , nextJumpPos , currentPos);
                        if (nextJumpPos.x < high.x) {
                            nextJumpPos.x++;
                        }
                        else if (nextJumpPos.x > low.x) {
                            nextJumpPos.x--;
                        }
                    }
                    
                    if (low.x == high.x) {
                        isXSet = true;
                    }
                }
            }
            
            if (isXSet) {
                Console.Error.WriteLine ("Working in y.");
                if (firstCheckY) {
                    nextJumpPos.y = high.y - Y0;
                    Console.Error.WriteLine ("First check in Y. Flipping Y from {0} to {1}." , Y0 , nextJumpPos.y);
                    firstCheckY = false;
                } 
                else { 
                    if (bombDir == "WARMER") {
                        if (currentPos.y > previousPos.y) {
                            low.y = currentPos.y - (int) Math.Floor ((double) ( currentPos.y - ( previousPos.y + 1 ) ) / 2f);
                        }
                        else {
                            high.y = currentPos.y + (int) Math.Floor ((double) ( previousPos.y - ( currentPos.y + 1 ) ) / 2f);
                        }
                    }
                    else if (bombDir == "COLDER") {
                        if (currentPos.y > previousPos.y) {
                            high.y = previousPos.y + (int) Math.Floor ((double) ( currentPos.y - ( previousPos.y + 1 ) ) / 2f);
                        }
                        else {
                            low.y = previousPos.y - (int) Math.Floor ((double) ( previousPos.y - ( currentPos.y + 1 ) ) / 2f);
                        }
                    }
                    else if (bombDir == "SAME") {
                        if (currentPos.y > previousPos.y) {
                            high.y = currentPos.y;
                            low.y = previousPos.y;
                        }
                        else {
                            high.y = previousPos.y;
                            low.y = currentPos.y;
                        }
                    }

                    Console.Error.WriteLine ("Calculating NextJumpPos in Y. low.y {0}, high.y {1}" , low.y , high.y);

                    nextJumpPos.y = low.y + ( high.y - low.y ) / 2;
                    if (nextJumpPos == currentPos) {
                        Console.Error.WriteLine ("jumping to the same window. {0} {1}" , nextJumpPos , currentPos);
                        if (nextJumpPos.y < high.y) {
                            nextJumpPos.y++;
                        }
                        else if (nextJumpPos.y > low.y) {
                            nextJumpPos.y--;
                        }
                    }
                }
                  
            }

            previousPos = currentPos;
            currentPos = nextJumpPos;

            Console.Error.WriteLine ("Ending turn. previousPos {0}, currentPos {1}, low {2}, high {3}" , previousPos , currentPos , low , high);
            Console.Error.WriteLine ("Jumping to window {0}" , nextJumpPos);
            Console.WriteLine ("{0} {1}" , nextJumpPos.x , nextJumpPos.y);
        }
    }



//    static private string RunBot ( int index ) {
//        string output;
//        if (index == bombIndex) {
//            output = "DONE";
//            Console.Error.WriteLine ("Bomb Found. {0}", index);
//        }
//        else if (Math.Abs (index - bombIndex) < Math.Abs (previousIndex - bombIndex)) {
//            output = "WARMER";
//        }
//        else if (Math.Abs (index - bombIndex) > Math.Abs (previousIndex - bombIndex)) {
//            output =  "COLDER";
//        }
//        else {
//            output = "SAME";
//        }
        
//        return output;
//    }
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