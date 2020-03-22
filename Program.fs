open System.IO
open System
open Microsoft.FSharp.Core.ExtraTopLevelOperators

type TileType = N=0 | S=1 | E=2 | B=3
type MoveType = Rotate=1 | Move=2

type TurtleOrientation = | North | South | East | West

type Tile = { X:int; Y:int }
type MazeCellState =
    {
        Position:Tile;
        Orientation: TurtleOrientation option;
    }

type MazeConfiguration =
    {
        Size: Tile;
        Start: MazeCellState;
        End: Tile;
        Bombs: Set<Tile>
    }

type TurtleMove = { MoveTo:Tile; Orientation:TurtleOrientation}

let (|InvariantEqual|_|) (str:string) arg =
  if str.Equals(arg, StringComparison.OrdinalIgnoreCase)
    then Some() else None

let parseMoveType (value:string) =
    match value with
    | InvariantEqual "r" -> MoveType.Rotate
    | InvariantEqual "m" -> MoveType.Move
    | _ -> failwith (sprintf "Invalid Move '%s'" value)


let split (what:string) (useSep:char) = what.Split useSep
let commaSplit (what:string) = split what ','

let resolveOrientation (value:string) =
    match value with
    | InvariantEqual "N" -> North
    | InvariantEqual "S" -> South
    | InvariantEqual "E" -> East
    | InvariantEqual "W" -> West
    | o -> failwith (sprintf "Invalid Orientation '%s'" o)

let tryGetOrientation (cellValues) =
    if Array.length cellValues > 2 then Some (resolveOrientation (Array.head (Array.skip 2 cellValues)))
    else None

let convertToTile (xy) =
    { X= Array.head xy |> int; Y = Array.head (Array.skip 1 xy) |> int }

let convertToCell (cellValues) =
    {
        Position = convertToTile (Array.take 2 cellValues);
        Orientation = tryGetOrientation cellValues
    }

let printMaze (maze:TileType[,]) size =

    printfn "Maze:"
    for x = 0 to size.X - 1 do
        if x = 0 then
            printf "X═%i\t" x
        else
            printf "%i\t" x
    printf "\n"

    for x = 0 to size.X - 1 do
        if x = 0 then
            printf "Y╦═════"
        else
            printf "════════"
    for y = size.Y - 1 downto 0 do
        printf "\n%i| " y
        for x = 0 to size.X - 1 do
            printf "%O \t" maze.[x , y]
    printf "\r\n"

let getMazeConfiguration (mazeFile) =
    use file = File.OpenText mazeFile
    {
        Size = file.ReadLine().Split 'x' |> convertToTile
        Start = file.ReadLine() |> commaSplit |> convertToCell
        End = file.ReadLine() |> commaSplit |> convertToTile
        Bombs = (file.ReadLine().Split ';')
                |> Seq.map (commaSplit >> convertToTile)
                |> set
    }

let getListOfMoves moveFiles =
    use file = File.OpenText moveFiles
    file.ReadLine().Split ',' |> seq

let getNextMove currentState move =
    match (parseMoveType move) with
    | MoveType.Move ->
        let next =
            match currentState.Orientation with
            | North -> ({MoveTo = { X = currentState.MoveTo.X; Y = currentState.MoveTo.Y + 1 }; Orientation = currentState.Orientation})
            | South -> ({MoveTo = { X = currentState.MoveTo.X; Y = currentState.MoveTo.Y - 1 }; Orientation = currentState.Orientation})
            | East -> ({MoveTo = { X = currentState.MoveTo.X + 1; Y = currentState.MoveTo.Y }; Orientation = currentState.Orientation})
            | West -> ({MoveTo = { X = currentState.MoveTo.X - 1; Y = currentState.MoveTo.Y }; Orientation = currentState.Orientation})
        printfn "Moving from [%i,%i] to [%i,%i]" currentState.MoveTo.X currentState.MoveTo.Y next.MoveTo.X next.MoveTo.Y
        next
    | MoveType.Rotate ->
        let next =
            match currentState.Orientation with
            | North -> ({MoveTo = currentState.MoveTo; Orientation = East})
            | South -> ({MoveTo = currentState.MoveTo; Orientation = West})
            | East -> ({MoveTo = currentState.MoveTo; Orientation = South})
            | West -> ({MoveTo = currentState.MoveTo; Orientation = North})
        printfn "Rotating from [%O] to [%O]" currentState.Orientation next.Orientation
        next
    | unknownMove -> failwithf "Unknown Move '%O'" unknownMove

let rec followInstructions isOutOfBounds (maze:TileType[,]) (visited:Set<Tile>) (moves:seq<string>) (current:TurtleMove) =
    let nextMove = Seq.tryHead moves
    let nextMoves = (Seq.tail  moves)
    let moveIt = followInstructions isOutOfBounds maze (visited.Add current.MoveTo) nextMoves

    match nextMove with
    | Some someNextMove ->
        let next = getNextMove current someNextMove
        match (parseMoveType someNextMove) with
        | MoveType.Move ->
            // check the move now before doing it
            // and return whether it is invalid,
            // a bomb, a tile we already visited
            // or the End tile
            if Set.contains next.MoveTo visited then
                "Going in circles (most likely)!!"
            elif isOutOfBounds next.MoveTo then
                "Out of bounds!!"
            elif maze.[next.MoveTo.X, next.MoveTo.Y] = TileType.B then
                "Mine Hit!!"
            elif maze.[next.MoveTo.X, next.MoveTo.Y] = TileType.E then
                "Success!!"
            else
                next |> moveIt
        // if we are just rotating we do not need
        // to check anything
        | MoveType.Rotate -> next |> moveIt
        | unknownMove -> failwithf "Unknown Move '%O'" unknownMove
    | None -> "Still in danger!!"

[<EntryPoint>]
let main argv =
    Console.OutputEncoding <- Text.Encoding.UTF8

    if argv.Length < 2 then
        failwith "Please run as follow: 'dotnet run -- [PATH-TO-MAZE] [PATH-TO-MOVES]'"

    try
        let mazeConfiguration = argv.[0] |> getMazeConfiguration

        let maze = Array2D.create mazeConfiguration.Size.X mazeConfiguration.Size.Y TileType.N
        for bomb in mazeConfiguration.Bombs do
            maze.[bomb.X, bomb.Y] <- TileType.B

        maze.[mazeConfiguration.Start.Position.X, mazeConfiguration.Start.Position.Y] <- TileType.S
        maze.[mazeConfiguration.End.X, mazeConfiguration.End.Y] <- TileType.E

        printMaze maze mazeConfiguration.Size

        let checkBounds c = c.X < 0 || c.Y < 0 || c.X > mazeConfiguration.Size.X || c.Y > mazeConfiguration.Size.Y

        let moves = argv.[1] |> getListOfMoves
        let firstMove = moves |> Seq.head
        let startState = {
            MoveTo = mazeConfiguration.Start.Position;
            Orientation = mazeConfiguration.Start.Orientation.Value;
        }
        printfn "Starting Point: %O" startState

        // the first move just position the turtle in the maze
        // it could be any position really
        // not only an "edge"
        getNextMove startState firstMove
        |> followInstructions checkBounds maze (Set.empty<Tile>.Add mazeConfiguration.Start.Position) (Seq.tail moves)
        |> printfn "%s"
    with
    | e -> failwith e.Message
    0
