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
class Player {
    static private PlayerUnit[] playerUnits;

    static private Vector2[] enemyPos;

    static private Dictionary<Vector2 , string> coordsToCardinal = new Dictionary<Vector2 , string> ()
    {
        { new Vector2 (0, -1),   "N" },
        { new Vector2 (1, -1),   "NE" },
        { new Vector2 (1, 0),   "E" },
        { new Vector2 (1, 1),  "SE" },
        { new Vector2 (0, 1),  "S" },
        { new Vector2 (-1, 1), "SW" },
        { new Vector2 (-1, 0),  "W" },
        { new Vector2 (-1, -1),  "NW" }
    };

    static private int size;
    static private int unitsPerPlayer;

    static private Grid grid;

    static private string outMoveDir = null;
    static private string outBuildDir = null;
    static private int finalIndex = 0;
    static private bool isPushing = false;

    static private int MAX_NEIGHBOURS = 8;

    static void Main ( string[] args ) {
        //   string[] inputs;
        size = int.Parse (Console.ReadLine ());
        unitsPerPlayer = int.Parse (Console.ReadLine ());

        playerUnits = new PlayerUnit[unitsPerPlayer];
        for (int i = 0 ; i < unitsPerPlayer ; i++) {
            playerUnits[i] = new PlayerUnit ();
        }

        enemyPos = new Vector2[unitsPerPlayer];

        // game loop
        while (true) {
            Initialization ();

            MakePlay ();

            string action = ( isPushing ) ? "PUSH&BUILD" : "MOVE&BUILD";

            Console.WriteLine ("{0} {1} {2} {3}" , action , finalIndex , outMoveDir , outBuildDir);
        }
    }

    static private void Initialization () {
        grid = new Grid (size);

        for (int rows = 0 ; rows < size ; rows++) {
            char[] rawHeight = Console.ReadLine ().ToCharArray ();

            for (int columns = 0 ; columns < size ; columns++) {
                int height;

                bool isValid = int.TryParse (rawHeight[columns].ToString () , out height);
                if (!isValid || height > 3) {
                    height = -1;
                }

                Vector2 position = new Vector2 (columns , rows);
                //Console.Error.WriteLine("Node: Position {1}, Height {0}", height, position.ToString());

                grid.AddNode (new Node (position , height));
            }
        }
        /// prints the whole grid
        //StringBuilder builder = new StringBuilder();
        //for (int rows = 0; rows < size; rows++)
        //{
        //    for (int columns = 0; columns < size; columns++)
        //    {
        //        Node temp = grid[new Vector2(columns, rows)];
        //        builder.Append(temp.pos.ToString()).Append(temp.height).Append(" ");
        //    }
        //    builder.AppendLine(" ");
        //}
        //Console.Error.WriteLine(builder);

        StringBuilder positionsString = new StringBuilder ();
        for (int i = 0 ; i < unitsPerPlayer ; i++) {
            string[] rawPos = Console.ReadLine ().Split (' ');
            playerUnits[i].pos = new Vector2 (int.Parse (rawPos[0]) , int.Parse (rawPos[1]));
            positionsString.AppendFormat ("Player{0}: {1} | " , i , playerUnits[i].pos.ToString ());

            playerUnits[i].node = grid[playerUnits[i].pos];
            playerUnits[i].canMove = true;
        }

        for (int i = 0 ; i < unitsPerPlayer ; i++) {
            string[] rawPos = Console.ReadLine ().Split (' ');
            enemyPos[i] = new Vector2 (int.Parse (rawPos[0]) , int.Parse (rawPos[1]));
            positionsString.AppendFormat ("Enemy{0} {1} | " , i , enemyPos[i].ToString ());
        }
        Console.Error.WriteLine (positionsString);

        int legalActions = int.Parse (Console.ReadLine ());
        for (int i = 0 ; i < legalActions ; i++) {
            string[] inputs = Console.ReadLine ().Split (' ');
            //inputs = Console.ReadLine().Split(' ');
            //string atype = inputs[0];
            //int index = int.Parse(inputs[1]);
            //string dir1 = inputs[2];
            //string dir2 = inputs[3];
        }
    }

