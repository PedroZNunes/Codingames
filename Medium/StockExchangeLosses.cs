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
    static void Main ( string[] args ) {
        int n = int.Parse (Console.ReadLine ());
        string[] inputs = Console.ReadLine ().Split (' ');
        int[] stocks = new int[n];
        int biggestLoss = 0;
        int topValue = 0;
        for (int i = 0 ; i < n ; i++) {
            stocks[i] = int.Parse (inputs[i]);
            Console.Error.WriteLine (stocks[i]);
        }

        for (int i = 0 ; i < n ; i++) {
            int a = stocks[i];
            if (topValue > a)
                continue;
            for (int j = i + 1 ; j < n ; j++) {
                if (stocks[j] > stocks[i]) {
                    continue;
                }
                int loss = stocks[i] - stocks[j];
                if (loss > biggestLoss) {
                    biggestLoss = loss;
                    topValue = a;
                }
            }
        }
        
        Console.WriteLine (-biggestLoss);
    }
}