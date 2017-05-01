using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

/*
Rules:

War is a card game played between two players. Each player gets a variable number of cards of the beginning of the game: that's the player's deck. Cards are placed face down on top of each deck.
 
Step 1 : the fight
At each game round, in unison, each player reveals the top card of their deck – this is a "battle" – and the player with the higher card takes both the cards played and moves them to the bottom of their stack. The cards are ordered by value as follows, from weakest to strongest:
2, 3, 4, 5, 6, 7, 8, 9, 10, J, Q, K, A.
 
Step 2 : war
If the two cards played are of equal value, then there is a "war". First, both players place the three next cards of their pile face down. Then they go back to step 1 to decide who is going to win the war (several "wars" can be chained). As soon as a player wins a "war", the winner adds all the cards from the "war" to their deck.
 
Special cases:
If a player runs out of cards during a "war" (when giving up the three cards or when doing the battle), then the game ends and both players are placed equally first.
The test cases provided in this puzzle are built in such a way that a game always ends (you do not have to deal with infinite games)
Each card is represented by its value followed by its suit: D, H, C, S. For example: 4H, 8C, AS.

When a player wins a battle, they put back the cards at the bottom of their deck in a precise order. First the cards from the first player, then the one from the second player (for a "war", all the cards from the first player then all the cards from the second player).
*/

class Solution {
    static Queue<int> playerCards1 = new Queue<int> ();
    static Queue<int> playerCards2 = new Queue<int> ();
    static Queue<int> battleCards1 = new Queue<int> ();
    static Queue<int> battleCards2 = new Queue<int> ();
    static bool atWar = false;
    static bool isOver = false;
    static int nRounds = 0;
    static int winner = -1;

    //Main method has to be static by the site's default
    static void Main ( string[] args ) {
        int n1 = int.Parse (Console.ReadLine ()); // the number of cards for player 1
        for (int i = 0 ; i < n1 ; i++) {
            string input = Console.ReadLine ();
            input = input.Substring (0 , input.Length - 1);
            Console.Error.WriteLine (input);
            int card = Int32.Parse (input.Replace ("J" , "11").Replace ("Q" , "12").Replace ("K" , "13").Replace ("A" , "14"));
            playerCards1.Enqueue (card); // the n cards of player 1
        }

        int n2 = int.Parse (Console.ReadLine ()); // the number of cards for player 2
        for (int i = 0 ; i < n2 ; i++) {
            string input = Console.ReadLine ();
            input = input.Substring (0 , input.Length - 1);
            int card = Int32.Parse (input.Replace ("J" , "11").Replace ("Q" , "12").Replace ("K" , "13").Replace ("A" , "14"));
            playerCards2.Enqueue (card); // the n cards of player 2
        }

        while (!isOver) {
            Console.Error.WriteLine ("\npreBattle - n1: {0}, n2: {1}" , playerCards1.Count () , playerCards2.Count ());
            nRounds++;
            Battle ();
            while (atWar) {
                Battle ();
            }
        }

        if (winner == -1) {
            Console.WriteLine ("PAT");
        }
        else {
            Console.WriteLine (winner + " " + nRounds);
        }

    }

    static void Battle () {
        int card1 = playerCards1.Dequeue ();
        int card2 = playerCards2.Dequeue ();
        battleCards1.Enqueue (card1);
        battleCards2.Enqueue (card2);

        Console.Error.WriteLine ("battle started {0} vs {1}" , battleCards1.Peek () , battleCards2.Peek ());
        if (card1 > card2) {
            while (battleCards1.Count () > 0) {
                playerCards1.Enqueue (battleCards1.Dequeue ());
            }
            while (battleCards2.Count () > 0) {
                playerCards1.Enqueue (battleCards2.Dequeue ());
            }

            if (playerCards2.Count () == 0) {
                winner = 1;
                isOver = true;
            }

            atWar = false;
        }
        else if (card2 > card1) {
            while (battleCards1.Count () > 0) {
                playerCards2.Enqueue (battleCards1.Dequeue ());
            }
            while (battleCards2.Count () > 0) {
                playerCards2.Enqueue (battleCards2.Dequeue ());
            }

            if (playerCards1.Count () == 0) {
                winner = 2;
                isOver = true;
            }

            atWar = false;
        }
        else if (card1 == card2) {
            Console.Error.WriteLine ("--War started--");

            if (playerCards1.Count () < 4 || playerCards2.Count () < 4) {
                winner = -1;
                atWar = false;
                isOver = true;
            }
            else {
                for (int i = 0 ; i < 3 ; i++) {
                    battleCards1.Enqueue (playerCards1.Dequeue ());
                    battleCards2.Enqueue (playerCards2.Dequeue ());
                }
                atWar = true;
                Console.Error.WriteLine ("3 cards placed during war. n1: {0}, n2: {1}" , playerCards1.Count () , playerCards2.Count ());
            }
        }

    }

}