    static private void MakePlay () {
        Play bestPlay = new Play ();

        for (int currentPlayerIndex = 0 ; currentPlayerIndex < unitsPerPlayer ; currentPlayerIndex++) {
            Node playerNode = playerUnits[currentPlayerIndex].node;

            Node moveTarget = new Node ();
            Node buildTarget = new Node ();

            Node[] neighbours;
            int currentHeight = playerNode.height;

            int initiativeScore = 0;
            int moveScore = 0;
            int buildScore = 0;

            //initiative score
            Node[] playerNeighbours = grid.GetAllValidNeighbours (playerNode);
            initiativeScore = CalculateInitiativeScore (playerNode , playerNeighbours);


            //push play
            for (int i = 0 ; i < enemyPos.Length ; i++) {
                if (enemyPos[i] == new Vector2 (-1 , -1))
                    continue;

                Node enemyNode = grid.GetNodeByPosition (enemyPos[i]);
                if (playerNode.IsAdjacentTo (enemyNode)) {

                    isPushing = true;
                    moveTarget = enemyNode;
                    moveScore = 0;
                    neighbours = grid.GetAllValidNeighbours (moveTarget);

                    if (neighbours.Length == 0)
                        continue;

                    moveScore = CalculateMoveScore (moveTarget , playerNode , neighbours , isPushing);

                    //set push direction
                    Vector2 mainDirection = moveTarget.pos - playerNode.pos;

                    for (int j = 0 ; j < neighbours.Length ; j++) {
                        //a soma do X e do Y tem q ser = 1
                        Vector2 neighbourDir = neighbours[j].pos - moveTarget.pos;
                        
                        if (( Math.Abs (neighbourDir.x - mainDirection.x) + Math.Abs (neighbourDir.y - mainDirection.y) ) <= 1) {
                            if (neighbours[j].height <= enemyNode.height + 1) {
                                buildTarget = neighbours[j];
                                Node[] targetNeighbours = grid.GetAllValidNeighbours (buildTarget);
                                buildScore = CalculateBuildScore (buildTarget , moveTarget , isPushing , targetNeighbours);

                                int score = initiativeScore + moveScore + buildScore;
                                if (score > bestPlay.score || ( bestPlay.isNull () )) {
                                    Console.Error.Write ("IS {0}, MS {1}, BS {2}" , initiativeScore , moveScore , buildScore);
                                    bestPlay = new Play (currentPlayerIndex , playerNode , moveTarget , buildTarget , score , isPushing);
                                    Console.Error.WriteLine ("| {0}" , bestPlay.ToString ());
                                }
                            }
                        }
                    }
                }
            }


            //move then build score
            for (int i = 0 ; i < playerNeighbours.Length ; i++) {
                if (playerNeighbours.Length == 0) {
                    playerUnits[i].canMove = false;
                    break;
                }

                if (playerNeighbours[i].height > playerNode.height + 1)
                    continue;

                moveScore = 0;
                isPushing = false;

                moveTarget = playerNeighbours[i];
                Node[] targetsNeighbours = grid.GetAllValidNeighbours (moveTarget , playerNode);

                moveScore = CalculateMoveScore (moveTarget , playerNode , targetsNeighbours , isPushing);

                //Deal with building
                //check the surroundings for building 
                for (int j = 0 ; j < targetsNeighbours.Length ; j++) {

                    buildScore = 0;

                    buildTarget = targetsNeighbours[j];
                    Node[] buildTargetNeighbours = grid.GetAllValidNeighbours (buildTarget, playerNode);
                    buildScore = CalculateBuildScore (buildTarget , moveTarget , isPushing, buildTargetNeighbours);

                    int score = initiativeScore + moveScore + buildScore;
                    if (score > bestPlay.score || ( bestPlay.isNull () )) {
                        Console.Error.Write ("IS {0}, MS {1}, BS {2}" , initiativeScore, moveScore , buildScore);
                        bestPlay = new Play (currentPlayerIndex , playerNode , moveTarget , buildTarget , score , isPushing);
                        Console.Error.WriteLine ("| {0}" , bestPlay.ToString ());
                    }
                    else if (score == bestPlay.score) {
                        if (playerNeighbours.Contains (buildTarget) && !playerNeighbours.Contains (bestPlay.buildTarget)) {
                            Console.Error.Write ("IS {0}, MS {1}, BS {2}" , initiativeScore , moveScore , buildScore);
                            bestPlay = new Play (currentPlayerIndex , playerNode , moveTarget , buildTarget , score , isPushing);
                            Console.Error.WriteLine ("| {0}" , bestPlay.ToString ());
                        }
                    }
                }
            }
        }

        outMoveDir = SetDirection (bestPlay.playerNode.pos , bestPlay.moveTarget.pos);
        outBuildDir = SetDirection (bestPlay.moveTarget.pos , bestPlay.buildTarget.pos);
        finalIndex = bestPlay.playerIndex;
        isPushing = bestPlay.isPushing;

        Console.Error.WriteLine ("Move Target: {0} , Build Target: {1}" ,
            bestPlay.moveTarget.pos.ToString () , bestPlay.buildTarget.pos.ToString ());
    }

