using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player {

    static private int previousIndex;
    static private int index;
    static private int bombIndex;

    static void Main ( string[] args ) {
        //string[] inputs;
        //inputs = Console.ReadLine().Split(' ');
        //int W = int.Parse(inputs[0]); // width of the building.
        int W = 100000;
        //int H = int.Parse(inputs[1]); // height of the building.
        int H = 1;
        //int N = int.Parse(Console.ReadLine()); // maximum number of turns before game over.
        int N = 100;
        //inputs = Console.ReadLine().Split(' ');
        //int X0 = int.Parse(inputs[0]);
        //int Y0 = int.Parse(inputs[1]);
        int X0 = 75;
        int Y0 = 0;

        int[,] grid = new int[W , H];

        string output = "UNKNOWN";
        bombIndex = 511;

        int low = grid.GetLowerBound (0);
        int high = grid.GetUpperBound (0);

        index = X0;



        // game loop
        while (true) {

            //string bombDir = Console.ReadLine(); // Current distance to the bomb compared to previous distance (COLDER, WARMER, SAME or UNKNOWN)
            Console.WriteLine ("starting turn. index {0}, previsousIndex {1}", index, previousIndex);

            if (output == "WARMER") {
                if (index > previousIndex) {
                    low = previousIndex + (int) Math.Ceiling ((double) ( ( ( index + 1 ) - previousIndex ) / 2f ));
                }
                else {
                    high = previousIndex - (int) Math.Ceiling ((double) ( ( ( previousIndex + 1 ) - index ) / 2f ));
                }
            }
            else if (output== "COLDER") {
                if (index > previousIndex) {
                    high = index - (int) Math.Ceiling ((double) ( ( ( index + 1 ) - previousIndex ) / 2 ));
                }
                else {
                    low = index + (int) Math.Ceiling ((double) ( ( ( previousIndex + 1 ) - index ) / 2 ));
                }
            }
            else if (output == "SAME") {
                if (index > previousIndex) {
                    high = index;
                    low = previousIndex;
                }
                else {
                    high = previousIndex;
                    low = index;
                }
            }
            else {
                Console.Error.WriteLine ("Unknown");
            }

            if (low == index)
                low++;
            if (high == index)
                high--;
            //Console.WriteLine("0 0");

            previousIndex = index;
            index = low + ( high - low ) / 2;
            if (index == previousIndex) {
                index++;
            }

            Console.Error.WriteLine ("Ending turn. Low {0}, High {1}, PrevIndex {2}, Index {3}, Bomb {4}" , low, high, previousIndex, index, bombIndex);
            output = RunBot (index);
            Console.WriteLine (output);

            if (output == "DONE")
                break;
            Console.ReadKey ();
        }
        Console.ReadKey ();
    }



    static private string RunBot ( int index ) {
        string output;
        if (index == bombIndex) {
            output = "DONE";
            Console.Error.WriteLine ("Bomb Found. {0}", index);
        }
        else if (Math.Abs (index - bombIndex) < Math.Abs (previousIndex - bombIndex)) {
            output = "WARMER";
        }
        else if (Math.Abs (index - bombIndex) > Math.Abs (previousIndex - bombIndex)) {
            output =  "COLDER";
        }
        else {
            output = "SAME";
        }
        
        return output;
    }
}