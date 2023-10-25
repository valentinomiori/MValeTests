using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MValeTests.AsyncEnumerable;

public class AsyncEnumerableTest
{
    const int plus = 3;
    // [SetUp]
    // public void Setup()
    // {
    // }

    public static async IAsyncEnumerable<int> Subject(
        [System.Runtime.CompilerServices.EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        int i = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(20));

            if (cancellationToken.IsCancellationRequested)
                break;
            
            yield return i++;
        }

        TestContext.Out.WriteLine($"Stopped on {i}.");

        for (int j = 0; j < plus; j++)
            yield return i++;
    }

    [Test]
    public async Task Test1()
    {
        const int max = 3;

        using var cts = new CancellationTokenSource();
        
        await foreach (var i in Subject(cts.Token))
        {
            TestContext.Out.WriteLine($"Got {i}.");

            if (i == max)
            {
                TestContext.Out.WriteLine($"Stopping on {i}.");
                cts.Cancel();
            }

            Assert.LessOrEqual(i, max + plus);
        }
    }
    
    [Test]
    public async Task Test2()
    {
        int max = -1;

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(79));

        int c = 0;
        using var registration = cts.Token.Register(() =>
        {
            TestContext.Out.WriteLine($"Stopping on {c}.");
            max = c;
        });

        await foreach (var i in Subject(cts.Token))
        {
            c = i;

            TestContext.Out.WriteLine($"Got {i}.");

            if (max >= 0)
            {
                Assert.LessOrEqual(i, max + plus);
            }
        }
    }
}