using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine($"Syntax: {Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location)} <input file>");
                return;
            }

            SudokuBoard boardToSolve = new SudokuBoard(args[0]);
        }
    }

    abstract class SudokuSection
    {
        internal abstract bool isValid();
        internal abstract bool hasChildren();
    }

    class SudokuSubsection : SudokuSection
    {
        int[] values;

        public SudokuSubsection()
        {

        }

        internal override bool isValid()
        {
            return values.Where(val => val != 0).Distinct().Count() == 9;
        }

        internal override bool hasChildren()
        {
            return false;
        }
    }

    class SudokuBoard : SudokuSection
    {
        List<SudokuSection> tiles;

        public SudokuBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                tiles.Add(new SudokuSubsection());
            }
        }

        public SudokuBoard(string boardFile)
        {
            try
            {
                string boardData = new StreamReader(File.Open(boardFile, FileMode.Open)).ReadToEnd();
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("Could not open board file for input.");
            }
        }

        internal override bool isValid()
        {
            bool isGood = false;
            foreach (var tile in tiles)
            {
                isGood = isGood && tile.isValid();
            }
            return isGood;
        }

        internal override bool hasChildren()
        {
            return true;
        }
    }
}
