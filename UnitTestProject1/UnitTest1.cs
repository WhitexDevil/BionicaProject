
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using project;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
         Squad[] humanArcher1 =new Squad[1]{ new Squad(new Unit(4, 12, 5, 4, 3, 20, 10f))};
         Squad[] humanArcher2 =new Squad[1]{new Squad( new Unit(4, 12, 5, 4, 3, 20, 10f))};
           Deffensive a = new Deffensive();
            int size = 8;
            byte[] m = new byte[size*size];
            m[0] = 1;

            humanArcher1[0].Position =new  Point(0, 0);
            m[size - 1] = 1;
            humanArcher2[0].Position = new Point(size - 1, 0);
            BattleData b =  new BattleData(humanArcher1, humanArcher2, m, size);
            a.Maneuvers[0](b);
           
        }
    }
}
