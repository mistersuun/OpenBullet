using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Models;
using OpenBullet.Core.Services;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 3 Validation Tests - Simple tests to verify implementation without file dependencies
/// </summary>
public class Step3_ValidationTests
{
    [Fact]
    public void ConfigurationService_Can_Be_Created_Successfully()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ConfigurationService>>();

        // Act
        var service = new ConfigurationService(loggerMock.Object);

        // Assert
        service.Should().NotBeNull();
        service.Should().BeAssignableTo<IConfigurationService>();
    }

    [Fact]
    public void ParseConfigContent_With_Valid_Json_Should_Work()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ConfigurationService>>();
        var service = new ConfigurationService(loggerMock.Object);
        
        var content = @"[SETTINGS]
{
  ""Name"": ""Test Config"",
  ""Author"": ""Test Author"",
  ""SuggestedBots"": 5,
  ""NeedsProxies"": true
}

[SCRIPT]
PRINT Hello World";

        // Act
        var config = service.ParseConfigContent(content, "test.anom");

        // Assert
        config.Should().NotBeNull();
        config.Name.Should().Be("Test Config");
        config.Author.Should().Be("Test Author");
        config.Settings.SuggestedBots.Should().Be(5);
        config.Settings.NeedsProxies.Should().BeTrue();
        config.Script.Should().Be("PRINT Hello World");
    }

    [Fact]
    public void ParseConfigContent_With_Missing_Settings_Should_Use_Defaults()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ConfigurationService>>();
        var service = new ConfigurationService(loggerMock.Object);
        
        var content = @"[SETTINGS]
{
  ""Name"": ""Minimal Config""
}

[SCRIPT]
PRINT Minimal script";

        // Act
        var config = service.ParseConfigContent(content);

        // Assert
        config.Should().NotBeNull();
        config.Name.Should().Be("Minimal Config");
        config.Settings.SuggestedBots.Should().Be(1); // Default value
        config.Settings.MaxRedirects.Should().Be(8); // Default value
        config.Settings.NeedsProxies.Should().BeFalse(); // Default value
    }

    [Fact]
    public void SerializeConfig_Should_Generate_Parseable_Content()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ConfigurationService>>();
        var service = new ConfigurationService(loggerMock.Object);
        
        var originalConfig = new ConfigModel
        {
            Name = "Serialize Test",
            Author = "Test User",
            Version = "1.5.0",
            Script = "PRINT Serialization test\nREQUEST GET \"https://test.com\"",
            Settings = new ConfigSettings
            {
                SuggestedBots = 15,
                NeedsProxies = true,
                MaxRedirects = 10,
                CustomUserAgent = "SerializeTest/1.0",
                DataRules = { "rule1", "rule2" },
                CustomInputs = { "input1" }
            }
        };

        // Act
        var serialized = service.SerializeConfig(originalConfig);
        var deserializedConfig = service.ParseConfigContent(serialized);

        // Assert
        serialized.Should().NotBeEmpty();
        serialized.Should().Contain("[SETTINGS]");
        serialized.Should().Contain("[SCRIPT]");
        
        deserializedConfig.Name.Should().Be(originalConfig.Name);
        deserializedConfig.Author.Should().Be(originalConfig.Author);
        deserializedConfig.Version.Should().Be(originalConfig.Version);
        deserializedConfig.Script.Should().Be(originalConfig.Script);
        deserializedConfig.Settings.SuggestedBots.Should().Be(originalConfig.Settings.SuggestedBots);
        deserializedConfig.Settings.NeedsProxies.Should().Be(originalConfig.Settings.NeedsProxies);
        deserializedConfig.Settings.MaxRedirects.Should().Be(originalConfig.Settings.MaxRedirects);
        deserializedConfig.Settings.CustomUserAgent.Should().Be(originalConfig.Settings.CustomUserAgent);
        deserializedConfig.Settings.DataRules.Should().BeEquivalentTo(originalConfig.Settings.DataRules);
        deserializedConfig.Settings.CustomInputs.Should().BeEquivalentTo(originalConfig.Settings.CustomInputs);
    }

    [Fact]
    public void ParseConfigContent_Should_Handle_All_Boolean_Settings()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ConfigurationService>>();
        var service = new ConfigurationService(loggerMock.Object);
        
        var content = @"[SETTINGS]
{
  ""Name"": ""Boolean Test"",
  ""IgnoreResponseErrors"": true,
  ""NeedsProxies"": true,
  ""OnlySocks"": true,
  ""OnlySsl"": true,
  ""EncodeData"": true,
  ""Base64"": true,
  ""Grayscale"": true,
  ""RemoveLines"": true,
  ""RemoveNoise"": true,
  ""Dilate"": true,
  ""Transparent"": true,
  ""OnlyShow"": true,
  ""ContrastGamma"": true,
  ""ForceHeadless"": true,
  ""AlwaysOpen"": true,
  ""AlwaysQuit"": true,
  ""DisableNotifications"": true,
  ""RandomUA"": true,
  ""LoliSave"": true
}

