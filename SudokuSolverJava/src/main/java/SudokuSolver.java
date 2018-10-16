import java.io.*;
import java.nio.file.Files;
import java.nio.file.Paths;


public class SudokuSolver {

    public static void main(String[] args) {
        SudokuSolver s = new SudokuSolver();
        if (args.length < 1) {
            System.out.println(String.format("Syntax: java %s <input file>", s.getClass().getSimpleName()));
            return;
        }
        try {
            SudokuBoard boardToSolve = new SudokuBoard(args[0]);
            boardToSolve.solveBoard();
            System.out.println("Solving complete.");
        } catch (Exception ex) {
            System.err.println(ex.toString());
        }
    }
}

class SudokuBoard {
    int[][] values;
    String outputFileName;

    public SudokuBoard(String boardFileName)
            throws FileNotFoundException, IOException, Exception
    {
        values = new int[9][];
        outputFileName = boardFileName.substring(0, boardFileName.lastIndexOf(".")) + ".sol.txt";
        String initialValues = String.join("", Files.readAllLines(Paths.get(boardFileName))).replaceAll("[\r\n]", "");
        if(initialValues.length() < 81) {
            throw new Exception(String.format("Invalid board size: %s (should be 81)", initialValues.length()));
        }

        for(int row = 0; row < 9; row++)
        {
            values[row] = new int [9];
            for(int column = 0; column < 9; column++)
            {
                char thisVal = initialValues.charAt(9 * row + column);
                if("123456789X".indexOf(thisVal) < 0)
                {
                    throw new Exception(String.format("Invalid character found on board: %c", thisVal));
                }
                values[row][column] = thisVal == 'X' ? 0 : thisVal - '0';
            }
        }
    }

    // see if a row contains a number
    private boolean rowContainsNumber(int row, int number)
    {
        for(int column = 0; column < 9; column++)
        {
            if(values[row][column] == number) return true;
        }
        return false;
    }

    // see if a column contains a number
    private boolean columnContainsNumber(int column, Integer number)
    {
        for(int row = 0; row < 9; row++)
        {
            if(values[row][column] == number) return true;
        }
        return false;
    }

    // see if <number> is used in a 3x3 section starting at [sectionStartingRow][sectionStartingColumn]
    private boolean sectionContainsNumber(int sectionStartingRow, int sectionStartingColumn, Integer number)
    {
        for(int row = sectionStartingRow; row < sectionStartingRow + 3; row++)
        {
            for (int column = sectionStartingColumn; column < sectionStartingColumn + 2; column++) {
                if(values[row][column] == number) return true;
            }
        }
        return false;
    }

    // see if [row][column] can legally hold <number>
    private boolean canSpaceHoldNumber(int row, int column, Integer number)
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
    private int[] getEmptySpace(int[][] values)
    {
        for (int row = 0; row < 9; row++)
            for (int column = 0; column < 9; column++)
                if (values[row][column] == 0) return new int[]{row, column};

        return new int[] {-1, -1};
    }

    public void solveBoard()
    throws IOException
    {
        try (FileWriter outputFile = new FileWriter(outputFileName, false))
        {
            if (solve(values))
            {
                for (int row = 0; row < 9; row++)
                {
                    for (int column = 0; column < 9; column++)
                    {
                        outputFile.write(values[row][column] + "") ;
                    }
                    outputFile.write("\r\n");
                }
            }
            else
            {
                outputFile.write("Board is not solvable.\r\n");
            }
        }
    }

    private boolean solve(int[][] values)
    {
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
            int[] firstEmptySpace = getEmptySpace(values);
            if (firstEmptySpace[0] == -1 && firstEmptySpace[1] == -1) return true;
            for (int valToTest = 1; valToTest <= 9; valToTest++)
            {
                if (canSpaceHoldNumber(firstEmptySpace[0], firstEmptySpace[1], valToTest))
                {
                    values[firstEmptySpace[0]][firstEmptySpace[1]] = valToTest;
                    if (solve(values))
                        return true;
                    values[firstEmptySpace[0]][firstEmptySpace[1]] = 0;
                }
            }
            return false;
        }
    }
}