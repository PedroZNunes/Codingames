/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

const   size        =   parseInt(readline());
const   base_light  =   parseInt(readline());

const   grid        =   [];
var     light_grid  =   [];

for ( let i = 0; i < size; i++ ) {
    let row         =   readline();
    row = row.replace(/ /g,'');

    grid[i]         =   [];
    light_grid[i]   =   [];

    for ( let j = 0; j < size; j++ ) {
        grid[i][j]          =   row[j];
        light_grid[i][j]    =   0;

    }
}

//check all grid for candles
for ( let i = 0; i < size; i++ ) {
    let debug_text  =   '';
    for ( let j = 0; j < size; j++ ) {
        const cell  =   grid[i][j];
        debug_text  +=  cell;

        if ( cell === 'C' ) {
            illuminate( i, j );
        }
    }
    console.error( debug_text );
}
console.error( '\n' );

//outputting
unlit_cells     =   0;
for ( let i = 0; i < size; i++ ) {
    let debug_text = '';
    for ( let j = 0; j < size; j++ ) {
        debug_text  +=  light_grid[i][j];
        if ( light_grid[i][j] == 0 ){
            unlit_cells++;
        }
    }
    console.error( debug_text );
}
console.error( '\n' );
console.log( unlit_cells );


function illuminate( pos_i, pos_j ) {
    // enquanto tiver luz
    console.error ( 'iluminating position ( %s, %s)', pos_i, pos_j );

    // faz uma base de 1, depois faz um complemento diminuindo o range de acordo com o current light
    for ( current_light = base_light; current_light > 0; current_light-- ) {
        let range = current_light - 1;
        for ( let i = -range; i <= range; i++ ) {
            for ( let j = -range; j <= range; j++ ) {
                check_i = pos_i + i;
                check_j = pos_j + j;

                if ( check_i    <   0    ||
                     check_i    >=  size ||
                     check_j    <   0    ||
                     check_j    >=  size   ) {
                     continue;
                }

                light_grid[check_i][check_j]++;
            }
        }
    }
}