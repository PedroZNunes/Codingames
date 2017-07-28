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

    static private Vector2 low;
    static private Vector2 high;

    static private int W = 0; //width
    static private int H = 0; //height

    static private bool isXSet = false; //boolean to check if x is done
    static private bool firstCheckY = true; //boolean for the first check in y

    static void Main ( string[] args ) {

        Vector2 startingPos = new Vector2 ();

        Initialization (out startingPos);
        // game loop
        while (true) {

            string bombDir = Console.ReadLine(); // Current distance to the bomb compared to previous distance (COLDER, WARMER, SAME or UNKNOWN)

            Console.Error.WriteLine ("Starting turn. previousPos {0}, currentPos {1}, low {2}, high {3}, bombDirection {4}", previousPos, currentPos , low , high , bombDir);
            if (low.x == high.x && !isXSet) //X is done when there's only 1 column left to search
                isXSet = true;

            //if X is not done yet, search in X
            if (!isXSet) {
                Console.Error.WriteLine ("Working in X.");

                //just mirror X in the first turn to get rid of half the map
                if (bombDir == "UNKNOWN") {
                    nextJumpPos.x = high.x - startingPos.x;
                    Console.Error.WriteLine ("Unknown. Flipping X from {0} to {1}." , startingPos.x , nextJumpPos.x);
                } 
                else {
                    //Set the high and low bounds for the x axis
                    SetAxisBounds (bombDir, ref low.x , ref high.x, currentPos.x, previousPos.x);
                    nextJumpPos.x = SetupNextJump (bombDir, 0);
                }
            }

            //if x is done, start searching in Y axis
            if (isXSet) {
                Console.Error.WriteLine ("Working in Y.");
                if (firstCheckY) {
                    nextJumpPos.y = high.y - startingPos.y;
                    Console.Error.WriteLine ("First check in Y. Flipping Y from {0} to {1}." , startingPos.y , nextJumpPos.y);
                    firstCheckY = false;
                }
                else {
                    //set the bounds (high and low) and then setup the next jump
                    SetAxisBounds (bombDir , ref low.y , ref high.y , currentPos.y , previousPos.y);
                    nextJumpPos.y = SetupNextJump (bombDir , 1);
                }
            }

            previousPos = currentPos;
            currentPos = nextJumpPos;

            Console.Error.WriteLine ("Ending turn. previousPos {0}, currentPos {1}, low {2}, high {3}" , previousPos , currentPos , low , high);
            Console.Error.WriteLine ("Jumping to window {0}" , nextJumpPos);
            Console.WriteLine ("{0} {1}" , nextJumpPos.x , nextJumpPos.y);
        }
    }

    /// <summary>
    /// Initialize variables
    /// </summary>
    static private void Initialization (out Vector2 startingPos) {
        string[] inputs;
        inputs = Console.ReadLine ().Split (' ');
        W = int.Parse (inputs[0]); // width of the building.
        H = int.Parse (inputs[1]); // height of the building.
        int N = int.Parse (Console.ReadLine ()); // maximum number of turns before game over.

        inputs = Console.ReadLine ().Split (' ');
        startingPos.x = int.Parse (inputs[0]); //starting pos y
        startingPos.y = int.Parse (inputs[1]); //starting pos x

        int[,] grid = new int[W , H]; //the grid

        low = new Vector2 (0 , 0); //the lower bounds of the search area
        high = new Vector2 (W - 1 , H - 1); //the higher bounds of the search area

        currentPos = startingPos;
        previousPos = currentPos;
        nextJumpPos = currentPos;

        if (W <= 1) {
            isXSet = true;
        }

        Console.Error.WriteLine ("Grid Size {0}." , new Vector2 (W , H));
    }

    /// <summary>
    /// Sets up the lower and upper bounds according to the reply given from the bot.
    /// </summary>
    /// <param name="bombDir">bomb direction (warmer, colder, etc)</param>
    /// <param name="low">the low bound for the axis </param>
    /// <param name="high">the upper bound for the axis </param>
    /// <param name="currentPos"> batman's current position on the axis </param>
    /// <param name="previousPos"> batman's last position </param>
    static private void SetAxisBounds (string bombDir, ref int low, ref int high, int currentPos, int previousPos ) {
        int tempPosX = 0;

        if (bombDir == "WARMER") {
            if (currentPos > previousPos) {
                tempPosX = currentPos - (int) Math.Floor ((double) ( currentPos - ( previousPos + 1 ) ) / 2f);
                if (tempPosX > low) {
                    low = tempPosX;
                }
            }
            else {
                tempPosX = currentPos + (int) Math.Floor ((double) ( previousPos - ( currentPos + 1 ) ) / 2f);
                if (tempPosX < high) {
                    high = tempPosX;
                }
            }

        }
        else if (bombDir == "COLDER") {
            if (currentPos > previousPos) {
                tempPosX = previousPos + (int) Math.Floor ((double) ( currentPos - ( previousPos + 1 ) ) / 2f);
                if (tempPosX < high) {
                    high = tempPosX;
                }
            }
            else {
                tempPosX = previousPos - (int) Math.Floor ((double) ( previousPos - ( currentPos + 1 ) ) / 2f);
                if (tempPosX > low) {
                    low = tempPosX;
                }
            }
        }
        else if (bombDir == "SAME") {
            if (currentPos > previousPos) {
                high = currentPos;
                low = previousPos;
            }
            else {
                high = previousPos;
                low = currentPos;
            }
        }
    }

    /// <summary>
    /// stes up the next jump in the desired axis
    /// </summary>
    /// <param name="bombDir"> bomb direction </param>
    /// <param name="axisIndex"> 0 = x; 1 = y </param>
    /// <returns> the next jump position for the axis </returns>
    static private int SetupNextJump ( string bombDir , int axisIndex ) {
        if (axisIndex > 1)
            Console.Error.WriteLine ("Invalid axis index in SetupNextJump function");

        int nextJumpPos = 0;
        int maxIndex = ( axisIndex == 0 ) ? W - 1 : H - 1;

        int currentPosition = ( axisIndex == 0 ) ? currentPos.x : currentPos.y;
        int previousPosition = ( axisIndex == 0 ) ? previousPos.x : previousPos.y;
        int lowerBound = ( axisIndex == 0 ) ? low.x : low.y;
        int upperBound = ( axisIndex == 0 ) ? high.x : high.y;

        int mid = lowerBound + ( upperBound - lowerBound ) / 2;
        Console.Error.WriteLine ("Calculating NextJumpPos. low {0}, high {1}" , lowerBound , upperBound);

        //if is the same distance, the bomb should be in the column right in between.
        if (bombDir == "SAME") {
            nextJumpPos = mid;
            if (axisIndex == 0)
                isXSet = true;
        }
        //if it is not, then mirror the position around the center of the area
        else {
            int targetPos = 2 * mid - currentPosition;
            int distance = ( targetPos - currentPosition );
            //if on the edge, walk less. this makes it easier to find nodes near the edges, which usually is pretty expensive on turns
            if (currentPosition == 0 || currentPosition == maxIndex)
                distance /= 2;
            nextJumpPos = currentPosition + distance;
        }

        //if the jump blew the max index for the axis, round it back to the nearest point inside the search area
        if (nextJumpPos > maxIndex) {
            Console.Error.WriteLine ("Invalid jump. tried {0}, rounded to {1}" , nextJumpPos , upperBound);
            nextJumpPos = upperBound;
        }
        else if (nextJumpPos < 0) {
            Console.Error.WriteLine ("Invalid jump. tried {0}, rounded to {1}" , nextJumpPos , lowerBound);
            nextJumpPos = lowerBound;
        }

        //work around for when the space is so tight that batman would try jumping on the same window twice
        if (nextJumpPos == currentPosition) {
            Console.Error.WriteLine ("jumping to the same window. {0} {1}" , nextJumpPos , currentPosition);
            if (nextJumpPos < upperBound) {
                nextJumpPos++;
            }
            else if (nextJumpPos > lowerBound) {
                nextJumpPos--;
            }
        }

        //if the area contains only 1 column, that should be it
        if (lowerBound == upperBound) {
            nextJumpPos = lowerBound;
        }

        return nextJumpPos;
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