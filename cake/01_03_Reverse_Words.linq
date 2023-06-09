<Query Kind="Program">
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>Xunit</Namespace>
</Query>

#load "xunit"   // Ctrl+Click on "xunit" to open/edit the xunit query.
#load "BenchmarkDotNet"

void Main()
{
	StringAlgo.BenchmarkSetup();
	RunTests();	

	RunBenchmark();
}

[SimpleJob(launchCount: 2, warmupCount: 2, invocationCount: 70)]
public class StringAlgo
{
	static Random rnd = new Random();

	const int n = 100000;
	static char[] text;
	static string solidText;

	[Benchmark]
	public void ReverseWordsByLinq()
	{
		var _ = ReverseWordsByLinq(solidText);
	}

	public string ReverseWordsByLinq(string text)
	{
		return string.Join(' ', text.Split(' ').Reverse());
	}

	[Benchmark]
	public void ReverseWords()
	{
		ReverseWords(text);
	}
	
	public void ReverseWords(char[] text)
	{
		ReverseChars(text, 0, text.Length - 1);

		int start = 0;
		for (int i = start; i < text.Length;)
		{
			 if (i == text.Length - 1)
			 {
				ReverseChars(text, start, i);
				i++;
			 }
			else if (text[i] == ' ')
			{
				ReverseChars(text, start, i - 1);
				i++;
				start = i;
			}
			else
			{
				i++;
			}
		}
	}

	void ReverseChars(char[] text, int start, int end)
	{
	    for (int i = start, j = end; i < j; i++, j--)
	    {
	        char temp = text[i];
	        text[i] = text[j];
	        text[j] = temp;
	    }
	}

	[GlobalSetup]
	public static void BenchmarkSetup()
	{
		string[] words = new string[n];
		for (int i = 0; i < n; i++)
		{
			char[] symbols = new char[rnd.Next(6)];
			for (int j = 0; j < symbols.Length; j++)
			{
				symbols[j] = (char)((byte)'a' + (byte)rnd.Next(24));
			}
			
			words[i] = string.Concat(symbols);
		}
		
		solidText = string.Join(' ', words);
		text = solidText.ToArray();
	}
}

public class TestFixture
{
	[Theory]
	[MemberData(nameof(GetReverseWordsTestCases))]
	public void TestReverseWords(string text, string expected)
	{
		var input = text.ToArray();
		var expectedText = expected.ToArray();
		
		var algo = new StringAlgo();
		algo.ReverseWords(input);
		var actual = input;
		
		Assert.Equal(expectedText, actual);
	}
	
	[Theory]
	[MemberData(nameof(GetReverseWordsTestCases))]
	public void TestReverseWordsByLinq(string text, string expected)
	{
		var algo = new StringAlgo();
		var actual = algo.ReverseWordsByLinq(text);
		
		Assert.Equal(expected, actual);
	}
	
	public static IEnumerable<object[]> GetReverseWordsTestCases()
	{
	    yield return new object[] {"", ""};
	    yield return new object[] {"a", "a"};
	    yield return new object[] {"ab", "ab"};
	    yield return new object[] {"a b", "b a"};
	    yield return new object[] {"ab c", "c ab"};
	    yield return new object[] {"a bc", "bc a"};
	    yield return new object[] {"ab bc", "bc ab"};
	    yield return new object[] {"abc d", "d abc"};
	    yield return new object[] {"a bcd", "bcd a"};
	    yield return new object[] {"a b c", "c b a"};
	    yield return new object[] {"ab b c", "c b ab"};
	    yield return new object[] {"a bb c", "c bb a"};
	    yield return new object[] {"a b bc", "bc b a"};
	    yield return new object[] {"ab bb bc", "bc bb ab"};
	    yield return new object[] {"ab bb bc bd", "bd bc bb ab"};
	    yield return new object[] {"ab bb bc bd be", "be bd bc bb ab"};
	}
}