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
        protected string initialValues;

        // is this a valid solution?
        public abstract bool isValid();

        // is this valid so far as it has been completed?
        public abstract bool isPartiallyValid();

        // does the section have child sections?
        protected abstract bool hasChildren();

        // reset to initial values
        protected abstract void resetToStart();
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

        // constructor with a 9-character input string indicating starting values
        public SudokuSubsection(string sectionValues)
        {
            initialValues = sectionValues;
            resetToStart();
        }

        // is this a valid solution? i.e., no repeats and fully completed
        public override bool isValid()
        {
            return values.Where(val => val != 0).Distinct().Count() == 9;
        }

        // is this valid so far as it has been completed? i.e., no repeats among non-zeroes
        public override bool isPartiallyValid()
        {
            return values.Where(val => val != 0).Distinct().Count() == values.Where(val => val != 0).Count();
        }

        // does the section have child sections? should always be false
        protected override bool hasChildren()
        {
            return false;
        }

        // reset to initial values
        protected override void resetToStart()
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

        // default constructor with no input
        public SudokuBoard()
        {
            tiles = new List<SudokuSection>();
            for (int i = 0; i < 9; i++)
            {
                tiles.Add(new SudokuSubsection());
            }
        }

        // constructor with a filename from which to load values
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

        // is this a valid solution? i.e., all subsections valid
        public override bool isValid()
        {
            bool isGood = true;
            foreach (var tile in tiles)
            {
                isGood = isGood && tile.isValid();
            }
            return isGood;
        }


        // is this valid so far as it has been completed? i.e., all subsections partially valid
        public override bool isPartiallyValid()
        {
            bool isGood = true;
            foreach (var tile in tiles)
            {
                isGood = isGood && tile.isPartiallyValid();
            }
            return isGood;
        }

        // does the section have child sections? should always be true
        protected override bool hasChildren()
        {
            return true;
        }

        // reset to initial values
        protected override void resetToStart()
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