[SCRIPT]
PRINT Boolean test";

        // Act
        var config = service.ParseConfigContent(content);

        // Assert
        config.Settings.IgnoreResponseErrors.Should().BeTrue();
        config.Settings.NeedsProxies.Should().BeTrue();
        config.Settings.OnlySocks.Should().BeTrue();
        config.Settings.OnlySsl.Should().BeTrue();
        config.Settings.EncodeData.Should().BeTrue();
        config.Settings.Base64.Should().BeTrue();
        config.Settings.Grayscale.Should().BeTrue();
        config.Settings.RemoveLines.Should().BeTrue();
        config.Settings.RemoveNoise.Should().BeTrue();
        config.Settings.Dilate.Should().BeTrue();
        config.Settings.Transparent.Should().BeTrue();
        config.Settings.OnlyShow.Should().BeTrue();
        config.Settings.ContrastGamma.Should().BeTrue();
        config.Settings.ForceHeadless.Should().BeTrue();
        config.Settings.AlwaysOpen.Should().BeTrue();
        config.Settings.AlwaysQuit.Should().BeTrue();
        config.Settings.DisableNotifications.Should().BeTrue();
        config.Settings.RandomUA.Should().BeTrue();
        config.Settings.LoliSave.Should().BeTrue();
    }

    [Fact]
    public void ParseConfigContent_Should_Handle_All_Numeric_Settings()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ConfigurationService>>();
        var service = new ConfigurationService(loggerMock.Object);
        
        var content = @"[SETTINGS]
{
  ""Name"": ""Numeric Test"",
  ""SuggestedBots"": 25,
  ""MaxCPM"": 500,
  ""MaxRedirects"": 12,
  ""MaxProxyUses"": 20,
  ""Threshold"": 0.85,
  ""DiffKeep"": 0.15,
  ""DiffHide"": 0.25,
  ""Contrast"": 1.5,
  ""Gamma"": 0.7,
  ""Brightness"": 1.3,
  ""RemoveLinesMin"": 3,
  ""RemoveLinesMax"": 25
}

[SCRIPT]
PRINT Numeric test";

        // Act
        var config = service.ParseConfigContent(content);

        // Assert
        config.Settings.SuggestedBots.Should().Be(25);
        config.Settings.MaxCPM.Should().Be(500);
        config.Settings.MaxRedirects.Should().Be(12);
        config.Settings.MaxProxyUses.Should().Be(20);
        config.Settings.Threshold.Should().Be(0.85);
        config.Settings.DiffKeep.Should().Be(0.15);
        config.Settings.DiffHide.Should().Be(0.25);
        config.Settings.Contrast.Should().Be(1.5);
        config.Settings.Gamma.Should().Be(0.7);
        config.Settings.Brightness.Should().Be(1.3);
        config.Settings.RemoveLinesMin.Should().Be(3);
        config.Settings.RemoveLinesMax.Should().Be(25);
    }

    [Fact]
    public void ParseConfigContent_Should_Handle_All_String_Settings()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ConfigurationService>>();
        var service = new ConfigurationService(loggerMock.Object);
        
        var content = @"[SETTINGS]
{
  ""Name"": ""String Test"",
  ""Author"": ""String Author"",
  ""Version"": ""2.3.4"",
  ""AdditionalInfo"": ""Additional information here"",
  ""AllowedWordlist1"": ""MailPass"",
  ""AllowedWordlist2"": ""UserPass"",
  ""CaptchaUrl"": ""https://example.com/captcha.jpg"",
  ""CustomUserAgent"": ""StringTest/2.3.4"",
  ""CustomCMDArgs"": ""--disable-gpu --headless""
}

[SCRIPT]
PRINT String test";

        // Act
        var config = service.ParseConfigContent(content);

        // Assert
        config.Name.Should().Be("String Test");
        config.Author.Should().Be("String Author");
        config.Version.Should().Be("2.3.4");
        config.AdditionalInfo.Should().Be("Additional information here");
        config.Settings.AllowedWordlist1.Should().Be("MailPass");
        config.Settings.AllowedWordlist2.Should().Be("UserPass");
        config.Settings.CaptchaUrl.Should().Be("https://example.com/captcha.jpg");
        config.Settings.CustomUserAgent.Should().Be("StringTest/2.3.4");
        config.Settings.CustomCMDArgs.Should().Be("--disable-gpu --headless");
    }

    [Fact]
    public void ParseConfigContent_Should_Handle_Complex_Arrays()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ConfigurationService>>();
        var service = new ConfigurationService(loggerMock.Object);
        
        var content = @"[SETTINGS]
{
  ""Name"": ""Array Test"",
  ""DataRules"": [
    ""rule with spaces"",
    ""rule_with_underscores"",
    ""rule-with-dashes"",
    ""rule.with.dots""
  ],
  ""CustomInputs"": [
    ""Username"",
    ""Password"",
    ""Email"",
    ""Phone Number""
  ]
}

