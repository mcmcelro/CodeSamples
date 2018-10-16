# CodeSamples

Sample code for various purposes.

## SudokuSolver

A universal Sudoku puzzle solver, written in C# (using features in C# 7.0). If a puzzle is solvable, the solution will be saved in \<filename\>.sln.txt; if not, \<filename\>.sln.txt will contain a message stating as much.

## SudokuSolverJava

Java port of SudokuSolver. Requires Java 8 or later. An interesting note: I attempted to imitate C#'s Linq features by initially using 'Arrays.stream' to generate IntStreams to use the 'map' and 'anyMatch' functions, but there was a serious perfomance hit; solving a single puzzle that way took several minutes, as opposed to less than a second for an int[][] structure.
