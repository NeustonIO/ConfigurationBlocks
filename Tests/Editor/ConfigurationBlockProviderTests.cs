using Neuston.ConfigurationBlocks;
using NUnit.Framework;
using UnityEngine;

public class ConfigurationBlockProviderTests
{
	ConfigurationBlockProvider configurationBlockProvider;
	FakeRemoteConfig remoteConfig;
	ExampleConfigBlock returnedConfigBlock;
	bool receivedErrorCallback;

	[SetUp]
	public void Setup()
	{
		remoteConfig = new FakeRemoteConfig();
		configurationBlockProvider = new ConfigurationBlockProvider(remoteConfig);
		configurationBlockProvider.FailedToParseJson += () => receivedErrorCallback = true;
		receivedErrorCallback = false;
	}

	[Test]
	public void ShouldReturnDefaultConstructedConfigBlockByDefault()
	{
		WhenGetIsCalled();

		ThenDefaultConstructedConfigBlockIsReturned();
	}

	[Test]
	public void JsonInRemoteConfigShouldBeUsedToConstructConfigBlock()
	{
		GivenRemoteConfigHasJsonForConfigBlock();

		WhenGetIsCalled();

		ThenConfigBlockWithValuesFromJsonIsReturned();
	}

	[Test]
	public void BrokenJsonYieldsDefaultValuesAndReportsError()
	{
		GivenBrokenJsonInRemoteConfig();

		WhenGetIsCalled();

		ThenDefaultConstructedConfigBlockIsReturned();
		AndReceivedErrorCallbackWasCalled();
	}

	[Test]
	public void ConfigBlockReferencesAreReusedForPerformance()
	{
		var a = configurationBlockProvider.Get<ExampleConfigBlock>();
		var b = configurationBlockProvider.Get<ExampleConfigBlock>();

		Assert.AreEqual(a, b);
	}

	[Test]
	public void CachedConfigBlocksCanBeCleared()
	{
		var a = configurationBlockProvider.Get<ExampleConfigBlock>();
		configurationBlockProvider.ClearCache();
		var b = configurationBlockProvider.Get<ExampleConfigBlock>();

		Assert.AreNotEqual(a, b);
	}

	void WhenGetIsCalled()
	{
		returnedConfigBlock = configurationBlockProvider.Get<ExampleConfigBlock>();
	}

	void ThenDefaultConstructedConfigBlockIsReturned()
	{
		var c = returnedConfigBlock;

		Assert.AreEqual(1234, c.SomeInt);
		Assert.AreEqual(1.234f, c.SomeFloat);
		Assert.AreEqual(true, c.SomeBool);
		Assert.AreEqual("Default string value", c.SomeString);
		Assert.AreEqual(SomeEnum.EnumValueA, c.SomeEnum);
	}

	void ThenConfigBlockWithValuesFromJsonIsReturned()
	{
		var c = returnedConfigBlock;

		Assert.AreEqual(2048, c.SomeInt);
		Assert.AreEqual(777.7f, c.SomeFloat);
		Assert.AreEqual(false, c.SomeBool);
		Assert.AreEqual("Sun", c.SomeString);
		Assert.AreEqual(SomeEnum.EnumValueB, c.SomeEnum);
	}

	void GivenRemoteConfigHasJsonForConfigBlock()
	{
		var json = JsonUtility.ToJson(new ExampleConfigBlock()
		{
			SomeInt = 2048,
			SomeFloat = 777.7f,
			SomeBool = false,
			SomeString = "Sun",
			SomeEnum = SomeEnum.EnumValueB,
		});

		remoteConfig.Add("ExampleConfigBlock", json);
	}

	void AndReceivedErrorCallbackWasCalled()
	{
		Assert.IsTrue(receivedErrorCallback);
	}

	void GivenBrokenJsonInRemoteConfig()
	{
		var json = "{ 'SomeString': this is borken json XD...";
		remoteConfig.Add("ExampleConfigBlock", json);
	}
}

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