[SCRIPT]
PRINT Array test";

        // Act
        var config = service.ParseConfigContent(content);

        // Assert
        config.Settings.DataRules.Should().HaveCount(4);
        config.Settings.DataRules.Should().Contain("rule with spaces");
        config.Settings.DataRules.Should().Contain("rule_with_underscores");
        config.Settings.DataRules.Should().Contain("rule-with-dashes");
        config.Settings.DataRules.Should().Contain("rule.with.dots");
        
        config.Settings.CustomInputs.Should().HaveCount(4);
        config.Settings.CustomInputs.Should().Contain("Username");
        config.Settings.CustomInputs.Should().Contain("Password");
        config.Settings.CustomInputs.Should().Contain("Email");
        config.Settings.CustomInputs.Should().Contain("Phone Number");
    }

    [Fact]
    public void IsValidConfigFile_Should_Handle_Various_Cases()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ConfigurationService>>();
        var service = new ConfigurationService(loggerMock.Object);

        // Act & Assert - These should not throw
        service.IsValidConfigFile("").Should().BeFalse();
        service.IsValidConfigFile("nonexistent.anom").Should().BeFalse();
        service.IsValidConfigFile("test.txt").Should().BeFalse();
    }

    [Theory]
    [InlineData(ConfigErrorType.ParseError)]
    [InlineData(ConfigErrorType.InvalidJson)]
    [InlineData(ConfigErrorType.MissingSection)]
    [InlineData(ConfigErrorType.InvalidScript)]
    [InlineData(ConfigErrorType.FileNotFound)]
    [InlineData(ConfigErrorType.AccessDenied)]
    public void ConfigError_Types_Should_Be_Available(ConfigErrorType errorType)
    {
        // Arrange & Act
        var error = new ConfigError
        {
            Type = errorType,
            Message = $"Test error of type {errorType}",
            Section = "TEST",
            LineNumber = 1
        };

        // Assert
        error.Type.Should().Be(errorType);
        error.Message.Should().NotBeEmpty();
        error.Section.Should().Be("TEST");
        error.LineNumber.Should().Be(1);
    }

    [Theory]
    [InlineData(ConfigWarningType.DeprecatedSetting)]
    [InlineData(ConfigWarningType.UnknownSetting)]
    [InlineData(ConfigWarningType.SuggestedImprovement)]
    [InlineData(ConfigWarningType.PerformanceWarning)]
    public void ConfigWarning_Types_Should_Be_Available(ConfigWarningType warningType)
    {
        // Arrange & Act
        var warning = new ConfigWarning
        {
            Type = warningType,
            Message = $"Test warning of type {warningType}",
            Section = "TEST",
            LineNumber = 5
        };

        // Assert
        warning.Type.Should().Be(warningType);
        warning.Message.Should().NotBeEmpty();
        warning.Section.Should().Be("TEST");
        warning.LineNumber.Should().Be(5);
    }

    [Fact]
    public void ConfigValidationResult_Should_Initialize_Correctly()
    {
        // Arrange & Act
        var result = new ConfigValidationResult();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNull().And.BeEmpty();
        result.Warnings.Should().NotBeNull().And.BeEmpty();
        result.ParsedConfig.Should().BeNull();
    }

    [Fact]
    public void ConfigInfo_Should_Initialize_Correctly()
    {
        // Arrange & Act
        var info = new ConfigInfo();

        // Assert
        info.FilePath.Should().BeEmpty();
        info.Name.Should().BeEmpty();
        info.Author.Should().BeEmpty();
        info.Version.Should().BeEmpty();
        info.LastModified.Should().Be(default);
        info.FileSize.Should().Be(0);
        info.IsValid.Should().BeFalse();
        info.Category.Should().BeNull();
        info.Tags.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ConfigParsingOptions_Should_Initialize_With_Defaults()
    {
        // Arrange & Act
        var options = new ConfigParsingOptions();

        // Assert
        options.ValidateScript.Should().BeTrue();
        options.ValidateSettings.Should().BeTrue();
        options.LoadMetadata.Should().BeTrue();
        options.StrictMode.Should().BeFalse();
        options.AllowedSections.Should().Contain(ConfigSections.Settings);
        options.AllowedSections.Should().Contain(ConfigSections.Script);
    }

    [Fact]
    public void Step3TestHelpers_Should_Create_Valid_Objects()
    {
        // Act
        var testConfig = Step3TestHelpers.CreateTestConfig();
        var complexConfig = Step3TestHelpers.CreateComplexTestConfig();
        var largeContent = Step3TestHelpers.CreateLargeConfigContent();
        var validationResult = Step3TestHelpers.CreateValidationResult(true);
        var configInfo = Step3TestHelpers.CreateConfigInfo();

        // Assert
        testConfig.Should().NotBeNull();
        testConfig.Name.Should().Be("Test Config");
        testConfig.Author.Should().Be("Test Author");
        
        complexConfig.Should().NotBeNull();
        complexConfig.Name.Should().Be("Complex Test Config");
        complexConfig.Settings.DataRules.Should().HaveCount(3);
        
        largeContent.Should().NotBeEmpty();
        largeContent.Should().Contain("[SETTINGS]");
        largeContent.Should().Contain("[SCRIPT]");
        
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        
        configInfo.Should().NotBeNull();
        configInfo.Name.Should().Be("Test Config");
        configInfo.IsValid.Should().BeTrue();
    }
}