    static private bool CanBeStolen ( Node node ) {
        Node[] neighbours = grid.GetAllNeighbours (node);
        for (int i = 0 ; i < neighbours.Length ; i++) {
            for (int j = 0 ; j < enemyPos.Length ; j++) {
                if (enemyPos[j] == new Vector2 (-1 , -1))
                    continue;

                Node enemyNode = grid.GetNodeByPosition (enemyPos[j]);
                if (neighbours[i].pos == enemyPos[j] && node.height <= enemyNode.height + 1) {
                    return true;
                }
            }
        }
        return false;
    }

    static private string SetDirection ( Vector2 playerPos , Vector2 targetPos ) {
        Vector2 dir = new Vector2 ();

        if (targetPos.x > playerPos.x) {
            dir.x = 1;
        }
        else if (targetPos.x == playerPos.x) {
            dir.x = 0;
        }
        else {
            dir.x = -1;
        }


        if (targetPos.y > playerPos.y) {
            dir.y = 1;
        }
        else if (targetPos.y == playerPos.y) {
            dir.y = 0;
        }
        else {
            dir.y = -1;
        }

        Console.Error.WriteLine ("Direction being Set: {0}" , dir.ToString ());

        return coordsToCardinal[dir];
    }

    static private int CalculateInitiativeScore (Node playerNode, Node[] neighbours) {
        int initiativeScore = 0;
        bool isFirstInstance = true;

        for (int i = 0 ; i < neighbours.Length ; i++) {
            if (neighbours[i].height <= playerNode.height + 1) {
                if (isFirstInstance) {
                    isFirstInstance = false;
                    initiativeScore = 50;
                }
                else {
                    initiativeScore = -25;
                }
            }
        }

        return ( initiativeScore > 0 ) ? initiativeScore : 0;
    }

    static private int CalculateMoveScore ( Node moveTarget , Node playerNode , Node[] neighbours , bool isPushing ) {
        //move.
        int moveScore = 0;
        //is pushing?
        if (isPushing) {
            if (playerNode.height == 3)
                moveScore = 40;
            else if (playerNode.height == 2)
                moveScore = 20;
            else 
                moveScore = 0;

            if (moveTarget.height == 3)
                moveScore += 60;
            else if (moveTarget.height == 2)
                moveScore += 30;
            else if (moveTarget.height == 1)
                moveScore += 10;

            if (playerNode.height <= moveTarget.height)
                moveScore += 15;
        }
        else {
            if (moveTarget.height == 0)
                moveScore = 0;
            else if (moveTarget.height == 1)
                moveScore = 15;
            else if (moveTarget.height == 2)
                moveScore = 45;
            else
                moveScore = 100;

            // mod for build options around move target
            if (neighbours.Length <= 1) {
                moveScore += -80;
            }
            else {
                for (int i = 0 ; i < neighbours.Length ; i++) {
                    if (neighbours[i].height == moveTarget.height || neighbours[i].height == moveTarget.height + 1) {
                        moveScore += 2 * ( neighbours[i].height * neighbours[i].height );
                    }
                    else {
                        moveScore += neighbours[i].height * neighbours[i].height;
                    }
                }
            }

            //mod for avoiding getting kicked out by the enemy when possible
            //this should be very subtle
            for (int i = 0 ; i < enemyPos.Length ; i++) {
                if (enemyPos[i] == new Vector2 (-1 , -1))
                    continue;
                Node enemyNode = grid.GetNodeByPosition (enemyPos[i]);
                if (moveTarget.IsAdjacentTo (enemyNode)) {
                    moveScore += -10 + (-10 * moveTarget.height);
                }
            }

        }
        return moveScore;
    }
    
