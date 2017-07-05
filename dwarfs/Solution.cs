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

    private static int size;
    private static int[,] influences;
    private static int longestBranch = 0;

    static void Main ( string[] args ) {
        size = int.Parse (Console.ReadLine ()); // the number of relationships of influence

        influences = new int[2 , size];

        for (int i = 0 ; i < size ; i++) {
            string[] inputs = Console.ReadLine ().Split (' ');
            int x = int.Parse (inputs[0]); // a relationship of influence between two people (x influences y)
            int y = int.Parse (inputs[1]);
            influences[0 , i] = x;
            influences[1 , i] = y;
            Console.Error.WriteLine ("{0} - {1}" , x , y);
        }

        for (int row = 0 ; row < size ; row++) {
            LookUp (influences[0 , row] , 1);
        }

        // The number of people involved in the longest succession of influences
        Console.WriteLine (longestBranch);
    }

    static private void LookUp (int personID, int n) {
        for (int row = 0 ; row < size; row++) {
            if (influences[0 , row] == personID) {
                int id = influences[1 , row];
                Console.Error.WriteLine ("{0} to {1}" , personID , id);
                LookUp (id , n + 1);
            }
        }
        Console.Error.WriteLine ("done with id {0}, n {1}" , personID , n);
        longestBranch = ( n > longestBranch ) ? n : longestBranch;
    } 

}