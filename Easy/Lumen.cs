using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


class Solution
{
    

    static void Main(string[] args) {

        int sideSize = int.Parse(Console.ReadLine());
        int baseLight = int.Parse(Console.ReadLine());
        string[,] grid = new string[sideSize, sideSize];
        int[,] lightGrid = new int[sideSize, sideSize];
        int darkCount = 0;

        for (int i = 0; i < sideSize; i++) {
            string linha = Console.ReadLine();
            linha = Regex.Replace(linha, @"\s+", "");
            char[] cedulacao = linha.ToCharArray();

            for (int j = 0; j < sideSize; j++) {
                lightGrid[i, j] = 0;
                grid[i, j] = cedulacao[j].ToString();
            }
        }

        // debbugging =======
        for (int i = 0; i < sideSize; i++) {
            for (int j = 0; j < sideSize; j++) {
                Console.Error.Write(grid[i,j]);
            }
            Console.Error.Write("\n");
        }

        //illuminate room
        for (int i = 0; i < sideSize; i++) {
            for (int j = 0; j < sideSize; j++) {
                
                // look for candles
                for (int k = 0; k < sideSize; k++) {
                    for (int l = 0; l < sideSize; l++) {
                        if ( grid[k, l] == "C" ) {
                            // distancia = maior dos 2 -> absoluto -> subtracao pos cel[i,j] - pos vela
                            int distanceToCandleI = Math.Abs( i - k );
                            int distanceToCandleJ = Math.Abs( j - l );

                            int distance = Math.Max( distanceToCandleI, distanceToCandleJ );

                            int light = baseLight - distance;

                            if ( light <= 0 ) {
                                continue;
                            }

                            lightGrid[i, j] += light;
                        }
                    }
                }

            }
        }

                // debbugging =======
        for (int i = 0; i < sideSize; i++) {
            string row = "";
            for (int j = 0; j < sideSize; j++) {
                row += lightGrid[i,j];
                if ( lightGrid[i,j] == 0) {
                    darkCount++;
                }
            }
            Console.Error.WriteLine( row );
        }
        
        Console.WriteLine( darkCount );

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

    }
    
}