    static private int CalculateBuildScore ( Node buildTarget , Node playerNode , bool isPushing , Node[] neighbours ) {
        //Build.
        int buildScore = 0;

        if (isPushing) {
            StringBuilder pushDecomposed = new StringBuilder ();
            Node enemyNode = playerNode;
            Node landingNode = buildTarget;

            //push based on height difference
            int heightDifference = ( enemyNode.height + 1 ) - landingNode.height;
            buildScore += heightDifference * 15 * enemyNode.height;
            pushDecomposed.AppendFormat ("Push build score decomposed: {0} -> ", buildScore);

            //mod to nerf pushes at height 0
            if (enemyNode.height == 0)
                buildScore += -10;
            pushDecomposed.AppendFormat ("{0} -> " , buildScore);

            bool isDampened = false;
            for (int i = 0 ; i < neighbours.Length ; i++) {
                //mod to consider enemy surroundings
                buildScore += 2 * (neighbours[i].height * neighbours[i].height);

                //dampen the play a bit if the enemy can just walk back in quickly
                if (neighbours[i].height == landingNode.height + 1) {
                    if (!isDampened) {
                        isDampened = true;
                        buildScore += -5;
                    }
                }
            }
            pushDecomposed.AppendFormat ("{0} -> " , buildScore);

            //mod to buffing the play if theres a player unit that can take advantage of the extra height
            if (enemyNode.height < 3 && enemyNode.height > 0) {
                for (int i = 0 ; i < playerUnits.Length ; i++) {
                    if (playerUnits[i].node.IsAdjacentTo (enemyNode)) {
                        if (playerUnits[i].node.height == enemyNode.height || playerUnits[i].node.height == enemyNode.height + 1) {
                            buildScore += 10 * ( enemyNode.height + 1 );
                        }
                    }
                }
            }
            pushDecomposed.AppendFormat ("{0} -> " , buildScore);

            //mod to put the enemy in awkward positions
            Node[] landingNeighbours = grid.GetAllValidNeighbours (landingNode);
            int count = 0;
            for (int i = 0 ; i < landingNeighbours.Length ; i++) {
                if (landingNeighbours[i].height <= landingNode.height + 1) {
                    count++;
                }
            }
            buildScore = 10 * (MAX_NEIGHBOURS - count);

            //mod to not sending the enemy to a better spot
            if (landingNode.height > enemyNode.height)
                buildScore += -60;
            pushDecomposed.AppendFormat ("{0} -> " , buildScore);

            //mod to lock enemy up
            //this should be pretty strong as it can lock the enemy
            if (neighbours.Length <= 1) {
                buildScore += 1000;
            }
            pushDecomposed.AppendFormat ("{0}. " , buildScore);
            Console.Error.WriteLine (pushDecomposed);
        }

        else {
            Node[] buildNeighbours = grid.GetAllValidNeighbours (buildTarget);
            if (buildTarget.height <= playerNode.height) {
                if (buildTarget.height == 0) {
                    Node[] temp;
                    if (grid.TryGetNeighboursByHeight (buildTarget , 2 , playerNode , out temp))
                        buildScore = 15;
                    else
                        buildScore = 0;
                }
                else if (buildTarget.height == 1) {
                    Node[] temp;
                    if (grid.TryGetNeighboursByHeight (buildTarget , 3 , playerNode , out temp))
                        buildScore = 50;
                    else
                        buildScore = 10;
                }
                else if (buildTarget.height == 2) {
                    if (CanBeStolen (buildTarget))
                        buildScore = -100;
                    else {
                        //test for visibility
                        bool isVisible = false;
                        for (int i = 0 ; i < playerUnits.Length ; i++) {
                            if (buildTarget.IsAdjacentTo(playerUnits[i].node)) {
                                buildScore = 40;
                                isVisible = true;
                            }
                        }
                        if (!isVisible)
                            buildScore = -50;
                    }
                }
                else {
                    if (CanBeStolen (buildTarget)) {
                        buildScore = 100;
                    }
                    else {
                        //test if it is accessible
                        for (int i = 0 ; i < buildNeighbours.Length ; i++) {
                            if (buildNeighbours[i].height >= 2) {
                                buildScore = 60;
                            }
                        }
                    }
                }

                //mod for helping other units
                for (int i = 0 ; i < playerUnits.Length ; i++) {
                    if (buildTarget.height < 3) {
                        if (playerUnits[i].node.IsAdjacentTo (buildTarget)) {
                            if (playerUnits[i].node.height == buildTarget.height) {
                                buildScore += 10 * buildTarget.height;
                            }
                            else if (playerUnits[i].node.height == buildTarget.height + 1) {
                                buildScore += 5 * buildTarget.height;
                            }
                        }
                    }
                }

                //mod for building closer to allies
                for (int i = 0 ; i < playerUnits.Length ; i++) {
                    if (playerUnits[i].node.IsAdjacentTo (buildTarget)) {
                        buildScore += 5;
                    }
                }

                //mod for avoiding giving enemies free nodes
                for (int i = 0 ; i < enemyPos.Length ; i++) {
                    if (enemyPos[i] == new Vector2 (-1 , -1))
                        continue;
                    Node enemyNode = grid.GetNodeByPosition (enemyPos[i]);
                    if (buildTarget.IsAdjacentTo (enemyNode)) {
                        if (buildTarget.height <= enemyNode.height + 1) {
                            if (buildTarget.height < 3)
                                buildScore += -15 + (-5 * (buildTarget.height + 1));
                        }
                    }
                }

                //mod for adjacent good neighbours
                for (int i = 0 ; i < neighbours.Length ; i++) {
                    if (neighbours.Length == 0)
                        break;

                    if (neighbours[i].height <= playerNode.height + 1) {
                        buildScore += 2 * neighbours[i].height;
                    }
                    else {
                        buildScore += neighbours[i].height;
                    }
                }
            }
            else //cannot move to it next turn
            {
                if (buildTarget.height == 3) {
                    if (CanBeStolen (buildTarget))
                        buildScore += 80;
                    else {
                        for (int i = 0 ; i < playerUnits.Length ; i++) {
                            if (playerUnits[i].node.IsAdjacentTo (buildTarget)) {
                                if (playerUnits[i].node.height <= buildTarget.height) {
                                    buildScore += -100;
                                }
                            }
                        }
                    }
                }
            }
        }

        return buildScore;
    }

