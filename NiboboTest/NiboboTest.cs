using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NiboboTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void TestBlockVarients()
        {
            Block blockA = BlockFactory.GetBlockByName("A");
            Assert.AreEqual(8, blockA.m_varients.Count);
            Block blockG = BlockFactory.GetBlockByName("G");
            Assert.AreEqual(4, blockG.m_varients.Count);
            Block blockI = BlockFactory.GetBlockByName("I");
            Assert.AreEqual(4, blockI.m_varients.Count);
            Block blockJ = BlockFactory.GetBlockByName("J");
            Assert.AreEqual(2, blockJ.m_varients.Count);
            Block blockK = BlockFactory.GetBlockByName("K");
            Assert.AreEqual(1, blockK.m_varients.Count);
            Block blockL = BlockFactory.GetBlockByName("L");
            Assert.AreEqual(1, blockL.m_varients.Count);
        }

        [Test]
        public void TestBlockingQueue()
        {
            BlockingCollection<int> bc = new BlockingCollection<int>(10);
            for (int i = 0; i < 10; i++)
            {
                bc.Add(i);
            }
            for (int i = 0; i < 10; i++)
            {
                bc.Take();
            }
        }

        [Test]
        public void TestBlockingQueue2()
        {
            BlockingCollection<int> bc = new BlockingCollection<int>(10);
            for (int i = 0; i < 10; i++)
            {
                bc.Add(i);
            }
            bc.Take();
            bc.Add(12);
            for (int i = 0; i < 9; i++)
            {
                bc.Take();
            }
            Assert.AreEqual(12, bc.Take());
        }
    }
}