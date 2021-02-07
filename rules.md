# Protista  
## Grid  
The board is a hexagonal "beehive" grid, with rows oriented left to right.  
Like this:  
```
/\ /\ /\ /\ /\ /\ /\ /\ /\
  |  |  |  |  |  |  |  |  |
\/ \/ \/ \/ \/ \/ \/ \/ \/
|  |  |  |  |  |  |  |  |  |
/\ /\ /\ /\ /\ /\ /\ /\ /\
  |  |  |  |  |  |  |  |  |
\/ \/ \/ \/ \/ \/ \/ \/ \/
```
The number of rows is greater than the number of columns.  
## Initial Position  
Either by random (by drawing cards), or set up in such a way that there are not too many configurations of each type (something like a checkers starting position).

## To Win  
Three target squares are chosen at random at the start, probably by drawing cards. First to occupy two out of three enemy targets wins.  
Alternative: designate a 'king' playing piece on each side, first team to capture to enemy's king wins.

## Moving
### Turn Structure
- The two contestants alternate turns
- Each turn has one or more moves
- Each move will involve moving either one piece, or moving entire blocks of pieces
- Blocks of pieces with different shapes will have different possible moves, as described below
- If playing a move completes a loop of pieces, then the player gets an additional move on the same turn
- Completing a loop means that a new loop of pieces (of the same color) is created; it must have a hole in the center which is either blank or contains pieces belonging to the opponent.
  - Note that if in the course of a move, one loop is broken but another one is created, that counts as creating a new loop, as long as the pieces along the "hole" are not exactly the same as the previous ones
- If you create more than one closed loop in a single move, you get *two* (and only two) extra moves
  - These do not accumulate
  - If the first move of those two extra moves creates two more moves, you only get those two more moves and not three (those two plus the one you had from before)
### Taking/Killing Pieces
- Any move of multiple steps terminates as soon as it hits a piece of the same color, or the player making the move decides to stop.  
  - Alternative: Until that point, it must continue as stated in the moves for that configuration.

- In the course of a move, if the player's pieces land on cells occupied by the opponent, the opponent's pieces are removed

### Possible Moves:

1. Individual single pieces can move one step at a time, to an adjacent cell

2. If you have a set of pieces of your color that are contiguous (that is: you can move from any of these pieces to any other by stepping only in cells that are occupied by your own color), then any of the pieces in this set can move to an empty square touching any of the pieces in that set. 

3. If you have a straight row of pieces and nothing else of the same color is touching it it's called a "cannon".  The end piece can move a number of steps equal to the number of pieces in the row (before the move).
   ```
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |x |x |x |  |  |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\

   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |x |x |  |  |  |x |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
   ```
   The end piece has moved three steps, because the length of the cannon was three pieces long.  Any enemy pieces in those three locations will be killed.

4. Zigzag shapes can move parallel to themselves, in any direction.  We call this a "wave".
   ```
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |y |y |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |x |x |y |y |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |x |x |y |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |  |  |x |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |  |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   ```
   In particular, a simple "V" shape with just three elements can move parallel to itself, in any direction (`x -> y` or `y-> x`).
   ```
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |y |y |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |x |x |y |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |x |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   ```
   Note what happens when you have two zigzag shapes facing each other that are both five pieces long: you have *two* loops.  So you can keep moving them until you hit a piece of your own color, or hit the edge, or hit a target cell.  
   Or: if you have a simple loop, of six pieces, you can consider it to be two "V"s.  So it can move twice as fast as it normally would: move one v, move the other v to create a loop and get an extra turn, move the first v, repeat.

5. If you have a sharp "V" of pieces, then the vertex piece can move as many steps as there are pieces in either leg.  These steps are *not* to adjacent cells, they are to cells connected.  Notice the piece marked "y" in these two diagrams:
   ```
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |  |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |  |  |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |  |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |  |  |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |  |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |  |y |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |x |x |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |x |  |x |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |x |  |  |x |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   ```
   to
   ```
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |  |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |  |  |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |  |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |  |y |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |  |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |  |  |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |  |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |  |  |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |  |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |  |  |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |x |x |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  |x |  |x |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |x |  |  |x |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   ```

6. Pieces can be generated: if you have two pieces in a line separate by three cells, you can complete the equilateral triangle on both sides in one move.  That is, if you don't have pieces of your color on those squares, you can place them there as a move.  
  Alternate: consider "fill the cup"; or any empty cell surrounded by four of your color; or numbers other than three.  Need to consider the implications -- does it lead to complex behavior?  Does it make winning too easy?
   ```
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |x |x |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   |  |  |  | y |x |  |  |  |  |
   /\ /\ /\ /\ /\ /\ /\ /\ /\
     |  |  |  |  |  |  |  |  |
   \/ \/ \/ \/ \/ \/ \/ \/ \/
   ```
7. A player can move one of their pieces onto another of their pieces (stacking them, similar to a king in checkers).  This can only be done by the first two kinds of moves (move a piece to an adjacent cell, or move a piece adjacent to the contiguous set of pieces).  This stacked piece behaves exactly as an ordinary piece for purposes of moving.  However, any move by an opponent stops its steps when it comes to one of these cells -- it stops at a step just before it would hit it.  When this happens, the stacked piece loses one of the stacks (alternative: loses two).

8. If a piece has been stacked, it can be unstacked in one move.  For example, if you have a piece that is a stack of four in a cell, then in one move, you can change it to a line of four pieces, with one end at the original cell.

## Difference Between a Step, a Move, and a Turn:
- Each movement of a piece from one hex to an adjacent cell is one step
- The set of all steps counts as one move
- If the move creates a new loop, then the player gets an extra move
- The set of all these moves played consecutively by one person constitutes that person's turn
  