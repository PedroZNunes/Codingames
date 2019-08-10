/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/


class Hero {

    constructor( position, direction, wall_side ) {
        this.pos = [];
        this.pos['x'] = position[0];
        this.pos['y'] = position[1];

        this.dir = direction;
        this.wall_side = wall_side;
    }

    get_position() {
        var output = '(' + this.pos['x'] + ', ' + this.pos['y'] + ')';
        return (output);
    }

    check_path() {
        console.error( 'checking path. starting from: \nPosition: %s \nDirection: %s \n', this.get_position(), this.dir );
        // fazer o ai do boneco e atualizar o grid a cada passo do boneco
        if ( this.check_for_wall( wall_side ) ) {
            console.error( 'Wall to the correct side, %s', wall_side );

            if ( ! this.check_for_wall( ) ) {
                console.error( 'No wall ahead' );
                this.walk();
            }
            else { //wall ahead
                console.error( 'Wall ahead. Turning %s', opposite_side );
                this.turn( opposite_side );
            }
        } else { 
            console.error( 'No wall to follow. Turning %s and walking', opposite_side );
            this.turn( wall_side );
            this.walk();
        }
    }
    
    check_for_wall( side = 'f' ) {
        var wall_abs_direction;
        var wall_abs_direction_vector;

        var next_pos;

        if ( side === 'f' ) {
            wall_abs_direction = this.dir;
        } else {
            wall_abs_direction = this.turn( side, false );
        }

        wall_abs_direction_vector = get_direction_vector( wall_abs_direction );
        
        next_pos = [];

        next_pos['x'] = this.pos['x'] + wall_abs_direction_vector[0];
        next_pos['y'] = this.pos['y'] + wall_abs_direction_vector[1];

        console.error( 'checking for a wall to the %s of (%s, %s). Destination: (%s, %s)', wall_abs_direction, this.pos['x'], this.pos['y'], next_pos['x'], next_pos['y'] );

        // out of bounds
        if (    (next_pos['x']  >=  width   )   ||
                (next_pos['x']  <   0       )   ||
                (next_pos['y']  >=  height  )   ||
                (next_pos['y']  <   0       )
        ) {
            console.error( 'wall found. out of bounds' );
            return true;
        }

        if ( grid[next_pos['x'], next_pos['y']] === '#' ) {
            console.error( 'wall found. real one' );

            return true;
        } else {
            console.error( 'wall NOT found.' );

            return false;
        }
    }

    turn( rotation_dir, actually_move = true ) {
        var dir = this.dir.toLowerCase();
        
        rotation_dir = (rotation_dir) ? rotation_dir.toLowerCase() : '';

        if ( rotation_dir == dir ){
            return dir;
        }

        if ( rotation_dir === null || typeof( rotation_dir ) !== 'string' ){
            console.error( 'rotation_dir: %s \ntypeof: %s', rotation_dir, typeof( rotation_dir ) );
        }

        if ( rotation_dir === 'l' ) {
            switch ( dir ) {
                case 'w':
                    dir = 's';    
                break;
                case 's':
                    dir = 'e';
                break;
                case 'e':
                    dir = 'n';
                break;
                case 'n':
                    dir = 'w';
                break;

                default:
                    console.error( 'Direction not recognized. turn function. direction: %s', dir );
                break;
            }
        } else {
            switch ( dir ) {
                case 'w':
                    dir = 'n';    
                break;
                case 'n':
                    dir = 'e';
                break;
                case 'e':
                    dir = 's';
                break;
                case 's':
                    dir = 'w';
                break;

                default:
                    console.error( 'Direction not recognized. turn function.' );
                break;
            }
        }

        if ( actually_move === true )
            console.error( 'Turned %s, now facing %s', rotation_dir, dir );

        if ( actually_move !== false ){
            this.dir = dir;
        }

        return dir;
    }

    walk( ) {
        var direction = this.dir.toLowerCase();
        var direction_vector = get_direction_vector( direction );
        
        var to_position = [];
        to_position['x'] = this.pos['x'] + direction_vector[0];
        to_position['y'] = this.pos['y'] + direction_vector[1];

        to_position['x'] = to_position['x'];
        to_position['y'] = this.pos['y'];
        console.error( 'current Position: %s', this.get_position() );
        console.error( 'trying to walk towards position: (%s, %s)', to_position['x'].toString(), to_position['y'].toString() );

        if ( (
            to_position['x']    < 0         ||
            to_position['x']    >=  width   ||
            to_position['y']    < 0         ||
            to_position['y']    >=  height ) 
            ||
            (grid[to_position['x'], to_position['y']] === '#' ) ) {
            console.error( 'Tried walking inside a wall or outside of the map. \nDirection: %s \npos_final: (%s, %s)', direction, to_position['x'], to_position['y'] );
        }
        else{
            console.error( 'Walking' );
            
            this.pos = to_position;
            
            grid[this.pos['x'], this.pos['y']]++;
        }
    }
}







var inputs = readline().split(' ');
const width = parseInt(inputs[0]);
const height = parseInt(inputs[1]);

var grid = [,]; //look for ways of recording maps... 

var ini_pos;
var ini_dir;

var wall_side;
var opposite_side;

for ( let y = 0; y < height; y++ ) {
    const row = readline();
    console.error(row);
    for ( let x = 0; x < width; x++ ) {
        if (    row[x] == '<'   ||
                row[x] == '^'   ||
                row[x] == 'v'   ||
                row[x] == '>'   ){
            ini_pos = [x, y];

            switch ( row[x] ) {
                case '<':
                    ini_dir = 'w';
                break;

                case '^':
                    ini_dir = 'n';
                break;
                
                case '>':
                    ini_dir = 'e';
                break;

                case 'v':
                    ini_dir = 's';
                break;
                    
                default:

                break;
            }

            grid[x, y] = 1;
            
        } else {
            grid[x, y] = row[x];
        }
    }
}
console.error('');

// outputting grid

for ( let y = 0; y < height; y++ ) {
    let row = '';
    for ( let x = 0; x < width; x++ ) {
        console.error('x: %i, y: %i', x, y);
        row += grid[x, y].toString();
    }
    console.error(row);
}
console.error('');


const line = readline();
wall_side = line[0];
opposite_side = ( ( wall_side.toLowerCase == 'l' ) ? 'r' : 'l');

hero = new Hero( ini_pos, ini_dir, wall_side );

do {
    hero.check_path();
} while( this.pos['x'] !== ini_pos[0]   &&  this.pos['y'] !== ini_pos[1]);

console.log( 'after main' );

// outputting 
for ( let y = 0; y < height; y++ ) {
    console.error( '\n' );        
    var row = '';
    for ( let x = 0; x < width; x++ ) {
        console.error( grid[x, y] );        
    }
    console.log( row.toString() );
}

for ( let y = 0; y <= height; y++ ) {
    var row = '';
    for ( let x = 0; x <= width; x++ ) {
        row += grid[x, y];
    }
    console.log( row.toString() );
}

//======================= end of main

function get_direction_vector( dir ) {
    dir = dir.toLowerCase();
    movement = [];
    switch( dir.toLowerCase() ) {
        case 'w':
            movement = [-1, 0];
            break;
    
        case 'n':
            movement = [0, -1];
            break;

        case 'e':
            movement = [1, 0];
            break;
        
        case 's':
            movement = [0, 1];
            break;

        default:
            break;
    }
    return movement;
}

