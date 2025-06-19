using FluentAssertions;
using k8s.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Implementations;

namespace xUnitTest
{
    public class FileServiceTests
    {
        private readonly Mock<ILogger<FileService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly FileService _service;
        public FileServiceTests()
        {
            _mockLogger = new Mock<ILogger<FileService>>();
            _mockConfig = new Mock<IConfiguration>();
            _service = new FileService(_mockLogger.Object, _mockConfig.Object);
        }

        [Fact]
        public async Task MailhunterLogParseLogListAsync_ShouldParseCompletedJobsCorrectly()
        {
            var cancellationToken = CancellationToken.None;
            var jobGuid = Guid.NewGuid().ToString();

            var logs = new List<string>
            {
                // 模擬 chunk 內容
                """
                找到檔案: control-1234-5678.ok
                寫入完成
                """,
                """
                找到檔案: control-1234-9999.ok
                寫入完成
                """,
                """
                找到檔案: control-1234-5678.ok
                寫入完成
                """
            };

            // Act
            var result = await _service.MailhunterLogParseLogListAsync(jobGuid, logs, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.CompletedJobs.Should().BeEquivalentTo(new[] { "1234-5678", "1234-9999" });
            result.CompletedJobs.Count.Should().Be(2);
        }

        [Fact]
        public async Task MailhunterLogParseLogListAsync_WithEmptyLogList_ShouldReturnEmpty()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _service.MailhunterLogParseLogListAsync("guid", new List<string>(), cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task MailhunterLogParseLogListAsync_WithNull_ShouldReturnEmpty()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _service.MailhunterLogParseLogListAsync("guid", null, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.CompletedJobs.Should().BeEmpty();
        }


        [Fact]
        public async Task LogOnlyHasWriteCompleteButNoJobId_ShouldNotAdd()
        {
            var cancellationToken = CancellationToken.None;
            var logs = new List<string> { "寫入完成" };
            var result = await _service.MailhunterLogParseLogListAsync("job123", logs, cancellationToken);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task JobIdFoundButNoWriteComplete_ShouldNotAdd()
        {
            var cancellationToken = CancellationToken.None;
            var logs = new List<string> { "找到檔案: control-1111-2222.ok" };
            var result = await _service.MailhunterLogParseLogListAsync("job123", logs, cancellationToken);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task CancellationToken_ShouldThrowOperationCanceledException()
        {
            var cts = new CancellationTokenSource();
            var logs = new List<string> { "找到檔案: control-1111-2222.ok\n寫入完成" };
            cts.Cancel(); // 提前取消

            Func<Task> action = async () => await _service.MailhunterLogParseLogListAsync("job123", logs, cts.Token);

            await action.Should().ThrowAsync<OperationCanceledException>();
        }


        [Fact]
        public async Task WithEmptyLogList_ShouldReturnEmpty()
        {
            var result = await _service.MailhunterLogParseLogListAsync("guid", new List<string>(), CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task WithNull_ShouldReturnEmpty()
        {
            var result = await _service.MailhunterLogParseLogListAsync("guid", null, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task OneChunk_CorrectFormat_ShouldReturnOneJob()
        {
            var logs = new List<string> { "找到檔案: control-1234-5678.ok\n寫入完成" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().ContainSingle().Which.Should().Be("1234-5678");
        }

        [Fact]
        public async Task MultipleChunks_ShouldReturnMultipleJobs()
        {
            var logs = new List<string>
        {
            "找到檔案: control-1111-2222.ok\n寫入完成",
            "找到檔案: control-3333-4444.ok\n寫入完成"
        };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEquivalentTo("1111-2222", "3333-4444");
        }

        [Fact]
        public async Task DuplicateJobIds_ShouldBeDeduplicated()
        {
            var logs = new List<string>
        {
            "找到檔案: control-1234-5678.ok\n寫入完成",
            "找到檔案: control-1234-5678.ok\n寫入完成"
        };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().ContainSingle().Which.Should().Be("1234-5678");
        }

        [Fact]
        public async Task FoundJobIdButNoWriteComplete_ShouldNotAdd()
        {
            var logs = new List<string> { "找到檔案: control-1111-2222.ok" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task WriteCompleteWithoutJobId_ShouldNotAdd()
        {
            var logs = new List<string> { "寫入完成" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task OnlyWhitespaceLines_ShouldReturnEmpty()
        {
            var logs = new List<string> { "     ", "\t", "\n" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task IrrelevantText_ShouldIgnoreLines()
        {
            var logs = new List<string> { "這是一行不相關的文字" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task MalformedJobId_ShouldIgnore()
        {
            var logs = new List<string> { "找到檔案: control-ABC-XYZ.ok\n寫入完成" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task RegexMatchReturnsEmpty_ShouldIgnore()
        {
            var logs = new List<string> { "找到檔案: control-.ok\n寫入完成" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task JobIdOverwrittenBeforeWriteComplete_ShouldAddLatestOnly()
        {
            var logs = new List<string>
        {
            "找到檔案: control-1111-2222.ok",
            "找到檔案: control-9999-8888.ok",
            "寫入完成"
        };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().ContainSingle().Which.Should().Be("9999-8888");
        }

        [Fact]
        public async Task JobIdResetAfterWriteComplete_ShouldBeCleared()
        {
            var logs = new List<string>
        {
            "找到檔案: control-1234-5678.ok",
            "寫入完成",
            "寫入完成"
        };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().ContainSingle().Which.Should().Be("1234-5678");
        }

        [Fact]
        public async Task TrimmedWhitespace_ShouldNotAffectParsing()
        {
            var logs = new List<string> { "  找到檔案: control-1111-2222.ok   \n   寫入完成  " };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().ContainSingle().Which.Should().Be("1111-2222");
        }

        [Fact]
        public async Task CancellationRequested_ShouldThrow()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var logs = new List<string> { "找到檔案: control-1111-2222.ok\n寫入完成" };
            Func<Task> act = async () => await _service.MailhunterLogParseLogListAsync("guid", logs, cts.Token);
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task ControlFileSplitAcrossChunks_ShouldBeParsedCorrectly()
        {
            var logs = new List<string>
        {
            "找到檔案: control-4444-5555.ok",
            "寫入完成"
        };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().ContainSingle().Which.Should().Be("4444-5555");
        }

        [Fact]
        public async Task WriteCompleteInPreviousChunk_ShouldNotRetainOldJobId()
        {
            var logs = new List<string>
        {
            "找到檔案: control-1111-2222.ok\n寫入完成",
            "寫入完成"
        };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().ContainSingle().Which.Should().Be("1111-2222");
        }

        [Fact]
        public async Task JobIdCaseSensitivity_ShouldMatchExactly()
        {
            var logs = new List<string> { "找到檔案: CONTROL-1234-5678.ok\n寫入完成" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty(); // Regex 是大小寫敏感
        }

        [Fact]
        public async Task JobIdFormatWithExtraDashes_ShouldIgnore()
        {
            var logs = new List<string> { "找到檔案: control-1234-5678-9999.ok\n寫入完成" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task JobIdWithoutControlPrefix_ShouldIgnore()
        {
            var logs = new List<string> { "找到檔案: 1234-5678.ok\n寫入完成" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task GarbledCharacters_ShouldBeHandledGracefully()
        {
            var logs = new List<string> { "找▲到檔案: control-12??-####.ok\n寫入完成" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty();
        }

        [Fact]
        public async Task FoundAndWriteInSameLine_ShouldBeSkipped()
        {
            var logs = new List<string> { "找到檔案: control-1234-5678.ok 寫入完成" };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEmpty(); // 因為沒有換行不會進入正確流程
        }

        [Fact]
        public async Task MultipleJobIdsInOneChunk_ShouldAllBeParsed()
        {
            var logs = new List<string>
        {
            "找到檔案: control-1234-1111.ok\n寫入完成\n找到檔案: control-1234-2222.ok\n寫入完成"
        };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEquivalentTo("1234-1111", "1234-2222");
        }

        [Fact]
        public async Task OnlyLastJobIdWithoutWriteComplete_ShouldNotBeAdded()
        {
            var logs = new List<string>
        {
            "找到檔案: control-1234-1111.ok\n寫入完成",
            "找到檔案: control-9999-0000.ok"
        };
            var result = await _service.MailhunterLogParseLogListAsync("guid", logs, CancellationToken.None);
            result.CompletedJobs.Should().BeEquivalentTo("1234-1111");
        }
    }
}