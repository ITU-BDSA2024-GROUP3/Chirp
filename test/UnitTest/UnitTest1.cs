using ChirpInfrastructure;
using ChirpWeb;

namespace UnitTest;

public class UnitTest1
{
    [Fact]
    public void convertunixtimestamp()
    {
        int timestamp = 1728284672;
        String time = CheepRepository.UnixTimeStampToDateTimeString(timestamp);
        Assert.Equal(time, "10/07/24 7.04.32");
        
    }
}