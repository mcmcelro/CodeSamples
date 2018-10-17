using System;
using System.IO;
using System.Linq;

namespace SudokuSolver
{
    public class SudokuBoard
    {
        private int[][] values;
        private bool solved;
        private bool loaded;

        public bool isSolved()
        {
            return this.solved;
        }

        public bool isLoaded()
        {
            return this.loaded;
        }

        public static SudokuBoard LoadSudokuBoardFromFile(string boardFileName)
        {
            int[][] boardData = new int[9][];
            string initialValues = new StreamReader(File.OpenRead(boardFileName)).ReadToEnd().Replace("\n", "").Replace("\r", "");
            if (initialValues.Length != 81) throw new InvalidDataException($"Invalid board size: {initialValues.Length} (should be 81)");

            for (int row = 0; row < 9; row++)
            {
                boardData[row] = new int[9];
                for (int column = 0; column < 9; column++)
                {
                    char thisVal = initialValues[9 * row + column];
                    if ("123456789X".IndexOf(thisVal) < 0)
                        throw new InvalidDataException($"Invalid character found on board: {thisVal}");
                    boardData[row][column] = thisVal == 'X' ? 0 : thisVal - '0';
                }
            }
            return new SudokuBoard(boardData);
        }

        public SudokuBoard(int[][] boardData)
        {
            // make sure it's 9x9 in case it wasn't loaded from a file
            if (boardData.Length == 9)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (boardData[row].Length != 9) return;
                }
                this.values = (int[][])boardData.Clone();
                this.loaded = true;
            }
        }

        // see if a row contains a number
        private bool rowContainsNumber(int row, int number)
        {
            return this.values[row].Contains(number);
        }

        // see if a column contains a number
        private bool columnContainsNumber(int column, int number)
        {
            return this.values.Select(row => row[column]).Contains(number);
        }

        // see if <number> is used in a 3x3 section starting at [sectionStartingRow][sectionStartingColumn]
        private bool sectionContainsNumber(int sectionStartingRow, int sectionStartingColumn, int number)
        {
            for (int row = sectionStartingRow; row < sectionStartingRow + 3; row++)
            {
                for (int column = sectionStartingColumn; column < sectionStartingColumn + 3; column++)
                {
                    if (this.values[row][column] == number) return true;
                }
            }
            return false;
        }

        // see if [row][column] can legally hold <number>
        private bool canSpaceHoldNumber(int row, int column, int number)
        {
            // if the row does not contain the number...
            return !rowContainsNumber(row, number)
            // and the column does not contain the number...
            && !columnContainsNumber(column, number)
            // and the 3x3 section that the square falls in does not contain the number...
            && !sectionContainsNumber(row - row % 3, column - column % 3, number);
            // you can put the number in this space!
        }

        // find the first empty space; returns (row: -1, column: -1) if none are found
        private (int row, int column) getEmptySpace()
        {
            for (int row = 0; row < 9; row++)
                for (int column = 0; column < 9; column++)
                    if (this.values[row][column] == 0) return (row: row, column: column);

            return (-1, -1);
        }

        public void solveBoard()
        {
            this.solved = solveRecursively();
        }

        private bool solveRecursively()
        {
            // methodology:
            // 1. find the first empty square on the board
            // 2. loop through numbers 1-9 to see which one can be placed there
            // 3. if a number can be placed, place it and pass the new board to a
            //    recursive call to the solver function
            // 4. if a number can't be placed, wipe the number back out and step back to the
            //    parent call, proceeding to the next potential solution 1-9
            // 5. if we get all the way through 9 on the first empty space and can't place a number,
            //    it's an invalid board and there is no solution
            var firstEmptySpace = getEmptySpace();
            if (firstEmptySpace.row == -1 && firstEmptySpace.column == -1) return true;
            for (int valToTest = 1; valToTest <= 9; valToTest++)
            {
                if (canSpaceHoldNumber(firstEmptySpace.row, firstEmptySpace.column, valToTest))
                {
                    this.values[firstEmptySpace.row][firstEmptySpace.column] = valToTest;
                    if (solveRecursively()) return true;
                    this.values[firstEmptySpace.row][firstEmptySpace.column] = 0;
                }
            }
            return false;
        }
        public void writeSolution(string outputFileName)
        {
            using (StreamWriter outputFile = new StreamWriter(File.OpenWrite(outputFileName)))
            {
                outputFile.BaseStream.SetLength(0);
                if (this.solved)
                {
                    for (int row = 0; row < 9; row++)
                    {
                        for (int column = 0; column < 9; column++)
                        {
                            outputFile.Write(this.values[row][column] + "");
                        }
                        outputFile.Write("\r\n");
                    }
                }
                else
                {
                    outputFile.WriteLine("Board is not solvable.");
                }
            }
        }
    }
}
