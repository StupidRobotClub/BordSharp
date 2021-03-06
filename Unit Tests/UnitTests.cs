using bord;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unit_Tests
{
    [TestClass]
    public class BoardLibShould
    {
        private BoardLib boardLib;
        private BordsContext ctx;

        [TestInitialize]
        public void Setup()
        {
            ctx = new BordsContext(DataContexts.InMemory);
            boardLib = new BoardLib(ctx);
        }

        [TestCleanup]
        public void TearDown()
        {
            ctx.Database.EnsureDeleted();
        }

        [TestMethod]
        public void NewTask_NoInput_CreatesDefaultTask()
        {
            Task _task = boardLib.CreateTask(Defaults.DefaultTaskDescription);
            Assert.IsTrue(_task.Description == Defaults.DefaultTaskDescription);
            Assert.IsFalse(_task.IsCompleted);
            Assert.IsTrue(_task.Priority == 1);
            Assert.IsTrue(_task.Board.Name == Defaults.DefaultBoardName);
        }
        [TestMethod]
        public void NewTask_PriorityOnly_CreatesPrioritizedDefaultTask()
        {
            Task _task = boardLib.CreateTask(Defaults.DefaultTaskDescription, 3);
            Assert.IsTrue(_task.Description == Defaults.DefaultTaskDescription);
            Assert.IsFalse(_task.IsCompleted);
            Assert.IsTrue(_task.Priority == 3);
            Assert.IsTrue(_task.Board.Name == Defaults.DefaultBoardName);
        }
        [TestMethod]
        public void NewTask_CustomBoard_CreatesNewTaskOnNewBoard()
        {
            string boardname = "TestBoard";
            Task _task = boardLib.CreateTask(boardname, Defaults.DefaultTaskDescription, 1);
            Assert.IsTrue(_task.Description == Defaults.DefaultTaskDescription);
            Assert.IsFalse(_task.IsCompleted);
            Assert.IsTrue(_task.Priority == 1);
            Assert.IsTrue(_task.Board.Name == boardname);
        }
        [TestMethod]
        public void NewTask_CustomDescription_CreatesNewTaskWithDescription()
        {
            string description = "Test Description";
            Task _task = boardLib.CreateTask(description);
            Assert.IsTrue(_task.Description == description);
            Assert.IsFalse(_task.IsCompleted);
            Assert.IsTrue(_task.Priority == 1);
            Assert.IsTrue(_task.Board.Name == Defaults.DefaultBoardName);
        }
        [TestMethod]
        public void NewTask_BadPriority()
        {
            Assert.ThrowsException<PriorityOutOfBoundsException>(() => boardLib.CreateTask("", 0));
            Assert.ThrowsException<PriorityOutOfBoundsException>(() => boardLib.CreateTask("", 500));
        }
        [TestMethod]
        public void ToggleTaskComplete_Toggles()
        {
            Task _task = boardLib.CreateTask(Defaults.DefaultTaskDescription);
            Assert.IsFalse(_task.IsCompleted);
            boardLib.ToggleTaskComplete(_task.TaskId);
            Assert.IsTrue(_task.IsCompleted);
            boardLib.ToggleTaskComplete(_task.TaskId);
            Assert.IsFalse(_task.IsCompleted);
        }
        [TestMethod]
        public void ToggleTaskComplete_ToggleNull() => Assert.ThrowsException<BadTaskIdException>(() => boardLib.ToggleTaskComplete(0));
        [TestMethod]
        public void PrioritizeTask_PrioritizeNull() => Assert.ThrowsException<BadTaskIdException>(() => boardLib.PrioritizeTask(0, 1));
        [TestMethod]
        public void PrioritizeTask_PrioritizeOutOfBounds()
        {
            Task _task = boardLib.CreateTask(Defaults.DefaultTaskDescription);
            Assert.ThrowsException<PriorityOutOfBoundsException>(() => boardLib.PrioritizeTask(_task.TaskId, 500));
            Assert.ThrowsException<PriorityOutOfBoundsException>(() => boardLib.PrioritizeTask(_task.TaskId, 0));
        }
        [TestMethod]
        public void PrioritizeTask_PrioritizesTask()
        {
            Task _task = boardLib.CreateTask(Defaults.DefaultTaskDescription);
            boardLib.PrioritizeTask(_task.TaskId, 3);
            Assert.IsTrue(_task.Priority == 3);
        }
        [TestMethod]
        public void DescribeTask_DescribesTask()
        {
            string description = "Test Description";
            Task _task = boardLib.CreateTask(Defaults.DefaultTaskDescription);
            boardLib.DescribeTask(_task.TaskId, description);
            Assert.IsTrue(_task.Description == description);
        }
        [TestMethod]
        public void MoveTask_MovesTask()
        {
            Task _task = boardLib.CreateTask(Defaults.DefaultTaskDescription);
            string boardname = "TestBoard";
            boardLib.MoveTask(_task.TaskId, boardname);
            Assert.IsTrue(_task.Board.Name == boardname);
            Assert.IsTrue(boardLib.GetBoard(boardname).Tasks.Count == 1);
        }
        [TestMethod]
        public void DeleteTask_DeletesTask()
        {
            Task _task = boardLib.CreateTask(Defaults.DefaultTaskDescription);
            Assert.IsTrue(boardLib.GetBoard(Defaults.DefaultBoardName).Tasks.Count == 1);
            boardLib.DeleteTask(_task.TaskId);
            Assert.IsTrue(boardLib.GetBoard(Defaults.DefaultBoardName).Tasks.Count == 0);
        }
        [TestMethod]
        public void FindBoard_FindsDefault()
        {
            Board board = boardLib.GetBoard(Defaults.DefaultBoardName);
            Assert.IsTrue(board.Name == Defaults.DefaultBoardName);
        }
        [TestMethod]
        public void FindBoard_CreatesIfNotExists()
        {
            string boardname = "Test Board";
            Board board = boardLib.GetBoard(boardname);
            Assert.IsTrue(board.Name == boardname);
        }
        [TestMethod]
        public void FindTask_FindsTask()
        {
            Task _task = boardLib.CreateTask(Defaults.DefaultTaskDescription);
            Task _task_2 = boardLib.GetTask(_task.TaskId);
            Assert.AreSame(_task, _task_2);
        }
        [TestMethod]
        public void FindTask_FindsNull()
        {
            var x = boardLib.GetTask(0);
            Assert.IsNull(x);
        }
        // Maybe MOQ for testing Print?
    }
}
