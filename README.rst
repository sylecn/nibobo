nibobo solver
================================

The game
-------------------
nibobo (or nibobo pyramid) is a board game for children. You may purchase it on jd.com or taobao.com.

The game have a triangle board of half of size 10x10 and 12 pre-built blocks.
A puzzle is given by putting some blocks on the board and the goal is to fit all other blocks on the board, leaving no space on the board.

See puzzle example in GameBoardInRealWorld.jpg

The Solver
--------------------
This solver use a basic depth first search to find (all) solutions for a given puzzle.
It rotates the board 90 degreen to the left, which make the board half of a 10x10 rectangle.
Then it tries to fit blocks to the first empty cell couting from top to bottom, left to right.
When all pieces are fit and the board is full, the puzzle is solved.

The GUI
--------------------
To run the GUI, build the project in Visual Studio or download .NET CORE SDK and run "dotnet run" in nibobo directory.

See GUI example in PuzzleExample01.png

You may generate a puzzle manually, click "Manual Generate" button.
Click cells on puzzle board to "draw" a block, then click "Next Block" to place the block and "draw" next block.
When all blocked placed, click "Finish Placement".
To undo last block placement, click "Remove last block".

To solve the puzzle, click "Solve Puzzle". To view all solutions, click "Next Solution" and/or "Prev Solution".

ChangeLog
---------------------

v1.0.1
- Added README.rst file.
- Added game board picture and game GUI picture.
- published on github.
