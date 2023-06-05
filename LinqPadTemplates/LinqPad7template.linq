<Query Kind="Program">
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>Xunit</Namespace>
</Query>

// LinqPad 7 template with bechmarks by BenchmarkDotNet and unit tests with xunit
//
// Issue:
// [ ] Not bechmark memory

#load "xunit"   // Ctrl+Click on "xunit" to open/edit the xunit query.
#load "BenchmarkDotNet"

void Main()
{
	StringAlgo.BenchmarkSetup();
	RunTests();	

	RunBenchmark(); return;
}

[SimpleJob(launchCount: 2, warmupCount: 2, invocationCount: 100)]
public class StringAlgo
{
	static Random rnd = new Random();

	const int n = 1000000;
	static char[] text = new char[n];

	[Benchmark]
	public void ReverseWithTupleAndHatOperator()
	{
		ReverseWithTupleAndHatOperator(text);
	}

	public void ReverseWithTupleAndHatOperator(char[] text)
	{
	    if (text is null)
			throw new ArgumentNullException(nameof(text));

	    for ( int i = 1; i <= text.Length / 2; i++)
	    {
			(text[i-1], text[^i]) = (text[^i], text[i-1]);
	    }
	}

	[Benchmark]
	public void ReverseWithWhileAndTuple()
	{
		ReverseWithWhileAndTuple(text);
	}

	public void ReverseWithWhileAndTuple(char[] text)
	{
	    if (text is null)
			throw new ArgumentNullException(nameof(text));

		int leftIndex = 0;
		int rightIndex = text.Length - 1;
	    while ( leftIndex < rightIndex)
	    {
			(text[leftIndex], text[rightIndex]) = (text[rightIndex], text[leftIndex]);
			
			leftIndex++;
			rightIndex--;
	    }
	}

	[GlobalSetup]
	public static void BenchmarkSetup()
	{
		for (int i = 0; i < text.Length; i++)
		{
			text[i] = (char)rnd.Next('z');
		}
	}
}

public class TestFixture
{
	[Fact]
	public void TestReverseWithWhileAndTuple_Null_ShouldThrowException()
	{
		var a = new StringAlgo();
		Assert.Throws<ArgumentNullException>(() => a.ReverseWithWhileAndTuple(null));
	}

	[Theory]
	[MemberData(nameof(GetReverseTestCases))]
	public void TestReverseWithWhileAndTuple(params char[] text)
	{
		var preExpected = (char[])text.Clone();
		Array.Reverse(preExpected);
		var expected = preExpected;
		
		var a = new StringAlgo();
		a.ReverseWithWhileAndTuple(text);
		
		Assert.Equal(expected, text);
	}
	
	[Theory]
	[MemberData(nameof(GetReverseTestCases))]
	public void TestReverseWithWhileAndTupleAndHatOperator(params char[] text)
	{
		var preExpected = (char[])text.Clone();
		Array.Reverse(preExpected);
		var expected = preExpected;
		
		var a = new StringAlgo();
		a.ReverseWithTupleAndHatOperator(text);
		
		Assert.Equal(expected, text);
	}
	
	public static IEnumerable<object[]> GetReverseTestCases()
	{
	    yield return new object[] {};
	    yield return new object[] {'a'};
	    yield return new object[] {'a', 'b'};
	    yield return new object[] {'a', 'b', 'c'};
	    yield return new object[] {'a', 'b', 'c', 'd'};
	    yield return new object[] {'a', 'b', 'c', 'd', 'e'};
	    yield return new object[] {'a', 'a'};
	    yield return new object[] {'a', 'a', 'a'};
	    yield return new object[] {'a', 'a', 'a', 'a'};
	}
}