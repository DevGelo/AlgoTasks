<Query Kind="Program">
  <NuGetReference>BenchmarkDotNet</NuGetReference>
  <NuGetReference>BenchmarkDotNet.Diagnostics.Windows</NuGetReference>
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>BenchmarkDotNet.Diagnostics.Windows</Namespace>
  <Namespace>BenchmarkDotNet.Diagnostics.Windows.Configs</Namespace>
  <Namespace>BenchmarkDotNet.Jobs</Namespace>
  <Namespace>BenchmarkDotNet.Running</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

void Main()
{
	var summary = BenchmarkRunner.Run<Md5VsSha256>();
}

[SimpleJob(launchCount: 1, warmupCount: 3, targetCount: 5, invocationCount: 100)]
[MemoryDiagnoser]
public class Md5VsSha256
{
	private const int N = 10000;
	private readonly byte[] data;

	private readonly SHA256 sha256 = SHA256.Create();
	private readonly MD5 md5 = MD5.Create();

	public Md5VsSha256()
	{
		data = new byte[N];
		new Random(42).NextBytes(data);
	}

	[Benchmark]
	public byte[] Sha256() => sha256.ComputeHash(data);

	[Benchmark]
	public byte[] Md5() => md5.ComputeHash(data);
}