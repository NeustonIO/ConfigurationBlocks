using Neuston.ConfigurationBlocks;
using NUnit.Framework;
using UnityEngine;

public enum SomeEnum
{
    EnumValueA,
    EnumValueB
}

public class ExampleConfigBlock
{
    public int SomeInt = 1234;
    public float SomeFloat = 1.234f;
    public bool SomeBool = true;
    public string SomeString = "Default string value";
    public SomeEnum SomeEnum;
}

public class ConfigBlockProviderTests
{
    ConfigBlockProvider configBlockProvider;
    FakeRemoteConfig remoteConfig;

    [SetUp]
    public void Setup()
    {
        remoteConfig = new FakeRemoteConfig();
        configBlockProvider = new ConfigBlockProvider(remoteConfig);
    }

    [Test]
    public void ShouldReturnDefaultConstructedConfigBlockByDefault()
    {
        var config = configBlockProvider.Get<ExampleConfigBlock>();

        Assert.AreEqual(1234, config.SomeInt);
        Assert.AreEqual(1.234f, config.SomeFloat);
        Assert.AreEqual(true, config.SomeBool);
        Assert.AreEqual("Default string value", config.SomeString);
        Assert.AreEqual(SomeEnum.EnumValueA, config.SomeEnum);
    }

    [Test]
    public void ShouldConstructConfigBlockWithRemoteConfigJson()
    {
        var configToBePlacedInRemoteConfig = new ExampleConfigBlock()
        {
            SomeInt = 2048,
            SomeFloat = 777.7f,
            SomeBool = false,
            SomeString = "Sun",
            SomeEnum = SomeEnum.EnumValueB,
        };
        var json = JsonUtility.ToJson(configToBePlacedInRemoteConfig);
        remoteConfig.Add("ExampleConfigBlock", json);
        var configBlockProvider = new ConfigBlockProvider(remoteConfig);

        var config = configBlockProvider.Get<ExampleConfigBlock>();

        Assert.AreEqual(2048, config.SomeInt);
        Assert.AreEqual(777.7f, config.SomeFloat);
        Assert.AreEqual(false, config.SomeBool);
        Assert.AreEqual("Sun", config.SomeString);
        Assert.AreEqual(SomeEnum.EnumValueB, config.SomeEnum);
    }

    [Test]
    public void BrokenJsonYieldsDefaultValuesAndReportsError()
    {
        var remoteConfig = new FakeRemoteConfig();
        var json = "{ 'SomeString': this is borken json XD...";
        remoteConfig.Add("ExampleConfigBlock", json);
        var configBlockProvider = new ConfigBlockProvider(remoteConfig);
        bool receivedErrorCallback = false;
        configBlockProvider.FailedToParseJson += () => receivedErrorCallback = true;

        var config = configBlockProvider.Get<ExampleConfigBlock>();

        Assert.AreEqual("Default string value", config.SomeString);
        Assert.IsTrue(receivedErrorCallback);
    }

    [Test]
    public void ConfigBlockReferencesAreReusedForPerformance()
    {
        var configBlockProvider = new ConfigBlockProvider();

        var a = configBlockProvider.Get<ExampleConfigBlock>();
        var b = configBlockProvider.Get<ExampleConfigBlock>();

        Assert.AreEqual(a, b);
    }

    [Test]
    public void CachedConfigBlocksCanBeCleared()
    {
        var configBlockProvider = new ConfigBlockProvider();

        var a = configBlockProvider.Get<ExampleConfigBlock>();
        configBlockProvider.ClearCache();
        var b = configBlockProvider.Get<ExampleConfigBlock>();

        Assert.AreNotEqual(a, b);
    }
}