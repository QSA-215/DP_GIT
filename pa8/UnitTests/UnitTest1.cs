using DBController;
using MessageBroker;
using Moq;
using Newtonsoft.Json;
using RankCalculator;
using Newtonsoft.Json;

namespace UnitTests.RankCalculator;

public struct RankCalculatorTestData
{
    public string Text;
    public double ExpectedRank;
}

public class RankCalculatorTests
{
    private const string FILE_PATH_WITH_DATA = "../../../test_data.json";

    [Theory]
    [MemberData(nameof(GetTestData))]
    public async Task CalculateRank_ShouldReturnExpectedResult(string text, double expectedRank)
    {
        string id = "id";
        string shardKey = "key";

        Mock<IDBController> dbMock = new Mock<IDBController>();
        Mock<IMessageBroker> brokerMock = new Mock<IMessageBroker>();

        dbMock.Setup(d => d.Get(id, "MAIN")).Returns(shardKey);
        dbMock.Setup(d => d.Get($"TEXT-{id}", shardKey)).Returns(text);

        RankCalculatorService rankCalculatorService = new(dbMock.Object, brokerMock.Object);


        await rankCalculatorService.Proccess(id);


        brokerMock.Verify(b => b.SendMessageAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()), Times.Once);

        dbMock.Verify(m => m.Set(shardKey, $"RANK-{id}", expectedRank.ToString()), Times.Once);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        IEnumerable<RankCalculatorTestData> data;
        using (StreamReader reader = new(FILE_PATH_WITH_DATA))
        {
            data = JsonConvert.DeserializeObject<IEnumerable<RankCalculatorTestData>>(reader.ReadToEnd());
        }

        foreach (var item in data)
        {
            yield return new object[] { item.Text, item.ExpectedRank };
        }
    }
}