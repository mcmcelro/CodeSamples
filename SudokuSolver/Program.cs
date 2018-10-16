using System;
using System.IO;
using System.Reflection;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine($"Syntax: {Path.GetFileName(Assembly.GetEntryAssembly().Location)} <input file>");
                return;
            }

            try
            {
                SudokuBoard boardToSolve = new SudokuBoard(args[0]);
                boardToSolve.solveBoard();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }

    class SudokuBoard
    {
        int[,] values;
        string outputFileName;

        public SudokuBoard(string boardFile)
        {
            values = new int[9, 9];
            outputFileName = Path.GetFileNameWithoutExtension(boardFile) + ".sol.txt";
            string initialValues = new StreamReader(File.OpenRead(boardFile)).ReadToEnd().Replace("\n", "").Replace("\r", "");

            for (var row = 0; row < 9; row++)
            {
                for (var column = 0; column < 9; column++)
                {
                    char thisVal = initialValues[9 * row + column];
                    if ((thisVal < '0' || thisVal > '9') && thisVal != 'X')
                        throw new InvalidDataException($"Invalid character found on board: {thisVal}");
                    values[row, column] = thisVal == 'X' ? 0 : thisVal - '0';
                }
            }
            if (initialValues.Length != 81) throw new InvalidDataException($"Invalid board size: {initialValues.Length} (should be 81)");
        }

        // see if a row contains a number
        private bool rowContainsNumber(int[,] passedBoard, int row, int number)
        {
            for (int column = 0; column < 9; column++)
                if (passedBoard[row, column] == number) return true;

            return false;
        }

        // see if a column contains a number
        private bool columnContainsNumber(int[,] passedBoard, int column, int number)
        {
            for (int row = 0; row < 9; row++)
                if (passedBoard[row, column] == number) return true;

            return false;
        }

        // see if <number> is used in a 3x3 section starting at [sectionStartingRow, sectionStartingColumn]
        private bool sectionContainsNumber(int[,] passedBoard, int sectionStartingRow, int sectionStartingColumn, int number)
        {
            for (int row = sectionStartingRow; row < sectionStartingRow + 3; row++)
            {
                for (int column = sectionStartingColumn; column < sectionStartingColumn + 3; column++)
                {
                    if (passedBoard[row, column] == number) return true;
                }
            }
            return false;
        }

        // see if [row, column] can legally hold <number>
        private bool canSpaceHoldNumber(int[,] passedBoard, int row, int column, int number)
        {
            // if the row does not contain the number...
            return !rowContainsNumber(passedBoard, row, number)
            // and the column does not contain the number...
            && !columnContainsNumber(passedBoard, column, number)
            // and the 3x3 section that the square falls in does not contain the number...
            && !sectionContainsNumber(passedBoard, row - row % 3, column - column % 3, number);
            // you can put the number in this space!
        }

        // find the first empty space; returns (row: -1, column: -1) if none are found
        private (int row, int column) getEmptySpace(int[,] passedBoard)
        {
            for (int row = 0; row < 9; row++)
                for (int column = 0; column < 9; column++)
                    if (passedBoard[row, column] == 0) return (row: row, column: column);

            return (-1, -1);
        }

        public void solveBoard()
        {
            using (StreamWriter outputFile = new StreamWriter(File.OpenWrite(outputFileName)))
            {
                outputFile.BaseStream.SetLength(0);
                if (solve(values))
                {
                    for (int row = 0; row < 9; row++)
                    {
                        for (int column = 0; column < 9; column++)
                        {
                            outputFile.Write(values[row, column] + "");
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

        private bool solve(int[,] passedBoard)
        {
            // methodology:
            // 1. find the first empty square on the board
            // 2. loop through numbers 1-9 to see which one can be placed there
            // 3. if a number can be placed, place it and pass the new board to a
            //    recursive call to the solver function
            // 4. if a number can't be placed, wipe the number back out and return to the last
            //    recursive call to the solver function, proceeding to the next potential solution
            // 5. if we get all the way through 9 on the first empty space and can't place a number,
            //    it's an invalid board and there is no solution
            return false;
        }
    }
}
