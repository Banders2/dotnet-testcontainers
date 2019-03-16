namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core;
  using DotNet.Testcontainers.Core.Wait;
  using Xunit;

  public class TestcontainersWaitStrategyTest
  {
    public class Finish : IWaitUntil, IWaitWhile
    {
      [Fact]
      public async Task UntilImmediately()
      {
        await WaitStrategy.WaitUntil(() => { return this.Until(string.Empty); });
      }

      [Fact]
      public async Task WhileImmediately()
      {
        await WaitStrategy.WaitWhile(() => { return this.While(string.Empty); });
      }

      public Task<bool> Until(string id)
      {
        return Task.Run(() => true);
      }

      public Task<bool> While(string id)
      {
        return Task.Run(() => false);
      }
    }

    public class Timeout : IWaitUntil, IWaitWhile
    {
      [Fact]
      public async Task UntilAfter1ms()
      {
        await Assert.ThrowsAsync<TimeoutException>(async () =>
        {
          await WaitStrategy.WaitUntil(() => this.Until(string.Empty), timeout: 1);
        });
      }

      [Fact]
      public async Task WhileAfter1ms()
      {
        await Assert.ThrowsAsync<TimeoutException>(async () =>
        {
          await WaitStrategy.WaitWhile(() => this.While(string.Empty), timeout: 1);
        });
      }

      public async Task<bool> Until(string id)
      {
        await Task.Delay(1000);
        return true;
      }

      public async Task<bool> While(string id)
      {
        await Task.Delay(1000);
        return false;
      }
    }
  }
}
