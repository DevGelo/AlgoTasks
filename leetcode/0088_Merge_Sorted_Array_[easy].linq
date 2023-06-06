<Query Kind="Program">
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>Xunit</Namespace>
</Query>

// 88. Merge Sorted Array
//
// You are given two integer arrays nums1 and nums2, sorted in non-decreasing order, 
// and two integers m and n, representing the number of elements in nums1 and nums2 respectively.

// Merge nums1 and nums2 into a single array sorted in non-decreasing order.

// The final sorted array should not be returned by the function, but instead be stored inside the array nums1. 
// To accommodate this, nums1 has a length of m + n, where the first m elements denote the elements 
// that should be merged, and the last n elements are set to 0 and should be ignored. nums2 has a length of n.

#load "xunit"   // Ctrl+Click on "xunit" to open/edit the xunit query.
#load "BenchmarkDotNet"

void Main()
{
	Algo.BenchmarkSetup();
	RunTests();	
	RunBenchmark();
}

[SimpleJob(launchCount: 2, warmupCount: 2, invocationCount: 10)]
public class Algo
{
	static Random rnd = new Random();

	const int m = 20;
	const int n = 10;
	static int[] nums1 = new int[m + n];
	static int[] nums2 = new int[n];

	[Benchmark]
    public void Merge() 
	{
		Merge(nums1, m , nums2, n);
	}

    public void Merge(int[] nums1, int m, int[] nums2, int n) 
	{
	    if (nums1 is null)
			throw new ArgumentNullException(nameof(nums1));

	    if (nums2 is null)
			throw new ArgumentNullException(nameof(nums2));

		int i = m - 1;
		int j = n - 1;
		int k = m + n - 1;
		while(k >= 0)
		{
			if ((i < 0) || ((j >= 0) && (nums2[j] > nums1[i])))
			{
				nums1[k] = nums2[j];
				j--;
			}
			else
			{
				nums1[k] = nums1[i];
				i--;
			}

			k--;
		}
	}

	[Benchmark]
    public void MergeWithArraySort() 
	{
		Merge(nums1, m , nums2, n);
	}

    public void MergeWithArraySort(int[] nums1, int m, int[] nums2, int n) 
	{
		for (int i = 0; i < n; i++)
		{
			nums1[m + i] = nums2[i];
		}
		
		Array.Sort(nums1);
	}

	[GlobalSetup]
	public static void BenchmarkSetup()
	{
		for (int i = 0; i < m; i++)
		{
			nums1[i] = rnd.Next(10000);
		}
		
		for (int i = 0; i < n; i++)
		{
			nums2[i] = rnd.Next(10000);
		}
	}
}

public class TestFixture
{
	[Fact]
	public void TestMerge_FirstNull_ShouldThrowException()
	{
		var algo = new Algo();
		Assert.Throws<ArgumentNullException>(() => algo.Merge(null, 0, new int[0], 0));
	}

	[Fact]
	public void TestMerge_SecondNull_ShouldThrowException()
	{
		var algo = new Algo();
		Assert.Throws<ArgumentNullException>(() => algo.Merge(new int[0], 0, null, 0));
	}

	[Theory]
	[MemberData(nameof(GetMergeTestCases))]
	public void TestMerge(int[] nums1, int[] nums2, int[] expected)
	{
		var algo = new Algo();
		algo.Merge(nums1, nums1.Length - nums2.Length, nums2, nums2.Length);
		
		Assert.Equal(expected, nums1);
	}

	[Theory]
	[MemberData(nameof(GetMergeTestCases))]
	public void TestMergeWithArraySort(int[] nums1, int[] nums2, int[] expected)
	{
		var algo = new Algo();
		algo.MergeWithArraySort(nums1, nums1.Length - nums2.Length, nums2, nums2.Length);
		
		Assert.Equal(expected, nums1);
	}

	public static IEnumerable<object[]> GetMergeTestCases()
	{
	    yield return new object[] {new int[0], new int[0], new int[0]};
	    yield return new object[] {new int[]{1}, new int[0], new int[]{1}};
	    yield return new object[] {new int[]{0}, new int[]{1}, new int[]{1}};

	    yield return new object[] {new int[]{1, 2}, new int[0], new int[]{1, 2}};
	    yield return new object[] {new int[]{0, 0}, new int[]{1, 2}, new int[]{1, 2}};
		
	    yield return new object[] {new int[]{1, 0}, new int[]{2}, new int[]{1, 2}};
	    yield return new object[] {new int[]{2, 0}, new int[]{1}, new int[]{1, 2}};
		
	    yield return new object[] {new int[]{1, 2, 0}, new int[]{3}, new int[]{1, 2, 3}};
	    yield return new object[] {new int[]{2, 3, 0}, new int[]{1}, new int[]{1, 2, 3}};
	    yield return new object[] {new int[]{1, 3, 0}, new int[]{2}, new int[]{1, 2, 3}};

	    yield return new object[] {new int[]{1, 2, 0, 0}, new int[]{3, 4}, new int[]{1, 2, 3, 4}};
	    yield return new object[] {new int[]{2, 3, 0, 0}, new int[]{1, 4}, new int[]{1, 2, 3, 4}};
	    yield return new object[] {new int[]{3, 4, 0, 0}, new int[]{1, 2}, new int[]{1, 2, 3, 4}};
	    yield return new object[] {new int[]{1, 4, 0, 0}, new int[]{2, 3}, new int[]{1, 2, 3, 4}};

	    yield return new object[] {new int[]{1, 2, 3, 0, 0, 0}, new int[]{4, 5, 6}, new int[]{1, 2, 3, 4, 5, 6}};
	    yield return new object[] {new int[]{1, 2, 6, 0, 0, 0}, new int[]{3, 4, 5}, new int[]{1, 2, 3, 4, 5, 6}};
	    yield return new object[] {new int[]{1, 5, 6, 0, 0, 0}, new int[]{2, 3, 4}, new int[]{1, 2, 3, 4, 5, 6}};
	    yield return new object[] {new int[]{4, 5, 6, 0, 0, 0}, new int[]{1, 2, 3}, new int[]{1, 2, 3, 4, 5, 6}};
	}
}