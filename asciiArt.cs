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
class Solution
{
    static void Main(string[] args)
    {
        int Width = int.Parse(Console.ReadLine());
        int Height = int.Parse(Console.ReadLine());
        string tempText = Console.ReadLine().ToUpper();
        char[] Text = tempText.ToCharArray();
        Console.Error.WriteLine("input text " + tempText);
        
        List<char>[] outText = new List<char>[Height];
        for (int i = 0 ; i < Height ; i++){
            outText[i] = new List<char> ();
        }
        
        char[] letters = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','?'};
        string[] asciiText = new string[Height];

        for (int i = 0; i < Height; i++)
        {
            asciiText[i] = Console.ReadLine().ToUpper();
        }

        for (int i = 0; i < Text.Length; i++)
        {

            int letterIndex = Array.IndexOf(letters, Text[i]);
            if (letterIndex == -1){
                letterIndex = letters.Length-1; 
            }
            int asciiIndexBegin = letterIndex * Width;
            int asciiIndexEnd = asciiIndexBegin + Width;
            Console.Error.WriteLine(letterIndex + " " + asciiIndexBegin + " " + asciiIndexEnd);
            
            for (int j = 0 ; j < Height ; j++){
                char[] sub = asciiText[j].Substring(asciiIndexBegin,Width).ToCharArray();
                Console.Error.WriteLine (sub);
                outText[j].AddRange(sub.ToList());
            }
        }

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");
        for (int i = 0 ; i < Height ; i++){
            string temp = new string (outText[i].ToArray());
            Console.Error.WriteLine(i + "->" + temp);
            Console.WriteLine(temp);
        }
    }
}