using AutoMapper;
using Xunit;

public class MappingTests
{
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _configuration;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = _configuration.CreateMapper();
    }

    [Fact]
    public void MappingConfiguration_IsValid()
    {
        // Verifica se a configuração do mapeamento é válida
        _configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_Map_Source_To_Destination()
    {
        // Arrange
        var source = new Source { Id = 1, Name = "Test" };

        // Act
        var destination = _mapper.Map<Destination>(source);

        // Assert
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal(source.Name, destination.Name);
    }
}

// Classes de exemplo
public class Source : IMapFrom<Destination>
{
    public int Id { get; set; }
    public string Name { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Source, Destination>();
    }
}

public class Destination
{
    public int Id { get; set; }
    public string Name { get; set; }
}
