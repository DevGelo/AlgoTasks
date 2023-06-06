<Query Kind="Program">
  <Namespace>Xunit</Namespace>
</Query>

#load "xunit"

void Main()
{
	RunTests();	
}

public class Algo
{
	public bool IsFistComeFirstServed(int[] takeOutOrders, int[] dineInOrders, int[] servedOrders)
	{
	    if (takeOutOrders is null)
			throw new ArgumentNullException(nameof(takeOutOrders));

	    if (dineInOrders is null)
			throw new ArgumentNullException(nameof(dineInOrders));

	    if (servedOrders is null)
			throw new ArgumentNullException(nameof(servedOrders));
		
		int i = 0;
		int j = 0;
		for (int k = 0; k < servedOrders.Length; k++)
		{
			int order = servedOrders[k];
			if (i < takeOutOrders.Length && takeOutOrders[i] == order)
			{
				i++;
			}
			else if(j < dineInOrders.Length && dineInOrders[j] == order)
			{
				j++;
			}
			else
			{
				return false;
			}
		}
		
		return (i == takeOutOrders.Length) && (j == dineInOrders.Length);
	}
}

public class TestFixture
{
	[Fact]
	public void TestIsFistComeFirstServed_FirstNull_ShouldThrowException()
	{
		var algo = new Algo();
		Assert.Throws<ArgumentNullException>(() => algo.IsFistComeFirstServed(null, new int[0], new int[0]));
	}

	[Fact]
	public void TestIsFistComeFirstServed_SecondNull_ShouldThrowException()
	{
		var algo = new Algo();
		Assert.Throws<ArgumentNullException>(() => algo.IsFistComeFirstServed(new int[0], null, new int[0]));
	}

	[Fact]
	public void TestIsFistComeFirstServed_ThirdNull_ShouldThrowException()
	{
		var algo = new Algo();
		Assert.Throws<ArgumentNullException>(() => algo.IsFistComeFirstServed(new int[0], new int[0], null));
	}

	[Theory]
	[MemberData(nameof(GetMergeTestCases))]
	public void TestIsFistComeFirstServed(int[] takeOutOrders, int[] dineInOrders, int[] servedOrders, bool expected)
	{
		var algo = new Algo();
		var actual = algo.IsFistComeFirstServed(takeOutOrders, dineInOrders, servedOrders);
		
		Assert.Equal(expected, actual);
	}

	public static IEnumerable<object[]> GetMergeTestCases()
	{
	    yield return new object[] {new int[0], new int[0], new int[0], true};
	    yield return new object[] {new int[]{1}, new int[0], new int[]{1}, true};
	    yield return new object[] {new int[0], new int[]{1}, new int[]{1}, true};

	    yield return new object[] {new int[]{1}, new int[]{2}, new int[]{1, 2}, true};
	    yield return new object[] {new int[]{1}, new int[]{2}, new int[]{2, 1}, true};

	    yield return new object[] {new int[]{1, 2}, new int[]{3}, new int[]{1, 2, 3}, true};
	    yield return new object[] {new int[]{1, 2}, new int[]{3}, new int[]{1, 3, 2}, true};
	    yield return new object[] {new int[]{1, 2}, new int[]{3}, new int[]{3, 1, 2}, true};
	    yield return new object[] {new int[]{1, 2}, new int[]{3}, new int[]{2, 1, 3}, false};
	    yield return new object[] {new int[]{1, 2}, new int[]{3}, new int[]{2, 3, 1}, false};
	    yield return new object[] {new int[]{1, 2}, new int[]{3}, new int[]{3, 2, 1}, false};
		
	    yield return new object[] {new int[]{1, 3}, new int[]{2}, new int[]{2, 1, 3}, true};
	    yield return new object[] {new int[]{1, 3}, new int[]{2}, new int[]{1, 2, 3}, true};
	    yield return new object[] {new int[]{1, 3}, new int[]{2}, new int[]{1, 3, 2}, true};
	    yield return new object[] {new int[]{1, 3}, new int[]{2}, new int[]{2, 3, 1}, false};
	    yield return new object[] {new int[]{1, 3}, new int[]{2}, new int[]{3, 2, 1}, false};
	    yield return new object[] {new int[]{1, 3}, new int[]{2}, new int[]{3, 1, 2}, false};
		
	    yield return new object[] {new int[]{2, 3}, new int[]{1}, new int[]{1, 2, 3}, true};
	    yield return new object[] {new int[]{2, 3}, new int[]{1}, new int[]{2, 1, 3}, true};
	    yield return new object[] {new int[]{2, 3}, new int[]{1}, new int[]{2, 3, 1}, true};
	    yield return new object[] {new int[]{2, 3}, new int[]{1}, new int[]{3, 2, 1}, false};
	    yield return new object[] {new int[]{2, 3}, new int[]{1}, new int[]{3, 1, 2}, false};
	    yield return new object[] {new int[]{2, 3}, new int[]{1}, new int[]{1, 3, 2}, false};
		
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{1, 2, 3, 4}, true};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{1, 3, 2, 4}, true};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{1, 3, 4, 2}, true};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{3, 1, 4, 2}, true};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{3, 4, 1, 2}, true};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{2, 1, 3, 4}, false};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{2, 3, 4, 1}, false};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{3, 4, 2, 1}, false};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{1, 2, 4, 3}, false};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{1, 4, 2, 3}, false};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{1, 4, 3, 2}, false};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{4, 1, 3, 2}, false};
	    yield return new object[] {new int[]{1, 2}, new int[]{3, 4}, new int[]{4, 3, 1, 2}, false};
	}
}