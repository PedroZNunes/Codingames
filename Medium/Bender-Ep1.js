/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

 

class Bender {

    constructor( pos ) {
        // 4 - try south, then east, then north, then west. ccw
        this.directions         =   ['s', 'e', 'n', 'w'];
        this.direction_i        =   0;

        this.pos                =   [];
        this.pos[0]             =   pos[0];
        this.pos[1]             =   pos[1];

        this.is_inverted        =   false;
        this.is_overloaded      =   false;
    }
    
    
    main() {

        if ( grid[this.pos[0]] [this.pos[1]] == '$' ) {
            return 'done'; //suicide
        
        } else if ( grid[this.pos[0]] [this.pos[1]] == 'T' ) {

        }
        // grab the pos in front.
        let check_pos           =   [];
        check_pos               =   this.get_next_pos();

        let target_cell         =   grid[check_pos[0]] [check_pos[1]];
        let out                 =   '';

        let turn_count          =   0;

        while ( ( target_cell       ==  "#" ) || 
                ( target_cell       ==  "X" && ! this.is_overloaded )
        ) {
            if ( turn_count         >=  4 ) {
                return 'LOOP';
            }

            if ( turn_count         ==  0 ) {
                if ( this.is_inverted ) {
                    this.turn( 'w' );
                } else {
                    this.turn( 's' );
                }
            } else {              
                this.turn();
                
            }

            check_pos       =   this.get_next_pos();
            target_cell     =   grid[check_pos[0]] [check_pos[1]];

            turn_count++;
        }

        switch ( target_cell ) {
            case '@':
            case ' ':
            case '$':
                out             =   this.walk(); //just walk
                break;
                
            // case '#':
            //     this.turn(); // obstacle
            //     break;

            case 'T':
                out             =   this.walk();
                this.teleport();// teleport
                break;

            case 'X':
                // breakable obstacle
                if ( this.is_overloaded ) {
                    out         =   this.walk();
                    grid[this.pos[0]] [this.pos[1]] = ' ';
                    console.error( 'rolling on the barrels! %s', this.pos);
                }   
                break;  

            case 'B':   
                out             =   this.walk();
                this.overload();
                break;  

            case 'I':   
                out             =   this.walk();
                this.invert();
                break;
                
            case 'S':
            case 'E':
            case 'N':
            case 'W':
                out             =   this.walk();
                console.error( "found modifier: %s. %s", target_cell.toLowerCase(), this.pos );
                this.turn( target_cell.toLowerCase() );
                break;

            default:
                // console.error('target_cell type not recognized. %s, %s. target_cell: %s', this.pos[0], this.pos[1], target_cell);
                break;
        }

        return ( out );
    }

    // 5 - change direction -> s-south,  e-east, n-north and w-west
    turn( dir ) {
        if ( dir == null ) {
            //check for wall
            if ( this.is_inverted ) {
                this.direction_i--;
    
                    if ( this.direction_i < 0 ) {
                        this.direction_i    =   3;
                    } 
            } else {
                this.direction_i++;
                                
                if ( this.direction_i % 4 == 0 ) {
                    this.direction_i        =   0;   
                }
            }
            // console.error( 'facing %s', this.directions[this.direction_i].toString() );
        } else {
            // console.error( 'facing %s, by reset or by force', dir );

             switch (dir) {
                case 's':
                    this.direction_i        =   0;
                    break;
                        
                case 'e':
                    this.direction_i        =   1;
                    break;
                        
                case 'n':
                    this.direction_i        =   2;
                    break;
                    
                case 'w':
                    this.direction_i        =   3;
                    break;
             }
        }
    }

    get_next_pos() { 
        let dir_array       =   this.get_dir_as_array( this.directions[this.direction_i] );

        let check_pos       =   [];
        check_pos[0]        =   this.pos[0] + dir_array[0];
        check_pos[1]        =   this.pos[1] + dir_array[1];

        return check_pos;
    }
        
