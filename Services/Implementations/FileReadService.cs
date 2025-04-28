using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Dto.Responses;
using Services.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Utilities.LogHelper;
using Utilities.Utilities;

public class FileService(
    ILogger<FileService> logger,
    IConfiguration config
    ) : IFileService
{
    private readonly ILogger<FileService> _logger = logger;
    private readonly IConfiguration _config = config;

    #region///1. 從資料夾 A 複製今日的檔案到資料夾 B 2. 只複製名稱開頭符合指定字串(StartsWith) 3. 若資料夾 B 不存在則建立 4. 如果今天過了，刪除 B 資料夾內的昨日檔案
    /// <summary>
    ///1. 從資料夾 A 複製今日的檔案到資料夾 B
    ///2. 只複製名稱開頭符合指定字串(StartsWith)
    ///3. 若資料夾 B 不存在則建立
    ///4. 如果今天過了，刪除 B 資料夾內的昨日檔案
    /// </summary>
    /// <param name="jobGuid"></param>
    /// <param name="sourcePath"></param>
    /// <param name="targetPath"></param>
    /// <param name="startsWithFileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> CopyTodayFilesAsync(
        string jobGuid,
        string sourcePath,
        string targetPath,
        string startsWithFileName,
        CancellationToken cancellationToken
        )
    {
        var logSource = $"【{jobGuid}】" + jobGuid + LogHelper.Build<FileService>();

        _logger.LogInformation($"{logSource} 程式執行開始");
        try
        {
            #region // 檢查 輸入參數
            if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
            {
                _logger.LogWarning($"{logSource} 未找到來源資料夾或路徑為空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                _logger.LogWarning($"{logSource} 目標資料夾路徑為空");
                return false;
            }
            if (string.IsNullOrWhiteSpace(startsWithFileName))
            {
                _logger.LogWarning($"{logSource} 檔案名稱起始字串為空");
                return false;
            }
            #endregion

            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            var todayFilePrefix = startsWithFileName.Replace("YYYY-MM-DD", today.ToString("yyyy-MM-dd"));
            var yesterdayFilePrefix = startsWithFileName.Replace("YYYY-MM-DD", yesterday.ToString("yyyy-MM-dd"));

            // 建立目的地資料夾
            if (!Directory.Exists(targetPath))
            {
                _logger.LogInformation($"{logSource} 未找到目的地資料夾，準備建立資料夾：{targetPath}");
                Directory.CreateDirectory(targetPath);
                _logger.LogInformation($"{logSource} 已建立資料夾：{targetPath}");
            }

            // 尋找今日檔案
            var todayFiles = Directory.GetFiles(sourcePath)
                .Where(f => Path.GetFileName(f).StartsWith(todayFilePrefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!todayFiles.Any())
            {
                _logger.LogError($"{logSource} 未找到【{todayFilePrefix}】開頭的檔案，無法進行複製");
                return false;
            }

            _logger.LogInformation($"{logSource} 共找到 {todayFiles.Count} 個【{todayFilePrefix}】開頭的檔案，準備複製...");

            // 複製檔案
            foreach (var file in todayFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(targetPath, fileName);

                try
                {
                    _logger.LogInformation($"{logSource} 開始複製：{fileName} -> {destFile}");

                    await using var sourceStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    await using var destStream = new FileStream(destFile, FileMode.Create, FileAccess.Write);
                    await sourceStream.CopyToAsync(destStream, cancellationToken).ConfigureAwait(false);

                    _logger.LogInformation($"{logSource} 完成複製：{fileName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{logSource} 複製檔案失敗：{fileName}，錯誤訊息：{ex.Message}");
                }
            }

            // 刪除昨天的檔案
            var yesterdayFiles = Directory.GetFiles(targetPath)
                .Where(f => Path.GetFileName(f).StartsWith(yesterdayFilePrefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!yesterdayFiles.Any())
            {
                _logger.LogInformation($"{logSource} 未找到【{yesterdayFilePrefix}】開頭的檔案，無需刪除");
            }
            else
            {
                _logger.LogInformation($"{logSource} 共找到 {yesterdayFiles.Count} 個【{yesterdayFilePrefix}】開頭的檔案，準備刪除...");

                foreach (var file in yesterdayFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var fileName = Path.GetFileName(file);
                    try
                    {
                        File.Delete(file);
                        _logger.LogInformation($"{logSource} 成功刪除檔案：{fileName}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{logSource} 刪除檔案失敗：{fileName}，錯誤訊息：{ex.Message}");
                    }
                }
            }

            _logger.LogInformation($"{logSource} 程式執行 任務完成");
            return true;
        }
        catch (OperationCanceledException ocex)
        {
            _logger.LogError($"{logSource} 被取消，訊息：{ocex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{logSource} 發生錯誤：{ex.Message}");
            return false;
        }
        finally
        {
            _logger.LogInformation($"{logSource} 程式執行結束");
        }
    }
    #endregion

    #region (不要用，檔案過大字串會被截斷)
    /// <summary>
    /// (不要用，檔案過大字串會被截斷) 取得當天日期，指定位置，指定檔案(startsWithFileName名稱)檔案內容
    /// </summary>
    /// <param name="jobGuid"></param>
    /// <param name="directory"></param>
    /// <param name="startsWithFileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>每個檔案文字內容</returns>
    public async Task<string?> GetTodayFileContentAsync(
        string jobGuid,
        string directory,
        string startsWithFileName,
        CancellationToken cancellationToken
        )
    {
        var logSource = $"【{jobGuid}】" + LogHelper.Build<FileService>();

        _logger.LogInformation($"{logSource} 程式執行開始");

        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
        {
            _logger.LogWarning($"{logSource} 未找到讀取資料夾或路徑為空");
            return null;
        }

        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var todayStartsWith = startsWithFileName.Replace("YYYY-MM-DD", today);

        var files = await Task.Run(() =>
        {
            return Directory.EnumerateFiles(directory)
                .Where(f => Path.GetFileName(f).StartsWith(todayStartsWith, StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => Path.GetFileName(f), new SortUtil.NaturalSort())
                .ToList();
        }, cancellationToken).ConfigureAwait(false);

        if (!files.Any())
        {
            _logger.LogError($"{logSource} 未找到符合【{todayStartsWith}】開頭的檔案，無法讀取內容");
            return null;
        }

        _logger.LogInformation($"{logSource} 共找到 {files.Count} 個檔案，開始讀取並合併內容");

        var contentBuilder = new StringBuilder();

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var fileName = Path.GetFileName(file);

            try
            {
                _logger.LogInformation($"{logSource} 讀取檔案：{fileName}");

                await using var stream = new FileStream(
                    file,
                    new FileStreamOptions
                    {
                        Mode = FileMode.Open,
                        Access = FileAccess.Read,
                        Share = FileShare.ReadWrite,
                        Options = FileOptions.Asynchronous,
                        BufferSize = 4096
                    });

                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

                contentBuilder.AppendLine(content);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{logSource} 讀取檔案失敗：{fileName}，錯誤訊息：{ex.Message}");
            }
        }

        _logger.LogInformation($"{logSource} 程式執行結束");

        var combinedContent = contentBuilder.ToString();
        return string.IsNullOrWhiteSpace(combinedContent) ? null : combinedContent;
    }

    /// <summary>
    /// (不要用，檔案過大字串會被截斷) 分段處理 取得當天日期，指定位置，指定檔案(startsWithFileName名稱)檔案內容
    /// </summary>
    /// <param name="jobGuid"></param>
    /// <param name="directory"></param>
    /// <param name="startsWithFileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string?> GetFileContentChunkAsync(
        string jobGuid,
        string directory,
        string startsWithFileName,
        CancellationToken cancellationToken
    )
    {
        var logSource = $"【{jobGuid}】" + LogHelper.Build<FileService>();

        var swTotal = Stopwatch.StartNew();
        _logger.LogInformation($"{logSource} 程式執行開始 - 目錄: {directory}, 檔案名稱起始字串: {startsWithFileName}");

        if (!Directory.Exists(directory))
        {
            _logger.LogError($"{logSource} 資料夾不存在: {directory}");
            return null;
        }

        var swPre = Stopwatch.StartNew();
        string today = DateTime.Today.ToString("yyyy-MM-dd");
        startsWithFileName = startsWithFileName.Replace("YYYY-MM-DD", today);
        _logger.LogInformation($"{logSource} 已轉換檔案名稱關鍵字: {startsWithFileName}");

        var files = Directory.EnumerateFiles(directory)
            .Where(f => Path.GetFileName(f).StartsWith(startsWithFileName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => Path.GetFileName(f), new SortUtil.NaturalSort())
            .ToList();

        swPre.Stop();
        _logger.LogInformation($"{logSource} 檔案搜尋耗時: {swPre.ElapsedMilliseconds} ms");

        if (!files.Any())
        {
            _logger.LogError($"{logSource} 找不到符合條件的檔案: {startsWithFileName}");
            return null;
        }

        _logger.LogInformation($"{logSource} 找到 {files.Count} 個符合條件的檔案");

        int size = _config.GetValue<int>("FileChunkSetting:ChunkSize");
        var unitStr = _config.GetValue<string>("FileChunkSetting:ChunkSizeUnit");

        if (!Enum.TryParse<FileSizeUtil.FileSize>(unitStr, out var unit))
        {
            _logger.LogError($"{logSource} 設定檔案單位無效: {unitStr}");
            throw new ArgumentException("設定檔案單位無效: Invalid ChunkSizeUnit");
        }

        int maxBytes = FileSizeUtil.ConvertToBytes(size, unit);
        _logger.LogInformation($"{logSource} 每個 chunk 的最大大小: {maxBytes} bytes");

        var swProcessing = Stopwatch.StartNew();

        var tasks = files.Select(file =>
        {
            return Task.Run(() => ProcessFileAsync(jobGuid, file, maxBytes, cancellationToken));
        });

        var chunks = await Task.WhenAll(tasks).ConfigureAwait(false);

        swProcessing.Stop();
        _logger.LogInformation($"{logSource} 所有檔案處理耗時: {swProcessing.ElapsedMilliseconds} ms");

        swTotal.Stop();
        _logger.LogInformation($"{logSource} 程式總耗時: {swTotal.ElapsedMilliseconds} ms");
        _logger.LogInformation($"{logSource} 程式執行結束");

        return string.Join(Environment.NewLine, chunks.Where(chunk => !string.IsNullOrWhiteSpace(chunk)));
    }

    /// <summary>
    /// 平行處理
    /// </summary>
    /// <param name="jobGuid"></param>
    /// <param name="file"></param>
    /// <param name="maxBytes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<string?> ProcessFileAsync(string jobGuid, string file, int maxBytes, CancellationToken cancellationToken)
    {
        var logSource = $"【{jobGuid}】" + LogHelper.Build<FileService>();
        var sw = Stopwatch.StartNew();
        _logger.LogInformation($"{logSource} 開始處理檔案: {file}");

        var chunks = new List<string>();

        try
        {
            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 81920, useAsync: true);
            using var reader = new StreamReader(stream, Encoding.UTF8);

            int currentByte = 0;
            var chunkBuilder = new StringBuilder();

            while (!reader.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = await reader.ReadLineAsync().ConfigureAwait(false);
                if (line == null) continue;

                var lineBytes = Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;

                if (currentByte + lineBytes > maxBytes)
                {
                    chunks.Add(chunkBuilder.ToString());
                    chunkBuilder.Clear();
                    currentByte = 0;
                }

                chunkBuilder.AppendLine(line);
                currentByte += lineBytes;
            }

            if (chunkBuilder.Length > 0)
            {
                chunks.Add(chunkBuilder.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{logSource} 處理檔案 {file} 時發生例外");
        }

        sw.Stop();
        _logger.LogInformation($"{logSource} 檔案 {file} 處理完成，共產生 {chunks.Count} 個 chunk，耗時: {sw.ElapsedMilliseconds} ms");

        return string.Join(Environment.NewLine, chunks);
    }
    #endregion

    #region 分段處理List 取得當天日期，指定位置，指定檔案(startsWithFileName名稱)檔案內容
    /// <summary>
    /// 分段處理List 取得當天日期，指定位置，指定檔案(startsWithFileName名稱)檔案內容
    /// </summary>
    /// <param name="jobGuid"></param>
    /// <param name="directory"></param>
    /// <param name="startsWithFileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<string>?> GetFileContentChunkListAsync(
        string jobGuid,
        string directory,
        string startsWithFileName,
        CancellationToken cancellationToken
    )
    {
        var logSource = $"【{jobGuid}】" + LogHelper.Build<FileService>();
        var swTotal = Stopwatch.StartNew();
        var result = new List<string>();

        _logger.LogInformation($"{logSource} 程式執行開始 - 目錄: {directory}, 檔案名稱起始字串: {startsWithFileName}");

        #region // 檢查 輸入參數
        if (!Directory.Exists(directory))
        {
            _logger.LogError($"{logSource} 資料夾不存在: {directory}");
            return result;
        }
        if (string.IsNullOrWhiteSpace(startsWithFileName))
        {
            _logger.LogWarning($"{logSource} 檔案名稱起始字串為空");
            return result;
        }
        #endregion

        var swPre = Stopwatch.StartNew();
        string today = DateTime.Today.ToString("yyyy-MM-dd");
        startsWithFileName = startsWithFileName.Replace("YYYY-MM-DD", today);
        _logger.LogInformation($"{logSource} 已轉換檔案名稱關鍵字: {startsWithFileName}");

        var files = Directory.EnumerateFiles(directory)
            .Where(f => Path.GetFileName(f).StartsWith(startsWithFileName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => Path.GetFileName(f), new SortUtil.NaturalSort())
            .ToList();

        swPre.Stop();
        _logger.LogInformation($"{logSource} 檔案搜尋耗時: {swPre.ElapsedMilliseconds} ms");

        if (!files.Any())
        {
            _logger.LogError($"{logSource} 找不到符合條件的檔案: {startsWithFileName}");
            return result;
        }

        _logger.LogInformation($"{logSource} 找到 {files.Count} 個符合條件的檔案");

        int size = _config.GetValue<int>("FileChunkSetting:ChunkSize");
        var unitStr = _config.GetValue<string>("FileChunkSetting:ChunkSizeUnit");

        if (!Enum.TryParse<FileSizeUtil.FileSize>(unitStr, out var unit))
        {
            _logger.LogError($"{logSource} 設定檔案單位無效: {unitStr}");
            throw new ArgumentException("設定檔案單位無效: Invalid ChunkSizeUnit");
        }

        int maxBytes = FileSizeUtil.ConvertToBytes(size, unit);
        _logger.LogInformation($"{logSource} 每個 chunk 的最大大小: {maxBytes} bytes");

        var swProcessing = Stopwatch.StartNew();

        // 加入 index 保留順序
        var indexedTasks = files.Select((file, index) =>
            Task.Run(async () => new
            {
                Index = index,
                Chunks = await ProcessFileListAsync(jobGuid, file, maxBytes, cancellationToken).ConfigureAwait(false)
            }, cancellationToken)
        );

        var chunkResults = await Task.WhenAll(indexedTasks).ConfigureAwait(false);

        swProcessing.Stop();
        _logger.LogInformation($"{logSource} 所有檔案處理耗時: {swProcessing.ElapsedMilliseconds} ms");

        swTotal.Stop();
        _logger.LogInformation($"{logSource} 程式執行結束 - 程式總耗時: {swTotal.ElapsedMilliseconds} ms");

        // 依照 index 合併所有 chunk
        result = chunkResults
            .OrderBy(x => x.Index)
            .SelectMany(x => x.Chunks)
            .Where(chunk => !string.IsNullOrWhiteSpace(chunk))
            ?.ToList() ?? [];

        return result;
    }


    /// <summary>
    /// 平行處理List
    /// </summary>
    /// <param name="jobGuid"></param>
    /// <param name="file"></param>
    /// <param name="maxBytes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<List<string>> ProcessFileListAsync(string jobGuid, string file, int maxBytes, CancellationToken cancellationToken)
    {
        var logSource = $"【{jobGuid}】" + LogHelper.Build<FileService>();
        var sw = Stopwatch.StartNew();
        _logger.LogInformation($"{logSource} 開始處理檔案: {file}");

        var chunks = new List<string>();

        try
        {
            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 81920, useAsync: true);
            using var reader = new StreamReader(stream, Encoding.UTF8);

            int currentByte = 0;
            var chunkBuilder = new StringBuilder();

            while (!reader.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = await reader.ReadLineAsync().ConfigureAwait(false);
                if (line == null) continue;

                var lineBytes = Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;

                if (currentByte + lineBytes > maxBytes)
                {
                    chunks.Add(chunkBuilder.ToString());
                    chunkBuilder.Clear();
                    currentByte = 0;
                }

                chunkBuilder.AppendLine(line);
                currentByte += lineBytes;
            }

            if (chunkBuilder.Length > 0)
            {
                chunks.Add(chunkBuilder.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{logSource} 處理檔案 {file} 時發生例外");
        }

        sw.Stop();
        _logger.LogInformation($"{logSource} 檔案 {file} 處理完成，共產生 {chunks.Count} 個 chunk，耗時: {sw.ElapsedMilliseconds} ms");

        return chunks;
    }

    #endregion

    #region 拆解文字檔，找出段落
    /// <summary>
    /// 拆解文字檔，找出段落
    /// </summary>
    /// <param name="jobGuid"></param>
    /// <param name="logContent">文字內容</param>
    /// <param name="cancellationToken"></param>
    /// <returns>寫入完成之序號</returns>
    public async Task<MailhunterLogParseResponse> MailhunterLogParseLogAsync(string jobGuid, string logContent, CancellationToken cancellationToken)
    {
        var logSource = $"【{jobGuid}】" + LogHelper.Build<FileService>();
        _logger.LogInformation($"{logSource} 程式執行開始");

        var result = new MailhunterLogParseResponse();
        var completedJobs = new ConcurrentBag<string>();

        if (string.IsNullOrWhiteSpace(logContent))
        {
            _logger.LogError($"{logSource} 傳入的 文字內容 為空 (參數名logContent)");
            return result;
        }

        var lines = logContent.Split(["\r\n", "\n"], StringSplitOptions.None);
        _logger.LogInformation($"{logSource} 共有 {lines.Length} 行待處理");

        var tasks = new List<Task>();

        foreach (var line in lines)
        {
            tasks.Add(Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                // thread-local 變數
                string? currentJobId = null;
                bool jobStarted = false;

                if (line.Contains("找到檔案:"))
                {
                    var match = Regex.Match(line, @"control-(\d+-\d+)\.ok");
                    if (match.Success)
                    {
                        currentJobId = match.Groups[1].Value;
                        jobStarted = true;
                        _logger.LogInformation($"{logSource} 找到檔案 - currentJobId: {currentJobId}");
                    }
                }
                else if (line.Contains("寫入完成"))
                {
                    // 如果前面有找到 jobId 才加入
                    if (jobStarted && !string.IsNullOrWhiteSpace(currentJobId))
                    {
                        completedJobs.Add(currentJobId);
                        _logger.LogInformation($"{logSource} 寫入完成 - currentJobId: {currentJobId}");
                    }
                    else
                    {
                        _logger.LogInformation($"{logSource} 找到 '寫入完成' 但無有效 ，Line：{line}");
                    }
                }

            }, cancellationToken));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        result.CompletedJobs = completedJobs?.ToList() ?? [];

        _logger.LogInformation($"{logSource} 程式執行結束 - 共解析出 {result?.CompletedJobs?.Count} 筆完成的");

        return result ?? new MailhunterLogParseResponse();
    }
    #endregion

    #region 拆解List文字檔，找出段落
    /// <summary>
    /// 拆解List文字檔，找出段落
    /// </summary>
    /// <param name="jobGuid"></param>
    /// <param name="logContentList">文字內容清單（每個 chunk）</param>
    /// <param name="_config"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>寫入完成之序號</returns>
    public async Task<MailhunterLogParseResponse> MailhunterLogParseLogListAsync(
        string jobGuid,
        List<string> logContentList,
        CancellationToken cancellationToken
        )
    {
        var logSource = $"【{jobGuid}】" + LogHelper.Build<FileService>();
        _logger.LogInformation($"{logSource} 程式執行開始");

        var result = new MailhunterLogParseResponse();
        var completedJobs = new ConcurrentBag<string>();

        #region // 檢查 輸入參數
        if (logContentList == null || !logContentList.Any())
        {
            _logger.LogError($"{logSource} 傳入的 logContentList 為空");
            return result;
        }
        #endregion

        _logger.LogInformation($"{logSource} 共有 {logContentList.Count} 個 chunks");

        string? currentJobId = null;

        foreach (var chunk in logContentList)
        {
            using var reader = new StringReader(chunk);
            string? line;

            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                line = line.Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.Contains("找到檔案:"))
                {
                    var match = Regex.Match(line, @"control-(\d+-\d+)\.ok");
                    if (match.Success)
                    {
                        currentJobId = match.Groups[1].Value;
                        _logger.LogInformation($"{logSource} 找到檔案 - currentJobId: {currentJobId}");
                    }
                }
                else if (line.Contains("寫入完成"))
                {
                    if (!string.IsNullOrWhiteSpace(currentJobId))
                    {
                        completedJobs.Add(currentJobId);
                        _logger.LogInformation($"{logSource} 寫入完成 - currentJobId: {currentJobId}");
                        currentJobId = null;
                    }
                    else
                    {
                        _logger.LogWarning($"{logSource} 找到 '寫入完成' 但無對應 jobId，Line：{line}");
                    }
                }
            }
        }

        // 去除 null/空白 並去重
        result.CompletedJobs = completedJobs
            ?.Where(x => !string.IsNullOrWhiteSpace(x))
            ?.Distinct()
            ?.ToList()
            ?? new List<string>();

        _logger.LogInformation($"{logSource} 程式執行結束 - 共解析出 {result.CompletedJobs.Count} 筆完成的");

        return result;
    }

    #endregion


}
