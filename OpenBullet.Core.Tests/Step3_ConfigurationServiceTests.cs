using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Models;
using OpenBullet.Core.Services;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 3 Tests: Configuration Service
/// </summary>
public class Step3_ConfigurationServiceTests : IDisposable
{
    private readonly Mock<ILogger<ConfigurationService>> _loggerMock;
    private readonly ConfigurationService _configurationService;
    private readonly string _testDataDirectory;
    private readonly string _tempDirectory;

    public Step3_ConfigurationServiceTests()
    {
        _loggerMock = new Mock<ILogger<ConfigurationService>>();
        _configurationService = new ConfigurationService(_loggerMock.Object);
        _testDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
        _tempDirectory = Path.Combine(Path.GetTempPath(), "OpenBulletTests", Guid.NewGuid().ToString());
        
        // Create temp directory
        Directory.CreateDirectory(_tempDirectory);
    }

    [Fact]
    public void ConfigurationService_Should_Initialize_Successfully()
    {
        // Assert
        _configurationService.Should().NotBeNull();
        _configurationService.Should().BeAssignableTo<IConfigurationService>();
    }

    [Fact]
    public async Task LoadConfigAsync_With_Valid_File_Should_Parse_Correctly()
    {
        // Arrange
        var validConfigPath = Path.Combine(_testDataDirectory, "valid_config.anom");
        
        // Act
        var config = await _configurationService.LoadConfigAsync(validConfigPath);

        // Assert
        config.Should().NotBeNull();
        config.Name.Should().Be("Test Configuration");
        config.Author.Should().Be("TestAuthor");
        config.Version.Should().Be("1.2.3");
        config.AdditionalInfo.Should().Be("This is a test configuration for unit testing");
        
        // Settings assertions
        config.Settings.SuggestedBots.Should().Be(50);
        config.Settings.NeedsProxies.Should().BeTrue();
        config.Settings.OnlySsl.Should().BeTrue();
        config.Settings.MaxProxyUses.Should().Be(10);
        config.Settings.Grayscale.Should().BeTrue();
        config.Settings.RemoveNoise.Should().BeTrue();
        config.Settings.Threshold.Should().Be(0.8);
        config.Settings.Contrast.Should().Be(1.2);
        config.Settings.CustomUserAgent.Should().Be("TestAgent/1.0");
        config.Settings.ForceHeadless.Should().BeTrue();
        config.Settings.DataRules.Should().HaveCount(2);
        config.Settings.CustomInputs.Should().HaveCount(1);
        
        // Script assertion
        config.Script.Should().NotBeEmpty();
        config.Script.Should().Contain("REQUEST POST \"https://example.com/login\"");
        config.Script.Should().Contain("KEYCHECK");
    }

    [Fact]
    public async Task LoadConfigAsync_With_Minimal_File_Should_Parse_With_Defaults()
    {
        // Arrange
        var minimalConfigPath = Path.Combine(_testDataDirectory, "minimal_config.anom");
        
        // Act
        var config = await _configurationService.LoadConfigAsync(minimalConfigPath);

        // Assert
        config.Should().NotBeNull();
        config.Name.Should().Be("Minimal Config");
        config.Author.Should().Be("TestUser");
        config.Version.Should().Be("1.0.0");
        config.Settings.SuggestedBots.Should().Be(1);
        
        // Defaults should be applied
        config.Settings.MaxRedirects.Should().Be(8); // Default value
        config.Settings.NeedsProxies.Should().BeFalse(); // Default value
        config.Script.Should().Be("PRINT Hello World");
    }

