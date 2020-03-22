# Turtle Challenge

## What I have assumed

1. The format of the the files ([here](#maze-setup) and [here](#maze-moves))
2. The maze 0,0 is bottom-left.
3. Using a graph algo must had better performance but it won't shine F# features like unions, active patterns, partial functions, etc.
4. Steps and grid are printed for better visualization of the logic

## File formats

### Maze setup

It only contains 4 lines:

1. Size of the maze, ie.: `5x4`
2. Start cell indexes and head direction, ie.: `0,1,n`, `0,1,e`
3. End cell index, ie.: `4,2`
4. Bomb positions indexes, ie.: `1,1;3,1;3,3`

All indexes are 0-based

### Maze moves

Only one line:

1. `r` indicates rotation
2. `m` indicates move one tile in the current orientation

## Run

There are two scripts `run.sh` and `run.ps1` to run it in a bash terminal or PowerShell respectively.
Both will run a few scenarios using [small.txt](./small.txt) maze.
It will run the moves:
1. [toExit](./toExit.txt)
2. [toABomb](./toABomb.txt)
3. [toOutBounds](./toOutBounds.txt)
4. [notOut](./notOut.txt)
5. [circle](./circle.txt)

There is also one last run that will use [small-NoEdgeStart.txt](./small-NoEdgeStart.txt) maze and [noEdgeStart-win.txt](./noEdgeStart-win.txt) to show how it can run with a start that is not an edge of the maze.
