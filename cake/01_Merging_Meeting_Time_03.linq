<Query Kind="Program">
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>Xunit</Namespace>
</Query>

#load "xunit"   // Ctrl+Click on "xunit" to open/edit the xunit query.
#load "BenchmarkDotNet"

void Main()
{
	Algo.BenchmarkSetup();
	RunTests();	

	RunBenchmark();
}

[SimpleJob(launchCount: 2, warmupCount: 2, invocationCount: 70)]
public class Algo
{
	const int n = 100000;
	Meeting[] schedule = new Meeting[n];
	Random rnd = new Random();

	[Benchmark]
	public List<Meeting> MergeRangesWithSort()
	{
		return MergeRangesWithSort(schedule);
	}

	List<Meeting> MergeRangesWithSort(params Meeting[] original)
	{
		if (original == null)
			throw new ArgumentNullException(nameof(original));

		if (schedule.Length == 1)
			return schedule.ToList();

		List<Meeting> sortedSchedule = original.OrderBy(x => x.StartTime).ToList();
		
		List<Meeting> merged = new List<Meeting>();
		merged.Add(schedule.First());
		
		int i = 1;
		while (i < sortedSchedule.Count)
		{
			var last = merged.Last();
			var current = schedule[i];
			if (current.StartTime <= last.EndTime)
			{
				last.EndTime = int.Max(current.EndTime, last.EndTime);
			}
			else
			{
				merged.Add(current);
			}
			
			i++;
		}
		
		return merged;
	}

	[Benchmark]
	public List<Meeting> MergeRangesA()
	{
		return MergeRangesA(schedule);
	}

	List<Meeting> MergeRangesA(params Meeting[] original)
	{
		if (original == null)
			throw new ArgumentNullException(nameof(original));
			
		if (original.Length == 1)
			return original.ToList();

		List<Meeting> schedule = new List<Meeting>(original);
		for (int i = 0; i < schedule.Count;)
		{
			int j = 1;
			bool isMerged = false;
			while(j < schedule.Count && !isMerged)
			{
				if (i == j)
				{
					j++;
					continue;
				}
			
				Meeting a = schedule[i];
				Meeting b = schedule[j];
				
				//i.Dump("i=");
				//j.Dump("j=");
				//a.Dump("a=");
				//b.Dump("b=");
				
				if ((a.StartTime <= b.StartTime && b.StartTime <= a.EndTime)
					|| (b.StartTime <= a.StartTime && a.StartTime <= b.EndTime))
				{
					//"true".Dump();

					var merged = new Meeting(int.Min(a.StartTime, b.StartTime), int.Max(a.EndTime, b.EndTime));
					schedule.RemoveAt(j);
					schedule.Insert(j, merged);
					schedule.RemoveAt(i);
					isMerged = true;
					
					//schedule.Dump("merged");
				}
				else
				{
					j++;
				}
			}
			
			if (!isMerged)
			{
				i++;
			}
		}

		return schedule;
	}

/*
	[Benchmark]
	public List<MeetingB> MergeRangesC()
	{
		return MergeRangesB(originalB);
	}

	public List<MeetingB> MergeRangesC(params MeetingB[] original)
	{
			if (original == null)
			throw new ArgumentNullException(nameof(original));
			
		if (original.Length == 1)
			return original.ToList();

		List<MeetingB> schedule = new List<MeetingB>(original);
		for (int i = 0; i < schedule.Count;)
		{
			int j = 1;
			bool isMerged = false;
			while(j < schedule.Count && !isMerged)
			{
				if (i == j)
				{
					j++;
					continue;
				}
			
				var a = schedule[i];
				var b = schedule[j];
				
				//i.Dump("i=");
				//j.Dump("j=");
				//a.Dump("a=");
				//b.Dump("b=");
				
				if ((a.StartTime <= b.StartTime && b.StartTime <= a.EndTime)
					|| (b.StartTime <= a.StartTime && a.StartTime <= b.EndTime))
				{
					//"true".Dump();

					//var merged = new Meeting(int.Min(a.StartTime, b.StartTime), int.Max(a.EndTime, b.EndTime));
					a.StartTime = int.Min(a.StartTime, b.StartTime);
					a.EndTime = int.Max(a.EndTime, b.EndTime);
					schedule.RemoveAt(j);
					isMerged = true;
					
					//schedule.Dump("merged");
				}
				else
				{
					j++;
				}
			}
			
			if (!isMerged)
			{
				i++;
			}
		}

		return schedule;
	}
*/

	public record MeetingRecord(int StartTime, int EndTime);

	public class Meeting
	{
		public int StartTime {get; set;}
		public int EndTime {get; set;}
		
		public Meeting(int startTime, int endTime)
		{
			StartTime = startTime;
			EndTime = endTime;
		}
	}

	[GlobalSetup]
	public void BenchmarkSetup()
	{
		const int max = 10000;
		for (int i = 0; i < schedule.Length; i++)
		{
			var start = rnd.Next(max);
			schedule[i] = new Meeting(start, start + rnd.Next(max));
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