    private class PlayerUnit {
        public Vector2 pos;
        public Node node;
        public Node moveTarget;
        public Node buildTarget;
        public bool canMove = true;
        public int playScore;

        public PlayerUnit () {
            pos = new Vector2 ();
            node = new Node ();
            moveTarget = new Node ();
            buildTarget = new Node ();
            canMove = true;
            playScore = 0;
        }
    }

    private struct Play {
        public int playerIndex;
        public Node playerNode;
        public Node moveTarget;
        public Node buildTarget;
        public int score;
        public bool isPushing;

        public Play ( int index , Node playerNode , Node moveTarget , Node buildTarget , int score , bool isPushing ) {
            this.playerIndex = index;
            this.playerNode = playerNode;
            this.moveTarget = moveTarget;
            this.buildTarget = buildTarget;
            this.score = score;
            this.isPushing = isPushing;
        }

        public override string ToString () {
            return ( String.Format ("i{0}, From{1}, M{2}, B{3}, Score:{4}, Push:{5}" ,
                            playerIndex , playerNode.pos.ToString () , moveTarget.pos.ToString () ,
                            buildTarget.pos.ToString () , score , isPushing) );
        }

        public bool isNull () {
            return ( this.playerNode == null );
        }
    }

    private struct Vector2 {
        public int x;
        public int y;

        static public Vector2 zero { get { return new Vector2 (0 , 0); } }

        public Vector2 ( int x , int y ) {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator + ( Vector2 a , Vector2 b ) {
            return new Vector2 (a.x + b.x , a.y + b.y);
        }

        public static Vector2 operator - ( Vector2 a , Vector2 b ) {
            return new Vector2 (a.x - b.x , a.y - b.y);
        }

        public static bool operator == ( Vector2 a , Vector2 b ) {
            return ( a.x == b.x && a.y == b.y );
        }

        public static bool operator != ( Vector2 a , Vector2 b ) {
            return ( a.x != b.x || a.y != b.y );
        }

        public bool Equals ( Vector2 other ) {
            return Equals (other , this);
        }

        public override bool Equals ( object obj ) {
            if (obj == null || GetType () != obj.GetType ()) {
                return false;
            }

            Vector2 other = (Vector2) obj;

            return other.x == x && other.y == y;
        }

        public override int GetHashCode () {
            var id = 10 * x + y;
            return id.GetHashCode ();
        }

        public override string ToString () {
            return ( String.Format ("({0}, {1})" , x , y) );
        }

        static public Vector2 DistanceBetween ( Vector2 a , Vector2 b ) {
            return ( new Vector2 (b.x - a.x , b.y - a.y) );
        }
    }

    private class Node {
        public Vector2 pos;
        public int height;

        public Node () {
            this.pos = new Vector2 ();
            this.height = -1;
        }

        public bool isEmpty () {
            return ( this.height == -1 );
        }

