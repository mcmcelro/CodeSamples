import java.nio.file.Paths
import org.junit.Assert
import org.junit.Test

class SudokuBoardTest extends groovy.util.GroovyTestCase {

    @Test
    String getPathForTestFile(String testFileName) {
        return Paths.get(this.getClass().getClassLoader().getSystemResource(testFileName).toURI())
    }

    // test for when the board is properly formatted and solvable
    @Test
    void testLoadGoodFile() {
        try {
            SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile(getPathForTestFile("good-board.txt"))
            // board should not be null
            Assert.assertNotNull("testLoadGoodFile: testBoard is null", testBoard)
            testBoard.solveBoard()
            // board should be solved
            Assert.assertTrue("testLoadGoodFile: board was not solved successfully", testBoard.isSolved())
        }
        catch (Exception ex) {
            Assert.fail("testLoadGoodFile: board was not solved successfully")
        }
    }

    // tests for when the board is directly input via code and solvable
    @Test
    void testSolveDirectInputBoard() {
        try {
            // direct input equivalent of puzzle5.txt
            int[][] boardData = [[0, 0, 0, 0, 0, 0, 0, 0, 7],
                                 [0, 4, 8, 6, 0, 2, 1, 5, 9],
                                 [0, 6, 0, 0, 0, 0, 0, 8, 0],
                                 [0, 9, 0, 8, 0, 4, 0, 3, 0],
                                 [0, 0, 0, 0, 2, 0, 0, 0, 0],
                                 [0, 2, 0, 9, 0, 5, 0, 6, 0],
                                 [0, 7, 0, 0, 0, 0, 0, 9, 0],
                                 [1, 8, 3, 2, 0, 7, 5, 4, 0],
                                 [2, 0, 0, 0, 0, 0, 0, 0, 0]]
            SudokuBoard testDirectBoard = new SudokuBoard(boardData)
            // board should not be null
            Assert.assertTrue("testSolveDirectInputBoard: testDirectBoard is not loaded", testDirectBoard.isLoaded())
            testDirectBoard.solveBoard()
            // board should be solved
            Assert.assertTrue("testSolveDirectInputBoard: board was not solved successfully", testDirectBoard.isSolved())

            // test with bad board
            boardData = [[]]
            testDirectBoard = new SudokuBoard(boardData)
            Assert.assertFalse("testSolveDirectInputBoard: testDirectBoard was loaded successfully from bad data", testDirectBoard.isLoaded())

        }
        catch (Exception ex) {
            Assert.fail("testSolveDirectInputBoard: board was not solved successfully")
        }
    }

    // test for writing the solution to a file
    @Test
    void testWriteSolution() {
        try {
            // clean up existing output file - does not throw any exceptions if the file is already deleted
            new File("testWriteSolution-output.txt").delete()

            SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile(getPathForTestFile("good-board.txt"))
            // board should not be null
            Assert.assertNotNull("testWriteSolution: testBoard is null", testBoard)
            testBoard.solveBoard()
            // board should be solved
            Assert.assertTrue("testWriteSolution: board was not solved successfully", testBoard.isSolved())
            testBoard.writeSolution("testWriteSolution-output.txt")
            // testWriteSolution-output.txt should exist
            Assert.assertTrue("testWriteSolution: board solution was not written successfully", new File("testWriteSolution-output.txt").exists())
        }
        catch (Exception ex) {
            Assert.fail("testWriteSolution: board solution was not written successfully")
        }
    }

