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
class Player {
    static void Main ( string[] args ) {
        RoomType[] types = new RoomType[14];
        types[0] = new RoomType ();
        types[1] = new RoomType (true , true , true , true);
        types[2] = new RoomType (false , true , false , true);
        types[3] = new RoomType (true , false , true , false);
        types[4] = new RoomType (true , true , true , true , false);
        types[5] = new RoomType (true , true , true , true , true);
        types[6] = new RoomType (true , true , false , true);
        types[7] = new RoomType (true , true , true , false);
        types[8] = new RoomType (false , true , true , true);
        types[9] = new RoomType (true , false , true , true);
        types[10] = new RoomType (true , false , false , true , false);
        types[11] = new RoomType (true , true , false , false , true);
        types[12] = new RoomType (false , true , true , false , false);
        types[13] = new RoomType (false , false , true , true , true);

        string[] inputs;
        inputs = Console.ReadLine ().Split (' ');
        int COLUMNS = int.Parse (inputs[0]); // number of columns.
        int ROWS = int.Parse (inputs[1]); // number of rows.
        int[,] grid = new int[COLUMNS , ROWS];
        for (int i = 0 ; i < ROWS ; i++) {
            string[] LINE = Console.ReadLine ().Split (' '); // represents a line in the grid and contains W integers. Each integer represents one room of a given type.
            Console.Error.WriteLine ("{0} lines and {1} columns" , LINE.Length , COLUMNS);
            for (int j = 0 ; j < COLUMNS ; j++) {
                grid[j , i] = int.Parse (LINE[j]);
            }
        }
        int ExitX = int.Parse (Console.ReadLine ()); // the coordinate along the X axis of the exit (not useful for this first mission, but must be read).

        // game loop
        while (true) {
            inputs = Console.ReadLine ().Split (' ');
            int posX = int.Parse (inputs[0]);
            int posY = int.Parse (inputs[1]);
            string POS = inputs[2];

            Vector2 outPos = new Vector2 (posX , posY);

            int index = grid[posX , posY];
            RoomType type = types[index];

            if (type.divergency == new bool? ()) {
                if (POS == "TOP") {
                    outPos += Vector2.bottom;
                }
                else if (POS == "LEFT") {
                    if (type.hasBottom) {
                        outPos += Vector2.bottom;
                    }
                    else {
                        outPos += Vector2.right;
                    }
                }
                else {
                    if (type.hasBottom) {
                        outPos += Vector2.bottom;
                    }
                    else {
                        outPos += Vector2.left;
                    }
                }
            }
            else if (type.divergency.GetValueOrDefault () == false) {
                if (POS == "TOP") {
                    outPos += Vector2.left;
                }
                else {
                    outPos += Vector2.bottom;
                }
            }
            else if (type.divergency.GetValueOrDefault () == true) {
                if (POS == "TOP") {
                    outPos += Vector2.right;
                }
                else {
                    outPos += Vector2.bottom;
                }
            }

            else {
                Console.Error.WriteLine ("divergency outside bounds.");
            }

            // One line containing the X Y coordinates of the room in which you believe Indy will be on the next turn.
            Console.WriteLine ("{0} {1}" , outPos.x , outPos.y);
        }
    }

}

struct Vector2 {
    public int x;
    public int y;

    public static Vector2 right {
        get {
            return new Vector2 (1 , 0);
        }
    }

    public static Vector2 left {
        get {
            return new Vector2 (-1 , 0);
        }
    }

    public static Vector2 bottom {
        get {
            return new Vector2 (0 , 1);
        }
    }

    public Vector2 ( int x , int y ) {
        this.x = x;
        this.y = y;
    }

    public static Vector2 operator + ( Vector2 a , Vector2 b ) {
        return new Vector2 (a.x + b.x , a.y + b.y);
    }
}

class RoomType {
    public bool hasTop = false, hasLeft = false, hasRight = false, hasBottom = false;
    public bool? divergency;
    public bool? Divergency {
        get {
            return divergency;
        }
        private set {
            divergency = value;
        }
    }

    public RoomType ( bool top = false , bool right = false , bool bottom = false , bool left = false , bool? divergency = new bool? () ) {
        hasTop = top;
        hasLeft = left;
        hasRight = right;
        hasBottom = bottom;
        this.Divergency = divergency;
    }

}


