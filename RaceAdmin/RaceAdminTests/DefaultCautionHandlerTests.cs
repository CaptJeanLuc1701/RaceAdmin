﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceAdmin;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace RaceAdminTests
{
    [TestClass]
    public class DefaultCautionHandlerTests
    {
        private static Color control, gold;

        private Panel panel;
        private DefaultCautionHandler handler;

        [ClassInitialize]
        public static void BeforeClass(TestContext ctx)
        {
            control = Color.FromName(RaceAdmin.Properties.Resources.ColorName_Control);
            gold = Color.FromName(RaceAdmin.Properties.Resources.ColorName_Gold);
        }

        [TestInitialize]
        public void Before()
        {
            panel = new Panel
            {
                BackColor = control
            };
            handler = new DefaultCautionHandler(panel)
            {
                Interval = 2, // ms
            };
        }

        [TestMethod]
        public void TestCautionThresholdReached_FlashesCautionPanel()
        {
            var colorChangeCount = 0;
            var lastColor = panel.BackColor;

            // handle color change events to test that the colors alternate each time
            // and count the number of changes
            panel.BackColorChanged += (sender, e) =>
            {
                var backColor = panel.BackColor;
                if (lastColor == control)
                {
                    Assert.AreEqual(gold, backColor);
                }
                else if (lastColor == gold)
                {
                    Assert.AreEqual(control, backColor);
                }
                else
                {
                    Assert.Fail("unexpected color in caution panel");
                }
                lastColor = backColor;
                colorChangeCount++;
            };

            // inform the handler than the caution threshold has been reached and
            // then wait a short time to let the panel flash
            handler.CautionThresholdReached();
            Thread.Sleep(200); // ms

            // make sure we got at least three color change events
            Assert.IsTrue(colorChangeCount >= 3, "expected colorChangeCount to be at least {0} but was {1}", 3, colorChangeCount);
        }

        [TestMethod]
        public void TestGreenFlagThrown_StopsFlashingCautionPanel()
        {
            var lastColor = panel.BackColor;
            var lastTime = DateTime.Now;

            // track the last color set and the time at which it was set
            panel.BackColorChanged += (sender, e) =>
            {
                lastColor = panel.BackColor;
                lastTime = DateTime.Now;
            };

            // start flashing the panel first
            handler.CautionThresholdReached();
            Thread.Sleep(200); // ms

            // then throw the green to stop the flashing
            handler.GreenFlagThrown();
            Thread.Sleep(1);
            var greenTime = DateTime.Now;

            // check panel.BackColor as well as the last color seen in the BackColorChanged 
            // event handler to make sure the flashisg actually stopped on time
            Assert.AreEqual(control, panel.BackColor);
            Assert.AreEqual(control, lastColor);
            Assert.IsTrue(
                greenTime > lastTime,
                "expected greenTime {0} to be after {1}",
                DateUtil.ToStringISO(greenTime), DateUtil.ToStringISO(lastTime));
        }

    }
}
