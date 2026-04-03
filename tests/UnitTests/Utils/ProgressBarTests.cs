namespace UnitTests.Utils
{
    using ConsoleWeaver.Utils;
    using System;
    using System.Threading.Tasks;

    [TestClass]
    public class ProgressBarTests
    {
        [TestMethod]
        public void Constructor_With_Default_Size()
        {
            using (ProgressBar progressBar = new ProgressBar())
            {
                Assert.AreEqual(20, progressBar.Size);
                Assert.IsTrue(progressBar.AutoClear);
            }
        }

        [TestMethod]
        public void Constructor_With_Custom_Size()
        {
            using (ProgressBar progressBar = new ProgressBar(50))
            {
                Assert.AreEqual(50, progressBar.Size);
                Assert.IsTrue(progressBar.AutoClear);
            }
        }

        [TestMethod]
        public void Constructor_With_AutoClear_False()
        {
            using (ProgressBar progressBar = new ProgressBar(30, false))
            {
                Assert.AreEqual(30, progressBar.Size);
                Assert.IsFalse(progressBar.AutoClear);
            }
        }

        [TestMethod]
        public void Constructor_With_Colors()
        {
            using (ProgressBar progressBar = new ProgressBar(25, true, ConsoleColor.Green, ConsoleColor.Black))
            {
                Assert.AreEqual(25, progressBar.Size);
            }
        }

        [TestMethod]
        public void Constructor_Throws_When_Size_Too_Small()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProgressBar(2));
        }

        [TestMethod]
        public void Constructor_Throws_When_Size_Too_Large()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProgressBar(101));
        }

        [TestMethod]
        public void Report_Clamps_Negative_Values_To_Zero()
        {
            using (ProgressBar progressBar = new ProgressBar())
            {
                // Should not throw, value gets clamped
                progressBar.Report(-0.5);
                progressBar.Report(-100.0);
            }
        }

        [TestMethod]
        public void Report_Clamps_Values_Greater_Than_One()
        {
            using (ProgressBar progressBar = new ProgressBar())
            {
                // Should not throw, value gets clamped
                progressBar.Report(1.5);
                progressBar.Report(100.0);
            }
        }

        [TestMethod]
        public void Report_Accepts_Valid_Range()
        {
            using (ProgressBar progressBar = new ProgressBar())
            {
                progressBar.Report(0.0);
                progressBar.Report(0.25);
                progressBar.Report(0.5);
                progressBar.Report(0.75);
                progressBar.Report(1.0);
            }
        }

        [TestMethod]
        public void Report_Is_Thread_Safe()
        {
            using (ProgressBar progressBar = new ProgressBar())
            {
                // Simulate concurrent updates from multiple threads
                // Don't wait for timer callbacks, just verify Report doesn't throw
                Parallel.For(0, 100, i =>
                {
                    progressBar.Report(i / 100.0);
                });
            }
        }

        [TestMethod]
        public void Dispose_Can_Be_Called_Multiple_Times()
        {
            ProgressBar progressBar = new ProgressBar();
            progressBar.Dispose();
            progressBar.Dispose();
            progressBar.Dispose();
        }

        [TestMethod]
        public void IProgress_Interface_Implementation()
        {
            using (ProgressBar progressBar = new ProgressBar())
            {
                IProgress<double> progress = progressBar;
                progress.Report(0.5);
                progress.Report(1.0);
            }
        }
    }
}