    walk() {
        let dir             =   this.directions[this.direction_i];
        
        let dir_array       =   this.get_dir_as_array( dir );
        
        this.pos[0]         =   this.pos[0] + dir_array[0];
        this.pos[1]         =   this.pos[1] + dir_array[1];
        
        //console.error( '%s. %s', this.pos, dir );
        return ( this.output_dir( dir ) );
    }

    output_dir( dir ) {
        switch( dir ) {
            case 's':
                return('SOUTH');
            case 'e':
                return('EAST');
            case 'n':
                return('NORTH');
            case 'w':
                return('WEST');
        }
    }


    get_dir_as_array( dir ) {
        switch ( dir ) {
            case 's':
                return  [1,0];
                
            case 'e':
                return  [0,1];
                    
            case 'n':
                return  [-1,0];
                
            case 'w':
                return  [0,-1];

            default:
                console.error('direction not recognized');
                break;
        }
        
    }

    teleport() { 
        if ( this.pos[0]        ==  teleport_a[0]    &&
             this.pos[1]        ==  teleport_a[1] ) {
            console.error( 'teleporting to b: %s from: %s', teleport_b, this.pos );

            this.pos[0]        =   teleport_b[0];
            this.pos[1]        =   teleport_b[1];

        } else { 
            console.error( 'teleporting to a: %s from: %s', teleport_a, this.pos );

            this.pos[0]        =   teleport_a[0];
            this.pos[1]        =   teleport_a[1];
            
        }
    }

    invert() {
        this.is_inverted    = ( this.is_inverted == false ) ? true : false;
        console.error( 'is inverted? %s', this.is_inverted )
    }

    overload() {
        this.is_overloaded  = ( this.is_overloaded == false ) ? true : false;
        console.error( 'is drunk? %s', this.is_overloaded )
    }

}

//============== start of program

var inputs          =   readline().split(' ');
const HEIGHT        =   parseInt(inputs[0]);
const WIDTH         =   parseInt(inputs[1]);

const max_steps     =   200000;

var start_pos       =   [];

var teleport_a      =   [];
var teleport_b      =   [];

var grid            =   [];

var bender;

for (let i = 0; i < HEIGHT; i++) {
    const row               =   readline();
    grid[i]                 =   [];
    for (let j = 0; j < WIDTH; j++) {
    let cell                =   row[j];
        if ( cell   ==  "@" ){
            start_pos       =   [i, j];
        }

        if ( cell   ==  "T" ){
            if ( typeof teleport_a == 'undefined' || teleport_a.length == 0 ) {
                teleport_a   =   [i, j];
               
                console.error( 'teleport a found: %s', teleport_a );
            } else {
                teleport_b   =   [i, j];

                console.error( 'teleport b found: %s', teleport_b );
            }
        }

        grid[i][j]          =   cell;
        
    }
}

if ( check_teleports() == false )
    return;

//============== debugging =====================
for (let i = 0; i < HEIGHT; i++) {
    let a = '';
    for (let j = 0; j < WIDTH; j++) {
        a += grid[i][j];
    }
    console.error( a );
}

    

bender = new Bender( start_pos );

let n = 0; 
let output = '';

while ( n < max_steps ) {
    let action = bender.main();

    if ( action == 'done' ){
        break;
    } else if ( action == 'LOOP' ) {
        output = action;
        break;
    } else if ( action != '' ) {
        output += action + '\n';
    }
    
    n++;
}

if ( n >= max_steps ) {
    console.log("LOOP");
} else {
    console.log(output);
}

// Write an action using console.log()
// To debug: console.error('Debug messages...');

// console.log('answer');

function check_teleports(){
    if ( typeof teleport_a != 'undefined' && teleport_a.length > 0 ) {
        if ( typeof teleport_b != 'undefined' && teleport_b.length > 0 ) {
            console.error( 'teleports!' );
            console.error( 'teleports a: %s and b: %s', teleport_a, teleport_b );
            return true;
        } else {
            console.error( '1 teleport? :/' );
            return false;
        }
    }
    console.error( 'no teleports' );
    return true;
}

