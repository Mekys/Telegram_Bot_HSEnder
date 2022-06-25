using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bot_tg;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AddCourse()
        {
            Information information = new Information();
            information.AddCours(1);
            Assert.IsTrue(information.Course.ContainsKey(1));
        }
        [TestMethod]
        public void AddPE()
        {
            Information information = new Information();
            information.AddCours(1);
            information.AddEducationProgram(1, "test");
            Assert.IsTrue(information.Course[1].ContainsKey("test"));
        }
        [TestMethod]
        public void AddPEWithoutCourse()
        {
            Information information = new Information();
            information.AddEducationProgram(1, "test");
            Assert.IsTrue(information.Course[1].ContainsKey("test"));
        }
        [TestMethod]
        public void AddGroup()
        {
            Information information = new Information();
            information.AddEducationProgram(1, "test");
            information.AddGroup(1, "test", "test-23-1");
            Assert.IsTrue(information.Course[1]["test"].ContainsKey("test-23-1"));
        }
        [TestMethod]
        public void AddGroupWithoutPE()
        {
            Information information = new Information();
            information.AddGroup(1, "test", "test-23-1");
            Assert.IsTrue(information.Course[1]["test"].ContainsKey("test-23-1"));
        }
        [TestMethod]
        public void AddId()
        {
            Information information = new Information();
            information.AddGroup(1, "test", "test-23-1");
            for (int i = 0; i < 10; i++)
                 information.AddId(1, "test", "test-23-1", i);
            Assert.IsTrue(information.GetId(1, "test", "test-23-1").Count == 10);
        }
        [TestMethod]
        public void AddIdWithoutGroup()
        {
            Information information = new Information();
            for (int i = 0; i < 10; i++)
                 information.AddId(1, "test", "test-23-1", i);
            Assert.IsTrue(information.GetId(1, "test", "test-23-1").Count == 10);
        }
        [TestMethod]
        public void UserStatesAddCourse()
        {
            UserStates states = new UserStates();
            states.Course = 2;
            Assert.AreEqual(states.Course, 2);
        }
        [TestMethod]
        public void UserStatesAddPE()
        {
            UserStates states = new UserStates();
            states.PE = "expect";
            Assert.AreEqual(states.PE, "expect");
        }
        [TestMethod]
        public void UserStatesAddGroup()
        {
            UserStates states = new UserStates();
            states.Group = "expect";
            Assert.AreEqual(states.Group ,"expect");
        }
        
    }
}
