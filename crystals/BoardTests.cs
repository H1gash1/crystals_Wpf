using Microsoft.VisualStudio.TestTools.UnitTesting;
using crystals_Wpf.GameLogic;

namespace crystals.Tests
{
    [TestClass]
    public class BoardTests
    {

        [TestMethod]
        public void GenerateBoard_ShouldCreate8x8Grid()
        {
            BoardManager boardManager = new BoardManager();

            var board = boardManager.GenerateBoard();

            Assert.AreEqual(8, board.GetLength(0), "Количество строк должно быть 8");
            Assert.AreEqual(8, board.GetLength(1), "Количество столбцов должно быть 8");
        }
    }
}