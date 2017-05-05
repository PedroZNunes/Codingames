using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/*
 * 	Goal

Count the number of lines needed to draw the skyline of a city.

The city skyline (or silhouette) as seen from a distance consists of a series of buildings with rectangular shape which might overlap each other.
Each building is described by its height (h) above ground level and the horizontal position of the left (x1) and right (x2) walls.

Line drawing rules:

To draw the skyline of a city with a single building 3 lines are required: One for the left wall, one for the roof and one for the right wall.
      ____
     |    |
     |    |
     |    |

Two separate buildings can be drawn using 7 lines: 3 lines for each of the two buildings and one line connecting the buildings on ground level.
      ____
     |    |      ______
     |    |     |      |
     |    |_____|      |

Two partially overlapping or adjacent buildings of different heights require 5 lines to be drawn. One for the left wall of the first building, two for the roofs, one for the wall connecting the roofs and one for the right wall of the second building.
The dotted lines in the picture below show the actual shape of the two buildings.
      ____
     |    |____
     |  ¦¨¦    |
     |  ¦ ¦    |

Two buildings are called adjacent (i.e. touching each other) if their opposing walls have the same horizontal position.
 */
class Solution {
    static void Main ( string[] args ) {
        int n = int.Parse (Console.ReadLine ());
        List<Building> buildings = new List<Building> ();
        for (int i = 0 ; i < n ; i++) {
            string[] inputs = Console.ReadLine ().Split (' ');
            int h = int.Parse (inputs[0]);
            int x1 = int.Parse (inputs[1]);
            int x2 = int.Parse (inputs[2]);
            buildings.Add (new Building (x1 , x2 , h));
        }

        buildings.Sort (( Building a , Building b ) => { return a.height.CompareTo (b.height); });
        buildings.Sort (( Building a , Building b ) => { return a.x1.CompareTo (b.x1); });

        for (int i = 0 ; i < buildings.Count () ; i++) {
            Console.Error.WriteLine ("{3} - pos({0}, {1}) - height: {2}." , buildings[i].x1 , buildings[i].x2 , buildings[i].height , buildings[i].id);
        }

        //this loops cleans up the scene by merging whatever it can.
        for (int i = 0 ; i < buildings.Count () ; i++) {
            Building a = buildings[i];
            for (int j = 0 ; j < buildings.Count () ; j++) {
                Building b = buildings[j];
                if (a.id == b.id) {
                    continue;
                }
                //loops adjacent and overlaping bigger
                if (a.OverlapsWith (b)) {
                    if (a.x2 >= b.x2 && a.height >= b.height) { //a bigger than b
                        Console.Error.WriteLine ("{0} got removed in first loop." , b.id);
                        buildings.Remove (b);
                        j = 0;
                    } else if (a.x2 < b.x2 && a.height == b.height) {
                        a.x2 = b.x2;
                        Console.Error.WriteLine ("{0} got removed. {1} incorporated {0}.x2" , b.id, a.id);
                        buildings.Remove (b);
                        j = 0;
                    }
                } else if (a.AdjacentTo (b) && a.height == b.height){
                    a.x2 = b.x2;
                    Console.Error.WriteLine ("{0} got removed. {1} incorporated {0}.x2" , b.id, a.id);
                    buildings.Remove (b);
                    j = 0;
                }
            }
        }
        buildings.Sort (( Building a , Building b ) => { return a.height.CompareTo (b.height); });
        buildings.Sort (( Building a , Building b ) => { return a.x1.CompareTo (b.x1); });
        Console.Error.WriteLine ("\n");

        for (int i = 0 ; i < buildings.Count () ; i++) {
            Console.Error.WriteLine ("{3} - pos({0}, {1}) - height: {2}." , buildings[i].x1 , buildings[i].x2 , buildings[i].height , buildings[i].id);
        }

        for (int i = 0 ; i < buildings.Count () ; i++) {
            Building a = buildings[i];
            Console.Error.WriteLine ("{3} - ({0}, {1}) - height: {2}." , buildings[i].x1 , buildings[i].x2 , buildings[i].height , a.id);
            for (int j = i ; j < buildings.Count () ; j++) {
                Building b = buildings[j];
                if (a.id == b.id)
                    continue;

                if (a.OverlapsWith (b)) {
                    Console.Error.WriteLine ("{0} overlaps {1}. {0}pos({2}, {3}) h{6}, {1}pos({4}, {5}) h{7}. " , a.id , b.id , a.x1 , a.x2 , b.x1 , b.x2 , a.height , b.height);
                    //totalLines -= 1;
                    if (b.x1 == a.x1) {
                        if (b.height > a.height) {
                            if (b.x2 >= a.x2) {
                                Console.Error.WriteLine ("{0} got removed." , a.id);
                                buildings.Remove (a);
                                i--;
                                break;
                            }
                            else {
                                a.x1 = b.x2;
                                a.TryDelLeft ();
                            }
                        }
                        else if (b.height < a.height) {
                            if (b.x2 <= a.x2) {
                                Console.Error.WriteLine ("{0} got removed." , b.id);
                                buildings.Remove (b);
                                j--;
                            }
                            else {
                                b.x1 = a.x2;
                                b.TryDelLeft ();
                            }
                        }
                        else { //b.h == a.h
                            if (b.x2 > a.x2) {
                                Console.Error.WriteLine ("{0} got removed." , a.id);
                                buildings.Remove (a);
                                i--;
                                break;
                            }
                            else {
                                Console.Error.WriteLine ("{0} got removed." , b.id);
                                buildings.Remove (b);
                                j--;
                            }
                        }
                    }

                    else { //a.x1 < b.x1 < a.x2
                        if (a.x2 > b.x2) {
                            //b inside A
                            if (a.height > b.height) {
                                Console.Error.WriteLine ("{0} got removed." , b.id);
                                buildings.Remove (b);
                                j--;
                            }

                            else if (a.height < b.height) { //weird
                                Console.Error.WriteLine ("{1} inside {0} and higher. splitting {0} into 2" , a.id , b.id);

                                Building newA = new Building (b.x2, a.x2, a.height, a.lineCount);
                                newA.TryDelLeft ();
                                newA.hasTop = a.hasTop;
                                int toInsert = -1;
                                for (int index = 0 ; index < buildings.Count() ; index++) {
                                    if (index == 0) {
                                        continue;
                                    }
                                    Building current = buildings[index];
                                    Building previous = buildings[index - 1];
                                    if (current.x1 >= newA.x1 && previous.x1 < newA.x1) {
                                        toInsert = index;
                                        if (current.x1 == newA.x1) {
                                            if ((previous.x1 == newA.x1 && previous.height < newA.height) && current.height >= newA.height) {
                                                toInsert = index;
                                            }
                                        }
                                    }
                                }

                                if (toInsert != ( -1 )) {
                                    Console.Error.WriteLine ("inserted in pos {0}" , toInsert);
                                    buildings.Insert (toInsert , newA);
                                }
                                else {
                                    Console.Error.WriteLine ("added");
                                    buildings.Add (newA);
                                }
                                
                                a.x2 = b.x1;
                                a.TryDelRight ();

                                foreach (Building build in buildings) {
                                    Console.Error.WriteLine ("{3} - pos({0}, {1}) - height: {2}." , build.x1 , build.x2 , build.height , build.id);
                                }
                            }

                            else {
                                Console.Error.WriteLine ("{0} got removed." , b.id);
                                buildings.Remove (b);
                                j--;
                            }
                        }

                        else if (a.x2 < b.x2) { //intercalation in X . , . ,

                            if (a.height > b.height) {
                                b.x1 = a.x2;
                                b.TryDelLeft ();
                            }

                            else if (a.height < b.height) {
                                a.x2 = b.x1;
                                a.TryDelRight ();
                            }

                            else {
                                a.x2 = b.x2;
                                Console.Error.WriteLine ("{0} got removed. {1} incorporated {0} x2" , b.id , a.id);
                                buildings.Remove (b);
                                j--;
                            }
                        }

                        else { //a.x2 == b.x2
                            if (a.height > b.height) {
                                Console.Error.WriteLine ("{0} got removed." , b.id);
                                buildings.Remove (b);
                                j--;
                            }

                            else if (a.height < b.height) {
                                a.x2 = b.x1;
                                a.TryDelRight ();
                            }

                            else {
                                Console.Error.WriteLine ("{0} got removed." , b.id);
                                buildings.Remove (b);
                                j--;
                            }
                        }
                    }
                }

                else if (a.AdjacentTo (b)) {
                    Console.Error.WriteLine ("{0} is adjacent to {1}.  Heights: {2} and {3}" , a.id , b.id , a.height , b.height);
                    if (a.height > b.height) {
                        b.TryDelLeft ();
                    }

                    else if (a.height < b.height) {
                        a.TryDelRight ();
                    }

                    else if (a.height == b.height) {
                        a.x2 = b.x2;
                        Console.Error.WriteLine ("THIS SHOULD NOT HAPPEN. {0} got removed. {1} incorporated {0} x2" , b.id , a.id);
                        buildings.Remove (b);
                        j--;
                    }
                }

            }
        }

        buildings.Sort (( Building a , Building b ) => { return a.x1.CompareTo (b.x1); });
        int totalLines = 0;
        for (int i = 0 ; i< buildings.Count () ; i++) { 
            Console.Error.WriteLine ("{0} has {1} lines" , buildings[i].id , buildings[i].lineCount);
            totalLines += buildings[i].lineCount;
            if (i < buildings.Count () - 1) {
                if (buildings[i].x2 < buildings[i + 1].x1) {
                    Console.Error.WriteLine ("Added separation between {0} and {1}. {0}.x2:{2}, {1}.x1:{3}",
                        buildings[i].id, buildings[i+1].id, buildings[i].x2, buildings[i+1].x1);
                    totalLines++; //separation line
                }
            }
        }

        Console.WriteLine (totalLines);
    }

