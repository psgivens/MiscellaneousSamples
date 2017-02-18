using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NumbersTranslators.UnitTests {
    [TestClass]
    public class Indonesian_UnitTest {
        [TestMethod]
        public void TestTigaRatusEmpatPuluhLima() {
            var translator = new NumbersTranslators.IndonesianNumberTranslator();
            var value = translator.TranslateNumber(345);
            Assert.AreEqual(value, "tiga ratus empat puluh lima");
        }

        [TestMethod]
        public void TestSeratusLimabelas() {
            var translator = new NumbersTranslators.IndonesianNumberTranslator();
            var value = translator.TranslateNumber(115);
            Assert.AreEqual(value, "seratus limabelas");
        }

        [TestMethod]
        public void TestSeratusSepuluh() {
            var translator = new NumbersTranslators.IndonesianNumberTranslator();
            var value = translator.TranslateNumber(111);
            Assert.AreEqual(value, "seratus sebelas");
        }

        [TestMethod]
        public void TestTujuhPuluhLima() {
            var translator = new NumbersTranslators.IndonesianNumberTranslator();
            var value = translator.TranslateNumber(75);
            Assert.AreEqual(value, "tujuh puluh lima");
        }
    }
}
