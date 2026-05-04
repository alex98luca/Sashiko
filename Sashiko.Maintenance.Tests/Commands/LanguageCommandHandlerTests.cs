using Sashiko.Maintenance.Commands;
using Xunit;

namespace Sashiko.Maintenance.Tests.Commands
{
	[Collection("NonParallel")]
	public sealed class LanguageCommandHandlerTests
	{
		[Fact]
		public async Task DispatchAsync_Update_ShouldInvokeHook()
		{
			// Arrange
			bool called = false;

			var handler = new LanguageCommandHandler
			{
				UpdateHook = () =>
				{
					called = true;
					return Task.CompletedTask;
				}
			};

			// Act
			await handler.DispatchAsync("update");

			// Assert
			Assert.True(called);
		}

		[Fact]
		public async Task DispatchAsync_Update_ShouldNotThrow_WhenHookIsNull()
		{
			// Arrange
			var handler = new LanguageCommandHandler
			{
				UpdateHook = null
			};

			// Act
			var exception = await Record.ExceptionAsync(() => handler.DispatchAsync("update"));

			// Assert
			Assert.Null(exception);
		}

		[Fact]
		public async Task DispatchAsync_UnknownCommand_ShouldPrintMessage()
		{
			// Arrange
			using var sw = new StringWriter();
			Console.SetOut(sw);

			var handler = new LanguageCommandHandler
			{
				UpdateHook = () => Task.CompletedTask
			};

			// Act
			await handler.DispatchAsync("invalid");

			// Assert
			var output = sw.ToString();
			Assert.Contains("Unknown languages command", output);
			Assert.Contains("Available: update", output);
		}

		[Fact]
		public async Task DispatchAsync_NullCommand_ShouldPrintMessage()
		{
			// Arrange
			using var sw = new StringWriter();
			Console.SetOut(sw);

			var handler = new LanguageCommandHandler
			{
				UpdateHook = () => Task.CompletedTask
			};

			// Act
			await handler.DispatchAsync(null);

			// Assert
			var output = sw.ToString();
			Assert.Contains("Unknown languages command", output);
		}
	}
}
