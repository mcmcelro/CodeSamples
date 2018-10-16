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
                string inputFileName = args[0];
                string outputFileName = Path.GetFileNameWithoutExtension(inputFileName) + ".sln.txt";
                SudokuBoard boardToSolve = SudokuBoard.LoadSudokuBoardFromFile(inputFileName);
                boardToSolve.solveBoard();
                boardToSolve.writeSolution(outputFileName);
                if (boardToSolve.isSolved())
                {
                    Console.WriteLine("Solving complete.");
                }
                else
                {
                    Console.WriteLine("Board is not solvable.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }

}