    // test for when the board file is missing
    @Test
    void testLoadMissingFile() {
        try {
            SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile(getPathForTestFile("nonexistent.txt"))
            // board should be null
            Assert.assertNull("testLoadMissingFile: testBoard is not null", testBoard)
            testBoard.solveBoard()
            // if somehow not null, board should not be solvable
            Assert.assertFalse("testLoadMissingFile: board from missing file was somehow solved", testBoard.isSolved())
        }
        catch (Exception ex) {
            // should throw java.lang.NullPointerException caused by getSystemResource not finding the file
            if (ex.getClass().getTypeName() != "java.lang.NullPointerException" || ex.getMessage() != "Cannot invoke method toURI() on null object") {
                Assert.fail("testLoadMissingFile: unexpected exception attempting to load nonexistent test file")
            }
        }
        try {
            SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("src/test/resources/nonexistent-board-file.txt")
            // board should be null
            Assert.assertNull("testLoadMissingFile: testBoard is not null", testBoard)
            testBoard.solveBoard()
            // if somehow not null, board should not be solvable
            Assert.assertFalse("testLoadMissingFile: board from missing file was somehow solved", testBoard.isSolved())
        }
        catch (Exception ex) {
            // java.nio.file.NoSuchFileException should be thrown
            if (ex.getClass().getTypeName() != "java.nio.file.NoSuchFileException") {
                Assert.fail("testLoadMissingFile: unexpected exception attempting to load nonexistent test file")
            }
        }
    }

    // tests for when the board is too big or too small
    @Test
    void testLoadFileWithWrongSize() {
        try {
            SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("src/test/resources/bad-board-size-80.txt")
            // board should be null
            Assert.assertNull("testLoadFileWithWrongSize: testBoard is not null", testBoard)
            testBoard.solveBoard()
            // if somehow not null, board should not be solvable
            Assert.assertFalse("testLoadFileWithWrongSize: invalid board was somehow solved", testBoard.isSolved())
        }
        catch (Exception ex) {
            // com.sun.media.sound.InvalidFormatException should be thrown
            if (ex.getClass().getTypeName() != "com.sun.media.sound.InvalidFormatException"
                    || ex.getMessage() != "Invalid board size: 80 (should be 81)") {
                Assert.fail("testLoadFileWithWrongSize: exception other than InvalidFormatException occurred with board too small (80 values)")
            }
        }

        try {
            SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("src/test/resources/bad-board-size-82.txt")
            // board should be null
            Assert.assertNull("testLoadFileWithWrongSize: testBoard is not null", testBoard)
            testBoard.solveBoard()
            // if somehow not null, board should not be solvable
            Assert.assertFalse("testLoadFileWithWrongSize: invalid board was somehow solved", testBoard.isSolved())
        }
        catch (Exception ex) {
            // com.sun.media.sound.InvalidFormatException should be thrown
            if (ex.getClass().getTypeName() != "com.sun.media.sound.InvalidFormatException"
                    || ex.getMessage() != "Invalid board size: 82 (should be 81)") {
                Assert.fail("testLoadFileWithWrongSize: exception other than InvalidFormatException occurred with board too big (82 values)")
            }
        }
    }

    // test for when the board has invalid characters
    @Test
    void testLoadFileWithInvalidCharacters() {
        try {
            SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("src/test/resources/bad-board-invalid-characters.txt")
            // board should be null
            Assert.assertNull("testLoadFileWithInvalidCharacters: testBoard is not null", testBoard)
            testBoard.solveBoard()
            // if somehow not null, board should not be solvable
            Assert.assertFalse("testLoadFileWithInvalidCharacters: invalid board was somehow solved", testBoard.isSolved())
        }
        catch (Exception ex) {
            // com.sun.media.sound.InvalidFormatException should be thrown
            if (ex.getClass().getTypeName() != "com.sun.media.sound.InvalidFormatException"
                    || ex.getMessage() != "Invalid character found on board: !") {
                Assert.fail("testLoadFileWithInvalidCharacters: exception other than InvalidFormatException occurred")
            }
        }
    }

    // test for when the board is properly formatted but not solvable
    @Test
    void testSolveUnsolvableBoard() {
        try {
            SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("src/test/resources/bad-board-unsolvable.txt")
            // board should not be null
            Assert.assertNotNull("testSolveUnsolvableBoard: testBoard is null", testBoard)
            testBoard.solveBoard()
            Assert.assertFalse("testSolveUnsolvableBoard: unsolvable board was somehow solved", testBoard.isSolved())
        }
        catch (Exception ex) {
            Assert.fail("testSolveUnsolvableBoard: unexpected exception attempting to load test file")
        }
    }
}
