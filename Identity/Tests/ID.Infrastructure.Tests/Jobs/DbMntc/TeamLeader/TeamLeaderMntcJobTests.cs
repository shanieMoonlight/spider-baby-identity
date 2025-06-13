using Id.Tests.Utility.Exceptions;
using ID.Application.Utility;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Validators;
using ID.Infrastructure.Jobs.Imps.DbMntc.Jobs;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using Microsoft.Extensions.Logging;
using Moq;


namespace ID.Infrastructure.Tests.Jobs.DbMntc.TeamLeader;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
public class TeamLeaderMntcJobTests : ServiceProviderTestBase
{
    private readonly Mock<IIdUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IIdentityTeamRepo> _mockTeamRepo;
    private readonly Mock<ILogger<TeamLeaderMntcJob>> _mockLogger;
    private readonly TeamLeaderMntcJob _job;

    //- - - - - - - - - - - - - - - - - - //

    public TeamLeaderMntcJobTests()
    {
        _mockUnitOfWork = new Mock<IIdUnitOfWork>();
        _mockTeamRepo = new Mock<IIdentityTeamRepo>();
        _mockLogger = new Mock<ILogger<TeamLeaderMntcJob>>();

        _mockUnitOfWork.Setup(uow => uow.TeamRepo).Returns(_mockTeamRepo.Object);
        MockServiceProvider.Setup(sp => sp.GetService(typeof(IIdUnitOfWork))).Returns(_mockUnitOfWork.Object);
        _job = new TeamLeaderMntcJob(MockServiceProvider.Object, _mockLogger.Object);
    }

    //------------------------------------//


    [Fact]
    public async Task HandleAsync_Should_Set_Leader_For_Teams_With_Missing_Leaders()
    {
        // Arrange
        var team1Id = Guid.NewGuid();
        var team1 = TeamDataFactory.Create(
            id: team1Id,
            members:
            [
                AppUserDataFactory.Create(teamId: team1Id, teamPosition: 1),
                AppUserDataFactory.Create(teamId: team1Id, teamPosition: 2)
            ]);
        var team2Id = Guid.NewGuid();
        var team2 = TeamDataFactory.Create(
            id: team2Id,
            members:
            [
                AppUserDataFactory.Create(teamId: team2Id, teamPosition: 1),
                AppUserDataFactory.Create(teamId: team2Id, teamPosition: 2)
            ]);

        var teams = new List<Team> { team1, team2 };

        _mockTeamRepo.Setup(repo => repo.ListAllAsync(It.IsAny<TeamsWithMissingLeadersSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);


        // Act
        await _job.HandleAsync(CancellationToken.None);

        // Assert
        foreach (var team in teams)
        {
            team.Leader.ShouldNotBeNull();
        }

        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//


    [Fact]
    public async Task HandleAsync_LogsException_WhenExceptionThrown()
    {

        _mockUnitOfWork.Setup(uow => uow.TeamRepo.ListAllAsync(It.IsAny<TeamsWithMissingLeadersSpec>(), It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new Exception("Test exception"));

        var cancellationToken = CancellationToken.None;

        // Act
        await _job.HandleAsync(cancellationToken);

        // Assert
        _mockLogger.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test exception")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleAsync_Should_LogGenResultFailure_When_ValidationFails()
    {
        // Arrange
        var team1Id = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();

        // Create a member that belongs to a different team (will cause validation failure)
        var memberFromOtherTeam = AppUserDataFactory.Create(teamId: otherTeamId, teamPosition: 10);

        var team1 = TeamDataFactory.Create(
            id: team1Id,
            members: [memberFromOtherTeam]); // Member belongs to different team

        var teams = new List<Team> { team1 };

        _mockTeamRepo.Setup(repo => repo.ListAllAsync(It.IsAny<TeamsWithMissingLeadersSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        // Act
        await _job.HandleAsync(CancellationToken.None);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_mockLogger, MyIdLoggingEvents.JOBS.DB_MNTC);


        // Verify team leader was NOT set
        team1.Leader.ShouldBeNull();
    }

    //------------------------------------//

}//Cls
