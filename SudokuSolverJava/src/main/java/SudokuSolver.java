public class SudokuSolver {

    public static void main(String[] args) {
        SudokuSolver s = new SudokuSolver();
        if (args.length < 1) {
            System.out.println(String.format("Syntax: java %s <input file>", s.getClass().getSimpleName()));
            return;
        }
        try {
            String inputFileName = args[0];
            String outputFileName = inputFileName.substring(0, inputFileName.lastIndexOf(".")) + ".sln.txt";
            SudokuBoard boardToSolve = SudokuBoard.LoadSudokuBoardFromFile(inputFileName);
            boardToSolve.solveBoard();
            boardToSolve.writeSolution(outputFileName);
            if(boardToSolve.isSolved()) {
                System.out.println("Solving complete.");
            }
            else
            {
                System.out.println("Board is not solvable.");
            }
        } catch (Exception ex) {
            System.err.println(ex.toString());
        }
    }
}
