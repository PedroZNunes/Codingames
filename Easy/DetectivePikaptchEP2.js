/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

 
class Hero {

    constructor( x, y, direction, wall_side ) {
        this.x = x;
        this.y = y;

        this.dir = direction;
        this.wall_side = wall_side;

        this.has_moved = false;
    }

    get_position() {
        var output = '(x:' + this.x + ', y:' + this.y + ')';
        return (output);
    }

    // segue a parede a menos que esteja cercado de paredes ou volte ao ponto inicial.
    main() {
        this.check_path();

        while ( true ) {
            // se voltou para o inicio
            if ( this.has_moved && this.is_home() ) {
                break;
            } 

            // se ta preso no primeiro tile
            if ( ! this.has_moved && this.dir === ini_dir ){
                break;
            }

            hero.check_path();
            console.error( hero.get_position() );
            console.error( '( %s, %s )', ini_pos_x, ini_pos_y);
        }

    }

    check_path() {
        console.error( 'checking path. starting from: \nPosition: %s \nDirection: %s \n', this.get_position(), this.dir );
        // fazer o ai do boneco e atualizar o grid a cada passo do boneco
        // se tem wall pro lado certo
        if ( this.check_for_wall( wall_side ) ) {
            console.error( 'Wall to the correct side, %s', wall_side );
            
            if ( ! this.check_for_wall( ) ) {   // se n tem parede na frente, anda reto
                console.error( 'No wall ahead' );
                this.walk();
            }
            else {  // se tem parede na frente, vira pro lado oposto ao lado que ele tem q grudar na parede
                console.error( 'Wall ahead. Turning %s', opposite_side );
                this.turn( opposite_side );
            }
        } else {    // se n tem parede pro lado q tem q ter, vira pra aquele lado e anda.
            console.error( 'No wall to follow. Turning %s and walking', opposite_side );
            this.turn( wall_side );
            this.walk();
        }
    }
    
    // checka se tem parede pro lado indicado. no caso de nenhum parametro, checkar na frente.
    check_for_wall( side = 'f' ) {
        var wall_abs_direction;
        var wall_abs_direction_vector;

        var pos_to_check_x;
        var pos_to_check_y;

        if ( side === 'f' ) { //se n tem parametro, manda um f de front pra saber que tem q olhar na frente.
            wall_abs_direction = this.dir;
        } else {    // se tem parametro, usa a função de virar pra só olhar pro lado sem movimentar o boneco e pegar a parede.
            wall_abs_direction = this.turn( side, false );
        }

        wall_abs_direction_vector = get_direction_vector( wall_abs_direction );
        
        pos_to_check_x = this.x + wall_abs_direction_vector[0];
        pos_to_check_y = this.y + wall_abs_direction_vector[1];

        console.error( 'checking for a wall to the %s of (%s, %s). Destination: (%s, %s).', wall_abs_direction, this.x, this.y, pos_to_check_x, pos_to_check_y );

        // out of bounds
        if (    (pos_to_check_x     >=  width   )   ||
                (pos_to_check_x     <   0       )   ||
                (pos_to_check_y     >=  height  )   ||
                (pos_to_check_y     <   0       )
        ) {
            console.error( 'wall found. out of bounds' );
            return true;
        }

        if ( grid[pos_to_check_y][pos_to_check_x] === '#' ) {
            console.error( 'wall found. real one' );

            return true;
        } else {
            console.error( 'wall NOT found.' );

            return false;
        }
    }

    // vira pro lado e pode ou não mover o pikachu, dependendo do segundo parâmetro. Isso é útil pra checkar as paredes pro lado.
    turn( rotation_dir, actually_move = true ) {
        let dir = (this.dir) ? this.dir.toLowerCase() : '';
        
        rotation_dir = (rotation_dir) ? rotation_dir.toLowerCase() : '';

        if ( rotation_dir === null || typeof( rotation_dir ) !== 'string' ){
            console.error( 'rotation_dir: %s \ntypeof: %s', rotation_dir, typeof( rotation_dir ) );
        }

        // vira dependendo do lado da rotação, 'l'eft ou 'r'ight
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

        if ( actually_move !== false ){ // se for pra andar, a direção do boneco atualiza
            console.error( 'Turned %s, now facing %s', rotation_dir, dir );
            this.dir = dir;
        }

        return dir;
    }

    walk( ) {
        var direction = this.dir.toLowerCase();
        var direction_vector = get_direction_vector( direction );
        
        var to_x = [];
        var to_y = [];
        to_x = this.x + direction_vector[0];
        to_y = this.y + direction_vector[1];

        console.error( 'current Position: %s', this.get_position() );
        console.error( 'trying to walk towards position: (%s, %s)', to_x.toString(), to_y.toString() );

        if ( (  to_x        <   0           ||
                to_x        >=  width       ||
                to_y        <   0           ||
                to_y        >=  height )    ||
                (grid[ to_y ][ to_x ] === '#' ) ) {
            console.error( 'Tried walking inside a wall or outside of the map. \nDirection: %s \npos_final: (%s, %s)', direction, to_x, to_y );
        }
        else{
            console.error( 'Walking' );
            
            this.x = to_x;
            this.y = to_y;
            
            grid[this.y] [this.x]++;

            this.has_moved  =   true;
        }
    }

    is_home(){
        return( this.x === ini_pos_x && this.y === ini_pos_y);
    }

}







var inputs              =   readline().split(' ');
const width             =   parseInt(inputs[0]);
const height            =   parseInt(inputs[1]);

var grid                =   []; //[][] sets positions for a place in memory. [,] sets 2 values that could be 2 positions, but they dont point to anywhere.

var ini_pos_x;
var ini_pos_y;

var ini_dir;

var wall_side;
var opposite_side;

for ( let y = 0; y < height; y++ ) {
    const row           =   readline();
    console.error( row );

    grid[y]             =   [];

    for ( let x = 0; x < width; x++ ) {
        let cell        =   row[x];

        if ( cell === null ) {
            console.error( 'empty cell' );
            continue;
        }

        if (    cell == '<'   ||
                cell == '^'   ||
                cell == 'v'   ||
                cell == '>'   )
        {
            ini_pos_x   =   x;
            ini_pos_y   =   y;

            switch ( cell ) {
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

            grid[y][x]  =   0;
        } else {
            grid[y][x]  =   cell;
        }
    }
}
console.error('');

// outputting grid
console.error( get_the_grid() );


const line              =   readline();
wall_side               =   line[0];
opposite_side           =   ( ( wall_side.toLowerCase() == 'l' ) ? 'r' : 'l' );

hero                    =   new Hero( ini_pos_x, ini_pos_y, ini_dir, wall_side );

// main
hero.main();

console.error( 'after main' );

console.error(  get_the_grid() );

console.log(    get_the_grid() );


function get_direction_vector( dir ) {
    dir                 =   dir.toLowerCase();
    movement            =   [0,0];
    switch( dir.toLowerCase() ) {
        case 'w':
            movement    =   [-1, 0];
            break;
    
        case 'n':
            movement    =   [0, -1];
            break;

        case 'e':
            movement    =   [1, 0];
            break;
        
        case 's':
            movement    =   [0, 1];
            break;

        default:
            break;
    }
    return movement;
}

function get_the_grid() {
    let out     =   '';
    for ( let y = 0; y < height; y++ ) {
        out             +=  grid[y].join('');
        out             +=  '\n';
    }

    return out;
}


