using Sashiko.Maintenance.Commands;

namespace Sashiko.Maintenance.Tests.Commands
{
	[Collection("NonParallel")]
	public sealed class NamesCommandHandlerTests
	{
		[Fact]
		public async Task DispatchAsync_Polish_ShouldInvokeHook()
		{
			bool called = false;

			var handler = new NamesCommandHandler
			{
				PolishHook = () =>
				{
					called = true;
					return Task.CompletedTask;
				}
			};

			var exception = await Record.ExceptionAsync(() => handler.DispatchAsync("polish"));

			Assert.Null(exception);

			Assert.True(called);
		}

		[Fact]
		public async Task DispatchAsync_Polish_ShouldNotThrow_WhenHookIsNull()
		{
			var handler = new NamesCommandHandler
			{
				PolishHook = null
			};

			var exception = await Record.ExceptionAsync(() => handler.DispatchAsync("polish"));

			Assert.Null(exception);
		}

		[Fact]
		public async Task DispatchAsync_UnknownCommand_ShouldPrintMessage()
		{
			var originalOut = Console.Out;
			using var sw = new StringWriter();
			Console.SetOut(sw);

			try
			{
				var handler = new NamesCommandHandler
				{
					PolishHook = () => Task.CompletedTask
				};

				await handler.DispatchAsync("invalid");

				var output = sw.ToString();
				Assert.Contains("Unknown names command", output);
				Assert.Contains("Available: polish", output);
			}
			finally
			{
				Console.SetOut(originalOut);
			}
		}

		[Fact]
		public async Task DispatchAsync_NullCommand_ShouldPrintMessage()
		{
			var originalOut = Console.Out;
			using var sw = new StringWriter();
			Console.SetOut(sw);

			try
			{
				var handler = new NamesCommandHandler
				{
					PolishHook = () => Task.CompletedTask
				};

				await handler.DispatchAsync(null);

				var output = sw.ToString();
				Assert.Contains("Unknown names command", output);
			}
			finally
			{
				Console.SetOut(originalOut);
			}
		}
	}
}
