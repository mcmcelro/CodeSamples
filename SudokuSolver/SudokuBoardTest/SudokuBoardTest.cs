using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSolver;
using System;
using System.IO;

namespace SudokuBoardTest
{
    [TestClass]
    public class SudokuBoardTest
    {
        // test for when the board is properly formatted and solvable
        [TestMethod]
        public void testLoadGoodFile()
        {
            try
            {
                SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("test files/good-board.txt");
                Assert.IsNotNull(testBoard, "testLoadGoodFile: testBoard is null");
                testBoard.solveBoard();
                // board should be solved
                Assert.IsTrue(testBoard.isSolved(), "testLoadGoodFile: board was not solved successfully");
            }
            catch (Exception ex)
            {
                Assert.Fail("testLoadGoodFile: board was not solved successfully");
            }
        }

        // tests for when the board is directly input via code
        [TestMethod]
        public void testSolveDirectInputBoard()
        {
            try
            {
                int[][] boardData = new int[][] {
                    new int [] {0, 0, 0, 0, 0, 0, 0, 0, 7},
                    new int [] {0, 4, 8, 6, 0, 2, 1, 5, 9},
                    new int [] {0, 6, 0, 0, 0, 0, 0, 8, 0},
                    new int [] {0, 9, 0, 8, 0, 4, 0, 3, 0},
                    new int [] {0, 0, 0, 0, 2, 0, 0, 0, 0},
                    new int [] {0, 2, 0, 9, 0, 5, 0, 6, 0},
                    new int [] {0, 7, 0, 0, 0, 0, 0, 9, 0},
                    new int [] {1, 8, 3, 2, 0, 7, 5, 4, 0},
                    new int [] {2, 0, 0, 0, 0, 0, 0, 0, 0}
                };
                SudokuBoard testDirectBoard = new SudokuBoard(boardData);
                // board should not be null
                Assert.IsTrue(testDirectBoard.isLoaded(), "testSolveDirectInputBoard: testDirectBoard is not loaded");
                testDirectBoard.solveBoard();
                // board should be solved
                Assert.IsTrue(testDirectBoard.isSolved(), "testSolveDirectInputBoard: board was not solved successfully");
                // test with bad board
                boardData = new int[0][];
                testDirectBoard = new SudokuBoard(boardData);
                Assert.IsFalse(testDirectBoard.isLoaded(), "testSolveDirectInputBoard: testDirectBoard was loaded successfully from bad data");
            }
            catch (Exception ex)
            {
                Assert.Fail("testSolveDirectInputBoard: board was not solved successfully");
            }
        }

        // test for writing the solution to a file
        [TestMethod]
        public void testWriteSolution()
        {
            try
            {
                // clean up existing output file - does not throw any exceptions if the file is already deleted
                File.Delete("testWriteSolution-output.txt");
                SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("test files/good-board.txt");
                // board should not be null
                Assert.IsNotNull(testBoard, "testWriteSolution: testBoard is null");
                testBoard.solveBoard();
                // board should be solved
                Assert.IsTrue(testBoard.isSolved(), "testWriteSolution: board was not solved successfully");
                testBoard.writeSolution("testWriteSolution-output.txt");
                // testWriteSolution-output.txt should exist
                Assert.IsTrue(File.Exists("testWriteSolution-output.txt"), "testWriteSolution: board solution was not written successfully");
            }
            catch (Exception ex)
            {
                Assert.Fail("testWriteSolution: board solution was not written successfully");
            }
        }

        // test for when the board file is missing
        [TestMethod]
        public void testLoadMissingFile()
        {
            try
            {
                SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("test files/nonexistent.txt");
                // board should be null
                Assert.IsNull(testBoard, "testLoadMissingFile: testBoard is not null");
                testBoard.solveBoard();
                // if somehow not null, board should not be solvable
                Assert.IsFalse(testBoard.isSolved(), "testLoadMissingFile: board from missing file was somehow solved");
            }
            catch (Exception ex)
            {
                // should throw FileNotFoundException
                if (ex.GetType().Name != "FileNotFoundException" || !ex.Message.StartsWith("Could not find file"))
                {
                    Assert.Fail("testLoadMissingFile: unexpected exception attempting to load nonexistent test file");
                }
            }
        }

        // tests for when the board is too big or too small
        [TestMethod]
        public void testLoadFileWithWrongSize()
        {
            try
            {
                SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("test files/bad-board-size-80.txt");
                // board should be null
                Assert.IsNull(testBoard, "testLoadFileWithWrongSize: testBoard is not null");
                testBoard.solveBoard();
                // if somehow not null, board should not be solvable
                Assert.IsFalse(testBoard.isSolved(), "testLoadFileWithWrongSize: invalid board was somehow solved");
            }
            catch (Exception ex)
            {
                // InvalidDataException should be thrown
                if (ex.GetType().Name != "InvalidDataException"
                        || ex.Message != "Invalid board size: 80 (should be 81)")
                {
                    Assert.Fail("testLoadFileWithWrongSize: exception other than InvalidFormatException occurred with board too small (80 values)");
                }
            }

            try
            {
                SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("test files/bad-board-size-82.txt");
                // board should be null
                Assert.IsNull(testBoard, "testLoadFileWithWrongSize: testBoard is not null");
                testBoard.solveBoard();
                // if somehow not null, board should not be solvable
                Assert.IsFalse(testBoard.isSolved(), "testLoadFileWithWrongSize: invalid board was somehow solved");
            }
            catch (Exception ex)
            {
                // InvalidDataException should be thrown
                if (ex.GetType().Name != "InvalidDataException"
                        || ex.Message != "Invalid board size: 82 (should be 81)")
                {
                    Assert.Fail("testLoadFileWithWrongSize: exception other than InvalidFormatException occurred with board too big (82 values)");
                }
            }
        }

        // test for when the board has invalid characters
        [TestMethod]
        public void testLoadFileWithInvalidCharacters()
        {
            try
            {
                SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("test files/bad-board-invalid-characters.txt");
                // board should be null
                Assert.IsNull(testBoard,"testLoadFileWithInvalidCharacters: testBoard is not null");
                testBoard.solveBoard();
                // if somehow not null, board should not be solvable
                Assert.IsFalse(testBoard.isSolved(), "testLoadFileWithInvalidCharacters: invalid board was somehow solved");
            }
            catch (Exception ex)
            {
                // InvalidDataException should be thrown
                if (ex.GetType().Name != "InvalidDataException"
                        || ex.Message != "Invalid character found on board: !")
                {
                    Assert.Fail("testLoadFileWithInvalidCharacters: exception other than InvalidFormatException occurred");
                }
            }
        }

        // test for when the board is properly formatted but not solvable
        [TestMethod]
        public void testSolveUnsolvableBoard()
        {
            try
            {
                SudokuBoard testBoard = SudokuBoard.LoadSudokuBoardFromFile("test files/bad-board-unsolvable.txt");
                // board should not be null
                Assert.IsNotNull(testBoard, "testSolveUnsolvableBoard: testBoard is null");
                testBoard.solveBoard();
                Assert.IsFalse(testBoard.isSolved(), "testSolveUnsolvableBoard: unsolvable board was somehow solved");
            }
            catch (Exception ex)
            {
                Assert.Fail("testSolveUnsolvableBoard: unexpected exception attempting to load test file");
            }
        }
    }
}