    public class Building {
        static private int currentID = 0;

        public int id { get; private set; }
        public int height;
        public int x1;
        public int x2;
        public int lineCount {get; private set;}

        public bool hasRight = true, hasLeft = true, hasTop = true;

        public Building ( int x1 , int x2 , int h ) {
            id = currentID;
            this.x1 = x1;
            this.x2 = x2;
            height = h;
            lineCount = 3;

            currentID++;
        }

        public Building ( int x1 , int x2 , int h, int lines ) {
            id = currentID++;
            this.x1 = x1;
            this.x2 = x2;
            height = h;
            lineCount = lines;
        }

        public bool OverlapsWith ( Building b ) {
            return ( x1 <= b.x1 && x2 > b.x1 );
        }

        public bool AdjacentTo ( Building b ) {
            return ( x2 == b.x1 );
        }

        public bool SameHeightAs ( Building b ) {
            return ( height == b.height );
        }

        public bool TryDelTop () {
            if (hasTop) {
                hasTop = false;
                Console.Error.WriteLine ("{0} lost its top line" , id);
                lineCount--;
                return true;
            }
            else {
                Console.Error.WriteLine ("{0} had no top line to lose" , id);
                return false;
            }
        }

        public bool TryDelRight () {
            if (hasRight) {
                hasRight = false;
                Console.Error.WriteLine ("{0} lost its right line" , id);
                lineCount--;
                return true;
            }
            else {
                Console.Error.WriteLine ("{0} had no right line to lose" , id);
                return false;
            }
        }

        public bool TryDelLeft () {
            if (hasLeft) {
                hasLeft = false;
                Console.Error.WriteLine ("{0} lost its left line" , id);
                lineCount--;
                return true;
            }
            else {
                Console.Error.WriteLine ("{0} had no left line to lose" , id);
                return false;
            }
        }

    }



}

