// https://dotnetfiddle.net/

using NUnit.Framework;
using NUnitLite;

namespace Prime.UnitTests.Services
{
    [TestFixture]
    public class PrimeService_IsPrimeShould
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void IsPrime_InputIs1_ReturnFalse()
        {
              Assert.IsFalse(false, "1 should not be prime");
        }
		
		public static int Main(string[] args)
		{
  			return new AutoRun().Execute(args);
		}
    }
}

