using ClArch.ValueObjects;
using ID.Domain.AppServices.Imps;
using ID.Domain.Entities.Teams;
using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace ID.Domain.Tests.AppServices.Imps;

public class TeamBuilderServiceTests
{
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions;
    private readonly Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>> _mockCustomerOptions;
    private readonly IdGlobalOptions _globalOptions;
    private readonly IdGlobalSetupOptions_CUSTOMER _customerOptions;
    private readonly TeamBuilderService _sut;

    public TeamBuilderServiceTests()
    {
        _mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        _mockCustomerOptions = new Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>>();

        _globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            defaultMinTeamPosition: 5,
            defaultMaxTeamPosition: 10,
            superTeamMinPosition: 1,
            superTeamMaxPosition: 1
        );
        
        _customerOptions = GlobalOptionsUtils.InitiallyValidCustomerOptions(
            maxTeamSize: 15,
            minTeamPosition: 2,
            maxTeamPosition: 8
        );

        _mockGlobalOptions.Setup(ap => ap.Value).Returns(_globalOptions);
        _mockCustomerOptions.Setup(ap => ap.Value).Returns(_customerOptions);

        _sut = new TeamBuilderService(_mockGlobalOptions.Object, _mockCustomerOptions.Object);
    }

    [Fact]
    public void CreateCustomerTeam_ShouldReturnTeamWithCustomerType()
    {
        // Arrange
        var name = Name.Create("Customer Team");
        var description = DescriptionNullable.Create("A team for customers");

        // Act
        var team = _sut.CreateCustomerTeam(name, description);

        // Assert
        team.ShouldNotBeNull();
        team.TeamType.ShouldBe(TeamType.customer);
    }


    [Fact]
    public void CreateCustomerTeam_ShouldUseCustomerOptionsForCapacityAndPositions()
    {
        // Arrange
        var name = Name.Create("Customer Team");
        var description = DescriptionNullable.Create("A team for customers");
        var expectedCapacity = _customerOptions.MaxTeamSize;
        var expectedMinPosition = _customerOptions.MinTeamPosition;
        var expectedMaxPosition = _customerOptions.MaxTeamPosition;

        // Act
        var team = _sut.CreateCustomerTeam(name, description);

        // Assert
        team.Capacity.ShouldBe(expectedCapacity);
        team.MinPosition.ShouldBe(expectedMinPosition);
        team.MaxPosition.ShouldBe(expectedMaxPosition);
        team.Name.ShouldBe(name.Value);
        team.Description.ShouldBe(description.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Valid Description")]
    public void CreateCustomerTeam_WithVaryingDescription_ShouldCreateTeamSuccessfully(string? descValue)
    {
        // Arrange
        var name = Name.Create("Test Team");
        var description = descValue == null ? DescriptionNullable.Create(null) : DescriptionNullable.Create(descValue);

        // Act
        var team = _sut.CreateCustomerTeam(name, description);

        // Assert
        team.ShouldNotBeNull();
        team.Name.ShouldBe(name.Value);
        team.Description.ShouldBe(description.Value);
        team.TeamType.ShouldBe(TeamType.customer);
    }



    [Fact]
    public void CreateMaintenanceTeam_ShouldReturnTeamWithMaintenanceType()
    {
        // Act
        var team = _sut.CreateMaintenanceTeam();

        // Assert
        team.ShouldNotBeNull();
        team.TeamType.ShouldBe(TeamType.maintenance);
    }

    [Fact]
    public void CreateMaintenanceTeam_ShouldUseGlobalOptionsForMinMaxPositions()
    {
        // Arrange
        var expectedMinPosition = _globalOptions.MntcTeamMinPosition;
        var expectedMaxPosition = _globalOptions.MntcTeamMaxPosition;

        // Act
        var team = _sut.CreateMaintenanceTeam();

        // Assert        team.MinPosition.ShouldBe(expectedMinPosition);
        team.MaxPosition.ShouldBe(expectedMaxPosition);
    }

    [Fact]
    public void CreateMaintenanceTeam_ShouldUseCorrectGlobalOptionsProperties()
    {
        // Arrange
        _globalOptions.MntcTeamMinPosition = 3;
        _globalOptions.MntcTeamMaxPosition = 7;
        // Ensure other global options are different to catch mis-mapping
        _globalOptions.SuperTeamMinPosition = 100;
        _globalOptions.SuperTeamMaxPosition = 101;
        _customerOptions.MaxTeamSize = 50;

        // Act
        var team = _sut.CreateMaintenanceTeam();

        // Assert
        team.MinPosition.ShouldBe(3);
        team.MaxPosition.ShouldBe(7);
    }


    [Fact]
    public void CreateSuperTeam_ShouldReturnTeamWithSuperType()
    {
        // Act
        var team = _sut.CreateSuperTeam();

        // Assert
        team.ShouldNotBeNull();
        team.TeamType.ShouldBe(TeamType.super);
    }

    [Fact]
    public void CreateSuperTeam_ShouldUseGlobalOptionsForMinMaxPositions()
    {
        // Arrange
        var expectedMinPosition = _globalOptions.SuperTeamMinPosition;
        var expectedMaxPosition = _globalOptions.SuperTeamMaxPosition;

        // Act
        var team = _sut.CreateSuperTeam();

        // Assert
        team.MinPosition.ShouldBe(expectedMinPosition);
        team.MaxPosition.ShouldBe(expectedMaxPosition);
    }
    [Fact]
    public void CreateSuperTeam_ShouldUseCorrectGlobalOptionsProperties()
    {
        // Arrange
        _globalOptions.SuperTeamMinPosition = 2;
        _globalOptions.SuperTeamMaxPosition = 2;
        // Ensure other global options are different
        _globalOptions.MntcTeamMinPosition = 200;
        _globalOptions.MntcTeamMaxPosition = 201;
        _customerOptions.MaxTeamSize = 60;

        // Act
        var team = _sut.CreateSuperTeam();

        // Assert
        team.MinPosition.ShouldBe(2);
        team.MaxPosition.ShouldBe(2);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(10, 20)]
    [InlineData(7, 7)] // Test case where min and max are the same
    public void CreateMaintenanceTeam_WithVariousGlobalOptionValues_ShouldMapCorrectly(int min, int max)
    {
        // Arrange
        _globalOptions.MntcTeamMinPosition = min;
        _globalOptions.MntcTeamMaxPosition = max;

        // Act
        var team = _sut.CreateMaintenanceTeam();

        // Assert
        team.MinPosition.ShouldBe(min);
        team.MaxPosition.ShouldBe(max);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 5)] // Super teams often have fixed min/max
    public void CreateSuperTeam_WithVariousGlobalOptionValues_ShouldMapCorrectly(int min, int max)
    {
        // Arrange
        _globalOptions.SuperTeamMinPosition = min;
        _globalOptions.SuperTeamMaxPosition = max;

        // Act
        var team = _sut.CreateSuperTeam();

        // Assert
        team.MinPosition.ShouldBe(min);
        team.MaxPosition.ShouldBe(max);
    }
    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(1)] // Test with a small team size
    public void CreateCustomerTeam_WithVariousCustomerOptionValues_ShouldMapCorrectly(int maxTeamSize)
    {
        // Arrange
        _customerOptions.MaxTeamSize = maxTeamSize;
        // Set different values for min/max positions to ensure they're mapped correctly
        _customerOptions.MinTeamPosition = 2;
        _customerOptions.MaxTeamPosition = 8;

        var name = Name.Create("Customer Team");
        var description = DescriptionNullable.Create("A team for customers");

        // Act
        var team = _sut.CreateCustomerTeam(name, description);

        // Assert
        team.Capacity.ShouldBe(maxTeamSize);
        team.MinPosition.ShouldBe(2);
        team.MaxPosition.ShouldBe(8);
    }


    // Test to ensure IdGlobalSetupOptions_CUSTOMER is NOT used for Maintenance Team
    [Fact]
    public void CreateMaintenanceTeam_ShouldNotUseCustomerOptions()
    {
        // Arrange
        // Set distinct values to ensure customer options are not accidentally used
        _customerOptions.MaxTeamSize = 999;
        _globalOptions.MntcTeamMinPosition = 3;
        _globalOptions.MntcTeamMaxPosition = 8;

        // Act
        var team = _sut.CreateMaintenanceTeam();

        // Assert
        team.MinPosition.ShouldBe(3); // Verifies it's not 999
        team.MaxPosition.ShouldBe(8); // Verifies it's not 999
    }


    // Test to ensure IdGlobalSetupOptions_CUSTOMER is NOT used for Super Team
    [Fact]
    public void CreateSuperTeam_ShouldNotUseCustomerOptions()
    {
        // Arrange
        _customerOptions.MaxTeamSize = 888; // Distinct value
        _globalOptions.SuperTeamMinPosition = 1;
        _globalOptions.SuperTeamMaxPosition = 2;

        // Act
        var team = _sut.CreateSuperTeam();

        // Assert
        team.MinPosition.ShouldBe(1); // Verifies it's not 888
        team.MaxPosition.ShouldBe(2); // Verifies it's not 888
    }
    // Test to ensure IdGlobalSetupOptions (global) is NOT used for Customer Team's MaxTeamSize/Capacity/Positions
    [Fact]
    public void CreateCustomerTeam_ShouldNotUseGlobalOptionsForSizing()
    {
        // Arrange
        _globalOptions.MntcTeamMaxPosition = 777; // A value different from customerOptions values
        _globalOptions.MntcTeamMinPosition = 666;
        _globalOptions.SuperTeamMaxPosition = 555;
        _customerOptions.MaxTeamSize = 12; // This should be used for capacity
        _customerOptions.MinTeamPosition = 3; // This should be used for min position
        _customerOptions.MaxTeamPosition = 9; // This should be used for max position

        var name = Name.Create("Customer Team");
        var description = DescriptionNullable.Create("A team for customers");

        // Act
        var team = _sut.CreateCustomerTeam(name, description);

        // Assert
        team.Capacity.ShouldBe(12);
        team.Capacity.ShouldNotBe(777);
        team.Capacity.ShouldNotBe(555);

        team.MinPosition.ShouldBe(3);
        team.MinPosition.ShouldNotBe(666);

        team.MaxPosition.ShouldBe(9);
        team.MaxPosition.ShouldNotBe(777);
    }
}