    [Fact]
    public async Task LoadConfigAsync_With_Nonexistent_File_Should_Throw()
    {
        // Arrange
        var nonexistentPath = Path.Combine(_tempDirectory, "nonexistent.anom");

        // Act & Assert
        var act = async () => await _configurationService.LoadConfigAsync(nonexistentPath);
        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task SaveConfigAsync_Should_Create_Valid_File()
    {
        // Arrange
        var config = Step3TestHelpers.CreateTestConfig();
        var outputPath = Path.Combine(_tempDirectory, "saved_config.anom");

        // Act
        await _configurationService.SaveConfigAsync(config, outputPath);

        // Assert
        File.Exists(outputPath).Should().BeTrue();
        
        var content = await File.ReadAllTextAsync(outputPath);
        content.Should().Contain("[SETTINGS]");
        content.Should().Contain("[SCRIPT]");
        content.Should().Contain("\"Name\": \"Test Config\"");
        content.Should().Contain("\"Author\": \"Test Author\"");
    }

    [Fact]
    public async Task SaveConfigAsync_Should_Create_Directory_If_Not_Exists()
    {
        // Arrange
        var config = Step3TestHelpers.CreateTestConfig();
        var nestedPath = Path.Combine(_tempDirectory, "nested", "dir", "config.anom");

        // Act
        await _configurationService.SaveConfigAsync(config, nestedPath);

        // Assert
        File.Exists(nestedPath).Should().BeTrue();
        Directory.Exists(Path.GetDirectoryName(nestedPath)).Should().BeTrue();
    }

    [Fact]
    public async Task SaveAndLoad_RoundTrip_Should_Preserve_Data()
    {
        // Arrange
        var originalConfig = Step3TestHelpers.CreateComplexTestConfig();
        var filePath = Path.Combine(_tempDirectory, "roundtrip.anom");

        // Act
        await _configurationService.SaveConfigAsync(originalConfig, filePath);
        var loadedConfig = await _configurationService.LoadConfigAsync(filePath);

        // Assert
        loadedConfig.Name.Should().Be(originalConfig.Name);
        loadedConfig.Author.Should().Be(originalConfig.Author);
        loadedConfig.Version.Should().Be(originalConfig.Version);
        loadedConfig.Script.Should().Be(originalConfig.Script);
        loadedConfig.Settings.SuggestedBots.Should().Be(originalConfig.Settings.SuggestedBots);
        loadedConfig.Settings.NeedsProxies.Should().Be(originalConfig.Settings.NeedsProxies);
        loadedConfig.Settings.CustomUserAgent.Should().Be(originalConfig.Settings.CustomUserAgent);
        loadedConfig.Settings.DataRules.Should().BeEquivalentTo(originalConfig.Settings.DataRules);
    }

    [Fact]
    public async Task ValidateConfigAsync_With_Valid_File_Should_Return_Success()
    {
        // Arrange
        var validConfigPath = Path.Combine(_testDataDirectory, "valid_config.anom");

        // Act
        var result = await _configurationService.ValidateConfigAsync(validConfigPath);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.ParsedConfig.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateConfigAsync_With_Invalid_Json_Should_Return_Errors()
    {
        // Arrange
        var invalidConfigPath = Path.Combine(_testDataDirectory, "invalid_config.anom");

        // Act
        var result = await _configurationService.ValidateConfigAsync(invalidConfigPath);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Type == ConfigErrorType.ParseError || e.Type == ConfigErrorType.InvalidJson);
    }

    [Fact]
    public async Task ValidateConfigAsync_With_Nonexistent_File_Should_Return_File_Error()
    {
        // Arrange
        var nonexistentPath = Path.Combine(_tempDirectory, "nonexistent.anom");

        // Act
        var result = await _configurationService.ValidateConfigAsync(nonexistentPath);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors[0].Type.Should().Be(ConfigErrorType.FileNotFound);
    }

    [Fact]
    public async Task GetConfigsFromDirectoryAsync_Should_Return_All_Configs()
    {
        // Arrange - Copy test files to temp directory
        foreach (var testFile in Directory.GetFiles(_testDataDirectory, "*.anom"))
        {
            var destPath = Path.Combine(_tempDirectory, Path.GetFileName(testFile));
            File.Copy(testFile, destPath);
        }

        // Act
        var configs = await _configurationService.GetConfigsFromDirectoryAsync(_tempDirectory);

        // Assert
        configs.Should().NotBeNull();
        var configList = configs.ToList();
        configList.Should().HaveCountGreaterThan(0);
        
        var validConfig = configList.FirstOrDefault(c => c.Name == "Test Configuration");
        validConfig.Should().NotBeNull();
        validConfig!.IsValid.Should().BeTrue();
        validConfig.Author.Should().Be("TestAuthor");
    }

    [Fact]
    public async Task GetConfigsFromDirectoryAsync_With_Nonexistent_Directory_Should_Return_Empty()
    {
        // Arrange
        var nonexistentDir = Path.Combine(_tempDirectory, "nonexistent");

        // Act
        var configs = await _configurationService.GetConfigsFromDirectoryAsync(nonexistentDir);

        // Assert
        configs.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void IsValidConfigFile_With_Valid_File_Should_Return_True()
    {
        // Arrange
        var validConfigPath = Path.Combine(_testDataDirectory, "valid_config.anom");

        // Act
        var result = _configurationService.IsValidConfigFile(validConfigPath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidConfigFile_With_Invalid_Extension_Should_Return_False()
    {
        // Arrange
        var txtFile = Path.Combine(_tempDirectory, "test.txt");
        File.WriteAllText(txtFile, "Not a config file");

        // Act
        var result = _configurationService.IsValidConfigFile(txtFile);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidConfigFile_With_Missing_Sections_Should_Return_False()
    {
        // Arrange
        var invalidFile = Path.Combine(_tempDirectory, "invalid.anom");
        File.WriteAllText(invalidFile, "This file is missing required sections");

        // Act
        var result = _configurationService.IsValidConfigFile(invalidFile);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ParseConfigContent_Should_Parse_Complete_Content()
    {
        // Arrange
        var content = @"[SETTINGS]
{
  ""Name"": ""Parsed Config"",
  ""Author"": ""ParseTest"",
  ""SuggestedBots"": 25,
  ""NeedsProxies"": true
}

[SCRIPT]
PRINT Parsed script content
REQUEST GET ""https://example.com""";

        // Act
        var config = _configurationService.ParseConfigContent(content, "parsed.anom");

        // Assert
        config.Should().NotBeNull();
        config.Name.Should().Be("Parsed Config");
        config.Author.Should().Be("ParseTest");
        config.Settings.SuggestedBots.Should().Be(25);
        config.Settings.NeedsProxies.Should().BeTrue();
        config.Script.Should().Contain("PRINT Parsed script content");
        config.Script.Should().Contain("REQUEST GET");
    }

    [Fact]
    public void SerializeConfig_Should_Generate_Valid_Content()
    {
        // Arrange
        var config = Step3TestHelpers.CreateTestConfig();

        // Act
        var serialized = _configurationService.SerializeConfig(config);

        // Assert
        serialized.Should().NotBeEmpty();
        serialized.Should().Contain("[SETTINGS]");
        serialized.Should().Contain("[SCRIPT]");
        serialized.Should().Contain("\"Name\": \"Test Config\"");
        serialized.Should().Contain("\"Author\": \"Test Author\"");
        serialized.Should().Contain("PRINT Test script");
        
        // Should be parseable again
        var reparsed = _configurationService.ParseConfigContent(serialized);
        reparsed.Name.Should().Be(config.Name);
        reparsed.Author.Should().Be(config.Author);
    }

    [Fact]
    public void ParseConfigContent_With_Empty_Content_Should_Return_Default_Config()
    {
        // Arrange
        var emptyContent = "";

        // Act
        var config = _configurationService.ParseConfigContent(emptyContent);

        // Assert
        config.Should().NotBeNull();
        config.Name.Should().BeEmpty();
        config.Script.Should().BeEmpty();
        config.Settings.Should().NotBeNull();
    }

    [Fact]
    public void ParseConfigContent_With_Only_Settings_Should_Parse_Settings_Only()
    {
        // Arrange
        var content = @"[SETTINGS]
{
  ""Name"": ""Settings Only"",
  ""SuggestedBots"": 10
}";

        // Act
        var config = _configurationService.ParseConfigContent(content);

        // Assert
        config.Name.Should().Be("Settings Only");
        config.Settings.SuggestedBots.Should().Be(10);
        config.Script.Should().BeEmpty();
    }

    [Fact]
    public void ParseConfigContent_Should_Handle_Complex_Arrays()
    {
        // Arrange
        var content = @"[SETTINGS]
{
  ""Name"": ""Array Test"",
  ""DataRules"": [""rule1"", ""rule2"", ""rule3""],
  ""CustomInputs"": [""input1"", ""input2""]
}

[SCRIPT]
PRINT Array test";

        // Act
        var config = _configurationService.ParseConfigContent(content);

        // Assert
        config.Settings.DataRules.Should().HaveCount(3);
        config.Settings.DataRules.Should().Contain("rule1");
        config.Settings.DataRules.Should().Contain("rule2");
        config.Settings.DataRules.Should().Contain("rule3");
        config.Settings.CustomInputs.Should().HaveCount(2);
        config.Settings.CustomInputs.Should().Contain("input1");
        config.Settings.CustomInputs.Should().Contain("input2");
    }

    public void Dispose()
    {
        // Clean up temp directory
        if (Directory.Exists(_tempDirectory))
        {
            try
            {
                Directory.Delete(_tempDirectory, true);
            }
            catch
            {
                // Ignore cleanup errors in tests
            }
        }
    }
}

/// <summary>
/// Step 3 Performance Tests
/// </summary>
public class Step3_PerformanceTests : IDisposable
{
    private readonly Mock<ILogger<ConfigurationService>> _loggerMock;
    private readonly ConfigurationService _configurationService;
    private readonly string _tempDirectory;

    public Step3_PerformanceTests()
    {
        _loggerMock = new Mock<ILogger<ConfigurationService>>();
        _configurationService = new ConfigurationService(_loggerMock.Object);
        _tempDirectory = Path.Combine(Path.GetTempPath(), "OpenBulletPerfTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
    }

    [Fact]
    public void ParseConfigContent_Should_Be_Fast()
    {
        // Arrange
        var content = Step3TestHelpers.CreateLargeConfigContent();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var config = _configurationService.ParseConfigContent(content);
            config.Should().NotBeNull();
        }

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should parse 100 configs in under 1 second
    }

    [Fact]
    public void SerializeConfig_Should_Be_Fast()
    {
        // Arrange
        var config = Step3TestHelpers.CreateComplexTestConfig();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var serialized = _configurationService.SerializeConfig(config);
            serialized.Should().NotBeEmpty();
        }

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500); // Should serialize 100 configs in under 500ms
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            try { Directory.Delete(_tempDirectory, true); } catch { }
        }
    }
}

/// <summary>
/// Step 3 Test Helpers
/// </summary>
public static class Step3TestHelpers
{
    public static ConfigModel CreateTestConfig()
    {
        return new ConfigModel
        {
            Name = "Test Config",
            Author = "Test Author",
            Version = "1.0.0",
            AdditionalInfo = "Test configuration for unit testing",
            Script = "PRINT Test script\nREQUEST GET \"https://example.com\"",
            Settings = new ConfigSettings
            {
                SuggestedBots = 10,
                NeedsProxies = true,
                MaxRedirects = 5,
                CustomUserAgent = "TestAgent/1.0"
            }
        };
    }

    public static ConfigModel CreateComplexTestConfig()
    {
        return new ConfigModel
        {
            Name = "Complex Test Config",
            Author = "Complex Author",
            Version = "2.1.0",
            AdditionalInfo = "Complex configuration with all settings",
            Script = @"## Complex test script
#LOGIN REQUEST POST ""https://example.com/login""
  CONTENT ""user=<USER>&pass=<PASS>""
  CONTENTTYPE ""application/x-www-form-urlencoded""

KEYCHECK 
  KEYCHAIN Success OR 
    KEY ""welcome""
    KEY ""dashboard""
  KEYCHAIN Failure OR 
    KEY ""invalid""
    KEY ""error""",
            Settings = new ConfigSettings
            {
                SuggestedBots = 25,
                MaxCPM = 1000,
                NeedsProxies = true,
                OnlySocks = false,
                OnlySsl = true,
                MaxProxyUses = 5,
                MaxRedirects = 10,
                EncodeData = true,
                AllowedWordlist1 = "MailPass",
                AllowedWordlist2 = "UserPass",
                DataRules = { "rule1", "rule2", "rule3" },
                CustomInputs = { "input1", "input2" },
                CaptchaUrl = "https://example.com/captcha",
                Base64 = false,
                Grayscale = true,
                RemoveNoise = true,
                Threshold = 0.75,
                Contrast = 1.3,
                Gamma = 0.8,
                Brightness = 1.2,
                ForceHeadless = true,
                CustomUserAgent = "ComplexAgent/2.0",
                RandomUA = false,
                CustomCMDArgs = "--disable-extensions"
            }
        };
    }

    public static string CreateLargeConfigContent()
    {
        var sb = new StringBuilder();
        sb.AppendLine("[SETTINGS]");
        sb.AppendLine("{");
        sb.AppendLine("  \"Name\": \"Large Config\",");
        sb.AppendLine("  \"Author\": \"Performance Test\",");
        sb.AppendLine("  \"SuggestedBots\": 100,");
        
        // Add many data rules to test array parsing performance
        sb.AppendLine("  \"DataRules\": [");
        for (int i = 0; i < 50; i++)
        {
            sb.AppendLine($"    \"rule_{i}\"{(i < 49 ? "," : "")}");
        }
        sb.AppendLine("  ],");
        
        sb.AppendLine("  \"CustomInputs\": [");
        for (int i = 0; i < 30; i++)
        {
            sb.AppendLine($"    \"input_{i}\"{(i < 29 ? "," : "")}");
        }
        sb.AppendLine("  ]");
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("[SCRIPT]");
        
        // Add large script content
        for (int i = 0; i < 100; i++)
        {
            sb.AppendLine($"## Comment line {i}");
            sb.AppendLine($"PRINT Script line {i}");
        }
        
        return sb.ToString();
    }

    public static ConfigValidationResult CreateValidationResult(bool isValid = true)
    {
        var result = new ConfigValidationResult { IsValid = isValid };
        
        if (!isValid)
        {
            result.Errors.Add(new ConfigError
            {
                Type = ConfigErrorType.ParseError,
                Message = "Test error message",
                Section = "SETTINGS",
                LineNumber = 1
            });
        }
        
        return result;
    }

    public static ConfigInfo CreateConfigInfo(string name = "Test Config")
    {
        return new ConfigInfo
        {
            FilePath = $"/test/{name.ToLower().Replace(" ", "_")}.anom",
            Name = name,
            Author = "Test Author",
            Version = "1.0.0",
            LastModified = DateTime.UtcNow,
            FileSize = 1024,
            IsValid = true,
            Category = "Test",
            Tags = { "test", "example" }
        };
    }
}