        public Node ( int x , int y , int height ) {
            this.height = height;
            pos.x = x;
            pos.y = y;
        }

        public Node ( Vector2 pos , int height ) {
            this.pos = pos;
            this.height = height;
        }

        public bool IsAdjacentTo ( Node other ) {
            Vector2 distance = Vector2.DistanceBetween (this.pos , other.pos);
            return ( Math.Abs (distance.x) <= 1 && Math.Abs (distance.y) <= 1 );
        }

        public bool isAvailable () {
            return ( height >= 0 && height <= 3 );
        }

    }

    private class Grid {
        public int size;
        public Node[,] nodes { get; private set; }

        public Grid ( int size ) {
            this.size = size;
            nodes = new Node[size , size];
        }

        public Node this[Vector2 position] {
            get {
                Node a = GetNodeByPosition (position);
                return a;
            }
        }

        public void AddNode ( Node node ) {
            nodes[node.pos.x , node.pos.y] = node;
        }
       
        public Node[] GetAllValidNeighbours ( Node core , Node ignoredPlayerNode ) {
            List<Node> nodeList = new List<Node> (GetAllNeighbours (core));
            for (int i = 0 ; i < nodeList.Count ; i++) {
                for (int j = 0 ; j < unitsPerPlayer ; j++) {
                    if (nodeList[i].pos == enemyPos[j] || ( nodeList[i] == playerUnits[j].node && playerUnits[j].node != ignoredPlayerNode )) {
                        nodeList.RemoveAt (i);
                        i--;
                        break;
                    }
                }
            }
            return nodeList.ToArray ();
        }

        public Node[] GetAllValidNeighbours ( Node core ) {
            List<Node> nodeList = new List<Node> (GetAllNeighbours (core));
            for (int i = 0 ; i < nodeList.Count ; i++) {
                for (int j = 0 ; j < unitsPerPlayer ; j++) {
                    if (nodeList[i].pos == enemyPos[j] || ( nodeList[i] == playerUnits[j].node )) {
                        nodeList.RemoveAt (i);
                        i--;
                        break;
                    }
                }
            }
            return nodeList.ToArray ();
        }

        public Node[] GetAllNeighbours ( Node core ) {
            //gets all nodes adjacent to the core
            List<Node> nodeList = new List<Node> ();
            int x = core.pos.x;
            int y = core.pos.y;

            //E
            Node node = GetNodeByPosition (new Vector2 (x + 1 , y));
            if (node != null)
                if (node.height != -1)
                    nodeList.Add (node);
            //SE
            node = GetNodeByPosition (new Vector2 (x + 1 , y - 1));
            if (node != null)
                if (node.height != -1)
                    nodeList.Add (node);
            //S
            node = GetNodeByPosition (new Vector2 (x , y - 1));
            if (node != null)
                if (node.height != -1)
                    nodeList.Add (node);
            //SW
            node = GetNodeByPosition (new Vector2 (x - 1 , y - 1));
            if (node != null)
                if (node.height != -1)
                    nodeList.Add (node);
            //W
            node = GetNodeByPosition (new Vector2 (x - 1 , y));
            if (node != null)
                if (node.height != -1)
                    nodeList.Add (node);
            //NW
            node = GetNodeByPosition (new Vector2 (x - 1 , y + 1));
            if (node != null)
                if (node.height != -1)
                    nodeList.Add (node);
            //N
            node = GetNodeByPosition (new Vector2 (x , y + 1));
            if (node != null)
                if (node.height != -1)
                    nodeList.Add (node);
            //NE
            node = GetNodeByPosition (new Vector2 (x + 1 , y + 1));
            if (node != null)
                if (node.height != -1)
                    nodeList.Add (node);

            return nodeList.ToArray ();
        }


        public bool TryGetNeighboursByHeight ( Node node , int height , Node playerNode , out Node[] neighbours ) {
            //gets all cells around 'cell' with desired height
            Node[] nodes = GetAllValidNeighbours (node);

            List<Node> nodeList = new List<Node> ();
            for (int i = 0 ; i < nodes.Length ; i++) {
                if (nodes[i].height == height) {
                    nodeList.Add (nodes[i]);
                }
            }

            neighbours = nodeList.ToArray ();
            return ( neighbours.Length > 0 );
        }

        public Node GetNodeByPosition ( Vector2 pos ) {
            if (pos.x < size && pos.x >= 0 && pos.y < size && pos.y >= 0) {
                return nodes[pos.x , pos.y];
            }
            return null;
        }

    }
}