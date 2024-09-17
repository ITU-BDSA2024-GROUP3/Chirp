namespace Chirp.CLI.Client.Tests;

public class ClientCLITest
{
    [Fact]
    public void Test1()
    {
    }

    // 
    [Theory]
    [InlineData(1726177000, "12/9/2024")]
    [InlineData(1726099201, "12/9/2024")]
    [InlineData(1717200001, "1/6/2024")]
    public void TestConvertTime(long timestamp, string expected)
    {
        //timestamp = 1726177000
        var actual = UserInterface.ConvertTime(timestamp);
        Assert.Equal(expected, actual);
    }
}