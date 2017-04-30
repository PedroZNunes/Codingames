using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/*
 Goal: The goal is to save at least one clone in a limited amount of rounds.
 
 The details:
Clones appear from a unique generator at regular intervals, every three game turns. The generator is located on floor 0. Clones exit the generator heading towards the right.

Clones move one position per turn in a straight line, moving in their current direction.

A clone is destroyed by a laser if it is goes below position 0 or beyond position width - 1.

Elevators are scattered throughout the drive and can be used to move from one floor to the one above. When a clone arrives on the location of an elevator, it moves up one floor. Moving up one floor takes one game turn. On the next turn, the clone continues to move in the direction it had before moving upward.

On each game turn you can either block the leading clone - meaning the one that got out the earliest - or do nothing.

Once a clone is blocked, you can no longer act on it. The next clone in line takes the role of "leading clone" and can be blocked at a later time.

When a clone moves towards a blocked clone, it changes direction from left to right or right to left. It also changes direction when getting out of the generator directly on a blocked clone or when going up an elevator onto a blocked clone.

If a clone is blocked in front of an elevator, the elevator can no longer be used.

When a clone reaches the location of the exit, it is saved and disappears from the area. 
 */
class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int floorsCount = int.Parse(inputs[0]); // number of floors
        int width = int.Parse(inputs[1]); // width of the area
        int nbRounds = int.Parse(inputs[2]); // maximum number of rounds
        int exitFloor = int.Parse(inputs[3]); // floor on which the exit is found
        int exitPos = int.Parse(inputs[4]); // position of the exit on its floor
        int clonesCountMax = int.Parse(inputs[5]); // number of generated clones
        int nbAdditionalElevators = int.Parse(inputs[6]); // ignore (always zero)
        int elevatorCount = int.Parse(inputs[7]); // number of elevators
        
        int[,] grid = new int[floorsCount,width]; //0-nothing 1-elevator 2-blockedClone 9-exit
        for (int i = 0; i < floorsCount; i++){
            for (int j = 0; j < width; j++){
                grid[i,j] = 0;
            }
        }
        
        grid[exitFloor, exitPos] = 9; //map exits
        
        for (int i = 0; i < elevatorCount; i++) //map elevators
        {
            inputs = Console.ReadLine().Split(' ');
            grid[int.Parse(inputs[0]), int.Parse(inputs[1])] = 1; //elevator floor / position
        }
        
        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int cloneFloor = int.Parse(inputs[0]); // floor of the leading clone
            int clonePos = int.Parse(inputs[1]); // position of the leading clone on its floor
            string direction = inputs[2]; // direction of the leading clone: LEFT or RIGHT
            // if there's no clone, wait.
            if (direction == "NONE"){
                Console.Error.WriteLine("No clones. waiting.");
                Console.WriteLine("WAIT");
                continue;
            }
            int localObjective = -1;
            //if exit on upper floor, find elevator. else find exit.
            if (exitFloor > cloneFloor){
                localObjective = 1;
                Console.Error.WriteLine ("exit is on a upper floor, going for the elevator");
            } else if (exitFloor == cloneFloor){
                localObjective = 9;
                Console.Error.WriteLine ("exit is on the same floor.");
            } else{
                Console.Error.WriteLine("exit is on a lower floor.");
                Console.WriteLine ("BLOCK");
                continue;
            }
            // locate the objective
            int objectivePos = -1;
            for (int i = 0; i < width; i++){
                if (grid[cloneFloor,i] == localObjective){
                    objectivePos = i;
                    Console.Error.WriteLine ("localObjective:{0} in pos: {1}",localObjective, objectivePos); 
                    break;
                }
            }
            Console.Error.WriteLine("{0}",direction);
            if ((direction == "RIGHT" && objectivePos >= clonePos) || (direction == "LEFT" && objectivePos <= clonePos)){
                Console.Error.WriteLine("Objective is straight forward");
                Console.WriteLine ("WAIT");
            } else {
                // look for blocked clones
                int blockedClonePos = -1;
                for (int i = 0; i < width; i++){
                   if (grid[cloneFloor,i] == 2){
                        blockedClonePos = grid[cloneFloor,i];
                        break;
                    }
                }
                
                if (blockedClonePos == -1){
                    Console.Error.WriteLine("there's no blocked clone in this floor");
                    grid[cloneFloor, clonePos] = 2;
                    Console.WriteLine ("BLOCK");
                } else if ((direction == "RIGHT" && blockedClonePos > clonePos) || (direction == "LEFT" && blockedClonePos < clonePos)){
                    Console.Error.WriteLine("Blocked clone straight ahead");
                    Console.WriteLine ("WAIT");
                } else {
                    Console.Error.WriteLine("Blocking leading clone");
                    grid[cloneFloor, clonePos] = 2;
                    Console.WriteLine ("BLOCK");
                }
                
            }
            
        }
        
    }
}