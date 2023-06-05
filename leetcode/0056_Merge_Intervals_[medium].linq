<Query Kind="Program">
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>Xunit</Namespace>
</Query>

// 56. Merge Intervals
//
// Given an array of intervals where intervals[i] = [starti, endi], merge all overlapping intervals, and 
// return an array of the non-overlapping intervals that cover all the intervals in the input.

#load "xunit"   // Ctrl+Click on "xunit" to open/edit the xunit query.
#load "BenchmarkDotNet"

void Main()
{
	IntervalsAlgo.BenchmarkSetup();
	RunTests();	
	RunBenchmark();
}

[SimpleJob(launchCount: 2, warmupCount: 2, invocationCount: 10)]
public class IntervalsAlgo
{
	static Random rnd = new Random();

	const int n = 100000;
	static int[][] intervals = new int[n][];


	[Benchmark]
    public int[][] MergeWithArraySort() 
	{
		return MergeWithArraySort(intervals);
	}

    public int[][] MergeWithArraySort(int[][] intervals) 
    {
	    if (intervals is null)
			throw new ArgumentNullException(nameof(intervals));
		
		if (intervals.Length < 2)
			return intervals;
		
		Array.Sort(intervals, (x, y) => x[0] - y[0]);
		List<int[]> merged = new List<int[]>();
	    merged.Add(intervals[0]);
	
	    int i = 1;
	    while (i < intervals.Length)
	    {
		    var last = merged[merged.Count - 1];
		    var current = intervals[i];
		    if (last[1] >= current[0])
		    {
			    last[1] = Math.Max(current[1], last[1]);
		    }
		    else
		    {
    			merged.Add(current);
	    	}
		
		    i++;
	    }
	
    	return merged.ToArray();
    }	

	[Benchmark]
    public int[][] MergeWithListAndLinq() 
	{
		return MergeWithListAndLinq(intervals);
	}

	public int[][] MergeWithListAndLinq(int[][] intervals) 
    {
	    if (intervals is null)
			throw new ArgumentNullException(nameof(intervals));
		
		if (intervals.Length < 2)
			return intervals;

 	    List<int[]> schedule = intervals.OrderBy(x => x[0]).ToList();
    	List<int[]> merged = new List<int[]>();
	    merged.Add(schedule.First());
	
	    int i = 1;
	    while (i < schedule.Count)
	    {
		    var last = merged.Last();
		    var current = schedule[i];
		
		    if (current[0] <= last[1])
		    {
			    last[1] = Math.Max(current[1], last[1]);
		    }
		    else
		    {
    			merged.Add(current);
	    	}
		
		    i++;
	    }
	
    	return merged.ToArray();
    }

	[GlobalSetup]
	public static void BenchmarkSetup()
	{
		for (int i = 0; i < intervals.Length; i++)
		{
			var start = rnd.Next(10000);
			intervals[i] = new int[]{start, start + rnd.Next(10000)};
		}
	}
}

public class TestFixture
{
	[Fact]
	public void TestMergeWithArraySort_Null_ShouldThrowException()
	{
		var algo = new IntervalsAlgo();
		Assert.Throws<ArgumentNullException>(() => algo.MergeWithArraySort(null));
	}

	[Theory]
	[MemberData(nameof(GetMergeTestCases))]
	public void TestMergeWithArraySort(int[][] intervals, int[][] expected)
	{
		var algo = new IntervalsAlgo();
		var actual = algo.MergeWithArraySort(intervals);
		
		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(GetMergeTestCases))]
	public void TestMergeWithListAndLinq(int[][] intervals, int[][] expected)
	{
		var algo = new IntervalsAlgo();
		var actual = algo.MergeWithListAndLinq(intervals);
		
		Assert.Equal(expected, actual);
	}

	public static IEnumerable<object[]> GetMergeTestCases()
	{
	    yield return new object[] {new int[][]{}, new int[][]{}};
	    yield return new object[] {new int[][]{new int[]{1, 4}}, new int[][]{new int[]{1, 4}}};
	    yield return new object[] {new int[][]{new int[]{11, 12}, new int[]{11, 12}}, new int[][]{new int[]{11, 12}}};
	    yield return new object[] {new int[][]{new int[]{22, 33}, new int[]{22, 33}, new int[]{22, 33}}, new int[][]{new int[]{22, 33}}};
		
	    yield return new object[] {new int[][]{new int[]{1, 2}, new int[]{10, 11}}, new int[][]{new int[]{1, 2}, new int[]{10, 11}}};
	    yield return new object[] {new int[][]{new int[]{10, 11}, new int[]{1, 2}}, new int[][]{new int[]{1, 2}, new int[]{10, 11}}};

		yield return new object[] {new int[][]{new int[]{1, 15}, new int[]{10, 11}}, new int[][]{new int[]{1, 15}}};
	    yield return new object[] {new int[][]{new int[]{1, 15}, new int[]{15, 17}}, new int[][]{new int[]{1, 17}}};
		
	    yield return new object[] {new int[][]{new int[]{10, 11}, new int[]{1, 15}}, new int[][]{new int[]{1, 15}}};
	    yield return new object[] {new int[][]{new int[]{10, 11}, new int[]{1, 10}}, new int[][]{new int[]{1, 11}}};
	    yield return new object[] {new int[][]{new int[]{10, 12}, new int[]{1, 11}}, new int[][]{new int[]{1, 12}}};

	    yield return new object[] {new int[][]{new int[]{1, 2}, new int[]{10, 11}, new int[]{101, 102}}, 
									new int[][]{new int[]{1, 2}, new int[]{10, 11}, new int[]{101, 102}}};
	    yield return new object[] {new int[][]{new int[]{101, 102}, new int[]{10, 11}, new int[]{1, 2}}, 
									new int[][]{new int[]{1, 2}, new int[]{10, 11}, new int[]{101, 102}}};

	    yield return new object[] {new int[][]{new int[]{1, 102}, new int[]{10, 11}, new int[]{21, 22}}, 
									new int[][]{new int[]{1, 102}}};
	    yield return new object[] {new int[][]{new int[]{1, 22}, new int[]{10, 11}, new int[]{31, 32}}, 
									new int[][]{new int[]{1, 22}, new int[]{31, 32}}};
	    yield return new object[] {new int[][]{new int[]{1, 12}, new int[]{21, 31}, new int[]{31, 32}}, 
									new int[][]{new int[]{1, 12}, new int[]{21, 32}}};

	    yield return new object[] {new int[][]{new int[]{31, 32}, new int[]{10, 11}, new int[]{1, 22}}, 
									new int[][]{new int[]{1, 22}, new int[]{31, 32}}};
	    yield return new object[] {new int[][]{new int[]{31, 32}, new int[]{21, 31},new int[]{1, 12}}, 
									new int[][]{new int[]{1, 12}, new int[]{21, 32}}};
	}
}