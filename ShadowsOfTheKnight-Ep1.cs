using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/*
Game Input
The program must first read the initialization data from standard input. Then, within an infinite loop, read the device data from the standard input and provide to the standard output the next movement instruction.
Initialization input
Line 1 : 2 integers W H. The (W, H) couple represents the width and height of the building as a number of windows.
Line 2 : 1 integer N, which represents the number of jumps Batman can make before the bombs go off
Line 3 : 2 integers X0 Y0, representing the starting position of Batman.

Input for one game turn
The direction indicating where the bomb is.
Output for one game turn
A single line with 2 integers X Y separated by a space character. (X, Y) represents the location of the next window Batman should jump to. X represents the index along the horizontal axis, Y represents the index 
along the vertical axis. (0,0) is located in the top-left corner of the building.
Note: (0,0) is on top-left
*/
class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int columns = int.Parse(inputs[0]); // width of the building.
        int rows = int.Parse(inputs[1]); // height of the building.
        int jumpCountMax = int.Parse(Console.ReadLine()); // maximum number of turns before game over.
        inputs = Console.ReadLine().Split(' ');
        int posX0 = int.Parse(inputs[0]);
        int posY0 = int.Parse(inputs[1]);

        int posX = posX0;
        int posY = posY0;
        
        int lowX = 0, highX = columns -1;
        int lowY = 0, highY = rows - 1;
        // game loop
        while (true)
        {
            char[] bombDir = Console.ReadLine().ToArray(); // the direction of the bombs from batman's current location (U, UR, R, DR, D, DL, L or UL)
            //constrain the area each time it misses                        
            if (bombDir.Contains('U')){ //y--
                highY = posY-1;
            } else if (bombDir.Contains('D')){ //y++
                lowY = posY+1;
            }
            
            if (bombDir.Contains('L')){ //x--
                highX = posX-1;
            } else if (bombDir.Contains('R')){ //x++
                lowX = posX+1;
            }
            
            posX = lowX + (highX - lowX) / 2;
            posY = lowY + (highY - lowY) / 2;
            
            Console.WriteLine(posX + " " + posY);
        }
    }
}