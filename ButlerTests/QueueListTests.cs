using System;
using Butler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ButlerTests {

    [TestClass]
    public class QueueListTests {

        [TestMethod]
        public void AddEmpty() {
            var intQueue = new QueueList<int>(5);
            intQueue.Add(5);
            Assert.IsTrue(intQueue[0] == 5);
        }

        [TestMethod]
        public void AddTillFull() {
            var intQueue = new QueueList<int>(5);
            intQueue.AddRange(1,2,3,4,5);

            Assert.IsTrue(
                intQueue[0] == 5 && 
                intQueue[1] == 4 && 
                intQueue[2] == 3 && 
                intQueue[3] == 2 && 
                intQueue[4] == 1
            );
        }


        [TestMethod]
        public void AddWhenFull() {
            var intQueue = new QueueList<int>(5);

            intQueue.AddRange(1,2,3,4,5,6,7,8);
            
            Assert.IsTrue(
                intQueue[0] == 8 &&
                intQueue[1] == 7 &&
                intQueue[2] == 6 &&
                intQueue[3] == 5 &&
                intQueue[4] == 4
            );
        }

        [TestMethod]
        public void Clear() {
            var intQueue = new QueueList<int>();
            intQueue.AddRange(123,12,312,3123,123);
            intQueue.Clear();

            Assert.IsTrue(intQueue.Count == 0);
        }
    }
}
