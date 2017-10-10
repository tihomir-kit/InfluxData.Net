using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v0.9.6 Integration")]
    [Trait("InfluxDb v0.9.6 Integration", "Serie")]
    public class IntegrationSerie_v_0_9_6 : IntegrationSerie
    {
        public IntegrationSerie_v_0_9_6(IntegrationFixture_v_0_9_6 fixture) : base(fixture)
        {
        }

        [Fact]
#pragma warning disable xUnit1024 // Test methods cannot have overloads
        public virtual async Task GetSeries_OnExistingSeries_ShouldReturnSerieSetCollection()
#pragma warning restore xUnit1024 // Test methods cannot have overloads
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);

            var points = await _fixture.MockAndWritePoints(3, 2, dbName);

            var result = await _fixture.Sut.Serie.GetSeriesAsync(dbName);
            result.Should().HaveCount(2);
            var firstSet = result.FirstOrDefault(p => p.Name == points.First().Name);
            firstSet.Should().NotBeNull();
            firstSet.Series.Should().HaveCount(3);
            firstSet.Series.First().Key.Should().NotBeNullOrEmpty();
            firstSet.Series.First().Tags.Should().HaveCount(points.First().Tags.Count);
            var lastSet = result.FirstOrDefault(p => p.Name == points.Last().Name);
            lastSet.Should().NotBeNull();
            lastSet.Series.Should().HaveCount(3);
            lastSet.Series.First().Key.Should().NotBeNullOrEmpty();
            lastSet.Series.First().Tags.Should().HaveCount(points.First().Tags.Count);
        }
    }
}
