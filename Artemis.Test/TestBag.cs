using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Artemis.Utils;
using System.Diagnostics;
using System.Globalization;

namespace Artemis.Test
{
    #region Additional test attributes

    /*
     * You can use the following additional attributes as you write your tests:
     * Use ClassInitialize to run code before running the first test in the class
     * [ClassInitialize()]
     * public static void MyClassInitialize(TestContext testContext)
     * {
     * }
     * 
     * Use ClassCleanup to run code after all tests in a class have run
     * [ClassCleanup()]
     * public static void MyClassCleanup()
     * {
     * }
     * 
     * Use TestInitialize to run code before running each test
     * [TestInitialize()]
     * public void MyTestInitialize()
     * {
     * }
     * 
     * Use TestCleanup to run code after each test has run
     * [TestCleanup()]
     * public void MyTestCleanup()
     * {
     * }
     */
    #endregion

    [TestClass]
    public class TestBag
    {
        /// <summary>The test Capacity.</summary>
        private const int Capacity = 10;

        /// <summary>The test element1.</summary>
        private const string TestElement1 = "Test element 1";

        /// <summary>The test element2.</summary>
        private const string TestElement2 = "Test element 2";

        /// <summary>The test element3.</summary>
        private const string TestElement3 = "Test element 3";

        /// <summary>Gets or sets the test context which provides information about and functionality for the current test run.</summary>
        /// <value>The test context.</value>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void TestBagConstructor()
        {
            Bag<string> target = new Bag<string>(Capacity);
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void TestAdd()
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            Bag<string> target = new Bag<string>(Capacity);
            // ReSharper restore UseObjectOrCollectionInitializer
            target.Add(TestElement1);
            target.Add(TestElement2);
            target.Add(TestElement3);

            Assert.IsTrue(target.Contains(TestElement1) && target.Contains(TestElement2) && target.Contains(TestElement3));
        }

        [TestMethod]
        public void TestAddRange()
        {
            Bag<string> target = new Bag<string>(Capacity);
            Bag<string> rangeOfElements = new Bag<string>(Capacity) { TestElement1, TestElement2, TestElement3 };
            target.AddRange(rangeOfElements);
            Assert.IsTrue(target.Contains(TestElement1) && target.Contains(TestElement2) && target.Contains(TestElement3));
        }

        [TestMethod]
        public void TestClear()
        {
            Bag<string> target = new Bag<string>(Capacity) { TestElement1, TestElement2, TestElement3 };
            target.Clear();
            Assert.AreEqual(0, target.Size);
        }


        [TestMethod]
        public void TestContains()
        {
            Bag<string> target = new Bag<string>(Capacity) { TestElement1, TestElement2, TestElement3 };
            const string Element = TestElement2;
            const bool Expected = true;
            bool actual = target.Contains(Element);
            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        public void TestGet()
        {
            Bag<string> target = new Bag<string>(Capacity) { TestElement1, TestElement2, TestElement3 };
            const int Index = 1;
            const string Expected = TestElement2;
            string actual = target[Index];
            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        public void TestGrow()
        {
            Bag<string> target = new Bag<string>(0);
            int beforeGrow = target.Capacity;
            target.AddRange(new Bag<string> { TestElement1, TestElement2, TestElement3 });
            const int Expected = 4;
            int actual = target.Capacity;
            Assert.IsTrue(beforeGrow < actual);
            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        public void TestRemove()
        {
            Bag<string> target = new Bag<string>(Capacity) { TestElement1, TestElement2, TestElement3 };
            const int Index = 1;
            const string Expected = TestElement2;
            string actual = target.Remove(Index);
            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        public void TestRemoveAll()
        {
            Bag<string> target = new Bag<string>(Capacity) { TestElement1, TestElement2, TestElement3 };
            Bag<string> bag = new Bag<string>(Capacity) { TestElement2, TestElement3 };
            const bool Expected = true;
            bool actual = target.RemoveAll(bag);
            Assert.AreEqual(Expected, actual);
        }

        public void TestRemoveLast()
        {
            Bag<string> target = new Bag<string>(Capacity) { TestElement1, TestElement2, TestElement3 };
            const string Expected = TestElement3;
            string actual = target.RemoveLast();
            Assert.AreEqual(Expected, actual);
        }
        public void TestSet()
        {
            Bag<string> target = new Bag<string>(Capacity) { TestElement1, TestElement2, TestElement3 };
            const int Index = Capacity - 1;
            const string Element = "TestSetElement";
            target[Index] = Element;
            const string Expected = Element;
            string actual = target.RemoveLast();
            Assert.AreEqual(Expected, actual);
        }

        public void TestCapacity()
        {
            Bag<string> target = new Bag<string>(Capacity);
            int actual = target.Capacity;
            Assert.AreEqual(actual, Capacity);
        }

        public void TestIsEmpty()
        {
            Bag<string> target = new Bag<string>(Capacity);
            Assert.IsTrue(target.IsEmpty);
        }

        [TestMethod]
        public void TestPerformance()
        {
            Debug.WriteLine("Number of elements: ");

            // Identify max mem size.
            Bag<int> bigBag = new Bag<int>();

            int maxMem = 500000;

            // pointless to use int.maxvalue (sometimes it works, some it does not ... depends on other process)
            for (int index = 0; index < maxMem; ++index)
            {
                try
                {
                    bigBag.Add(index);
                }
                catch (Exception)
                {
                    // some extra to be sure (there are some memory allocs we cant control in other threads)
                    maxMem = index;
                    break;
                }
            }

            bigBag = null;

            // This is need to secure that enough memory is left.
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();


            Debug.WriteLine(maxMem.ToString(CultureInfo.InvariantCulture));

            // Reset bag.
            bigBag = new Bag<int>(0);

            // Start measurement.
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Fill
            for (int index = maxMem; index >= 0; --index)
            {
                bigBag.Add(index);
            }

            stopwatch.Stop();
            string s1 = string.Format("Load  duration: {0}" ,stopwatch.Elapsed.TotalMilliseconds);

            stopwatch.Restart();
            bigBag.Clear();
            stopwatch.Stop();
            string s2 = string.Format("Clear duration: {0}", stopwatch.Elapsed.TotalMilliseconds);

            Assert.Fail(s1 + "\n" + s2);
        }
    }
}
