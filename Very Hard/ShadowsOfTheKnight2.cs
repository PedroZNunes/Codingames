using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{

    static private int previousIndex;

    static private int bombIndex;
    static void Main(string[] args)
    {
        //string[] inputs;
        //inputs = Console.ReadLine().Split(' ');
        //int W = int.Parse(inputs[0]); // width of the building.
        int W = 10;
        //int H = int.Parse(inputs[1]); // height of the building.
        int H = 1;
        //int N = int.Parse(Console.ReadLine()); // maximum number of turns before game over.
        int N = 100;
        //inputs = Console.ReadLine().Split(' ');
        //int X0 = int.Parse(inputs[0]);
        //int Y0 = int.Parse(inputs[1]);
        int X0 = 2;
        int Y0 = 0;

        int[,] grid = new int[W , H];

        string output = "UNKNOWN";
        bombIndex = 8;

        int low = 0;
        int high = grid.GetUpperBound (0);

        // game loop
        while (true)
        {
            //string bombDir = Console.ReadLine(); // Current distance to the bomb compared to previous distance (COLDER, WARMER, SAME or UNKNOWN)        
            //Console.WriteLine("0 0");
        }
    }

    static private string RunBot (int index) {
        if (Math.Abs (index - bombIndex) > Math.Abs (previousIndex - bombIndex)) {
            return "WARMER";
        } else if (Math.Abs (index - bombIndex) < Math.Abs (previousIndex - bombIndex)) {
            return "COLDER";
        } else {
            return "SAME";
        }
    }
}