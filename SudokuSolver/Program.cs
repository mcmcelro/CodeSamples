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

    // abstract class defining functions needed for board sections,
    // which could be either the 3x3 sections or the whole board
    abstract class SudokuSection
    {
        // initial values assigned to section
        internal string initialValues;

        // is this a valid solution?
        internal abstract bool isValid();

        // does the section have child sections?
        internal abstract bool hasChildren();

        // reset to initial values
        internal abstract void resetToStart();
    }

    // class for a 3x3 board section
    class SudokuSubsection : SudokuSection
    {
        int[] values;

        // default constructor with no input
        public SudokuSubsection()
        {
            initialValues = "XXXXXXXXX";
            values = new int[9];
        }

        // constructor with a 9-character string of values
        public SudokuSubsection(string sectionValues)
        {
            initialValues = sectionValues;
            resetToStart();
        }
        // constructor with a 9-character input string indicating starting values

        // is this a valid solution? i.e., no repeats
        internal override bool isValid()
        {
            return values.Where(val => val != 0).Distinct().Count() == 9;
        }

        // does the section have child sections? should always be false
        internal override bool hasChildren()
        {
            return false;
        }

        // reset to initial values
        internal override void resetToStart()
        {
            values = new int[9];
            if(initialValues.Length != 9)
            {
                throw new InvalidDataException($"Invalid tile size {initialValues.Length} detected.");
            }
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    if (initialValues[i] == 'X')
                    {
                        values[i] = 0;
                    }
                    else if (initialValues[i] > '9' || initialValues[i] < '1')
                    {
                        throw new InvalidDataException($"Invalid input character {initialValues[i]} found in board.");
                    }
                    else
                    {
                        values[i] = initialValues[i] - '0';
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new InvalidDataException("Invalid number of values found in a tile.");
            }
        }
    }

    class SudokuBoard : SudokuSection
    {
        List<SudokuSection> tiles;

        public SudokuBoard()
        {
            tiles = new List<SudokuSection>();
            for (int i = 0; i < 9; i++)
            {
                tiles.Add(new SudokuSubsection());
            }
        }

        public SudokuBoard(string boardFile)
        {
            try
            {
                initialValues = new StreamReader(File.OpenRead(boardFile)).ReadToEnd();
                resetToStart();
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine($"Could not open board file for input. Details: {ex.Message}");
            }
            catch (InvalidDataException ex)
            {
                Console.Error.WriteLine(ex.Message);
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

        internal override void resetToStart()
        {
            tiles = new List<SudokuSection>();
            using (StringReader valueLines = new StringReader(initialValues))
            {
                string line;
                while ((line = valueLines.ReadLine()) != null)
                {
                    tiles.Add(new SudokuSubsection(line));
                }
            }
            if (tiles.Count() > 9 || tiles.Count() < 1)
            {
                throw new InvalidDataException("Invalid board input detected.");
            }
        }
    }
}
