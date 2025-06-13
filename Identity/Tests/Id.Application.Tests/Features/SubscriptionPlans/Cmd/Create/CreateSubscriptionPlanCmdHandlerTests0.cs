//using CollectionHelpers;
//using ID.Application.Features.FeatureFlags;
//using ID.Application.Features.SubscriptionPlans;
//using ID.Application.Features.SubscriptionPlans.Cmd.Create;
//using ID.Domain.Abstractions.Repos;
//using ID.Domain.Entities.SubscriptionPlans;
//using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
//using ID.Domain.Utility;
//using ID.Tests.Data.Factories;
//using ID.Tests.Data.Factories.Dtos;
//using Moq;
//using MyResults;
//using Shouldly;

//namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.Create;

//public class CreateSubscriptionPlanCmdHandlerTests
//{
//    private readonly Mock<IIdentitySubscriptionPlanRepo> _mockSubscriptionPlanRepo;
//    private readonly Mock<IIdentityFeatureFlagRepo> _mockFeaturesRepo;
//    private readonly Mock<IIdUnitOfWork> _mockUnitOfWork;

//    //- - - - - - - - - - - - - - - - - - //

//    public CreateSubscriptionPlanCmdHandlerTests()
//    {
//        _mockSubscriptionPlanRepo = new Mock<IIdentitySubscriptionPlanRepo>();
//        _mockFeaturesRepo = new Mock<IIdentityFeatureFlagRepo>();
//        _mockUnitOfWork = new Mock<IIdUnitOfWork>();
//        _mockUnitOfWork.Setup(uow => uow.SubscriptionPlanRepo).Returns(_mockSubscriptionPlanRepo.Object);
//        _mockUnitOfWork.Setup(uow => uow.FeatureFlagRepo).Returns(_mockFeaturesRepo.Object);
//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Handle_ShouldCreateSubscriptionPlan_WhenDtoHasNoFeatures()
//    {
//        // Arrange
//        var requestDto = SubscriptionPlanDtoDataFactory.Create(
//            null,
//           "Test SubscriptionPlanName_NEWNEWNEW",
//           "Test SubscriptionPlanDescription_NEWNEWNEW");

//        var newModel = requestDto.ToModel();

//        var cmd = new CreateSubscriptionPlanCmd(requestDto);
//        _mockSubscriptionPlanRepo.Setup(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(newModel);
//        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
//            .Returns(Task.CompletedTask);

//        var handler = new CreateSubscriptionPlanCmdHandler(_mockUnitOfWork.Object);

//        // Act
//        var result = await handler.Handle(cmd, CancellationToken.None);

//        // Assert
//        result.ShouldBeOfType<GenResult<SubscriptionPlanDto>>();
//        result.Value.ShouldNotBeNull();
//        result.Value?.Name.ShouldBe(cmd.Dto.Name);
//        result.Value?.Description.ShouldBe(cmd.Dto.Description);
//        _mockSubscriptionPlanRepo.Verify(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()), Times.Once);
//        _mockFeaturesRepo.Verify(repo => repo.GetRangeByIdsAsync(It.IsAny<IEnumerable<Guid>>()), Times.Never);
//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Handle_ShouldCreateSubscriptionPlanAndLinkFeatures_WhenDtoHasFeatureIds()
//    {
//        // Arrange
//        var featureId1 = Guid.NewGuid();
//        var featureId2 = Guid.NewGuid();
//        var feature1 = FeatureFlagDataFactory.Create(featureId1, "Feature_1_Name", "Feature_1_Description");
//        var feature2 = FeatureFlagDataFactory.Create(featureId2, "Feature_2_Name", "Feature_2_Description");
//        var feature1Dto = feature1.ToDto();
//        var feature2Dto = feature2.ToDto();

//        var existingFeatures = new List<FeatureFlag> { feature1, feature2 };
//        var existingFeatureDtos = new List<FeatureFlagDto> { feature1Dto, feature2Dto };
//        var existingFeatureDtoIds = existingFeatureDtos.Select(ff => ff.Id);

//        var dto = SubscriptionPlanDtoDataFactory.Create(
//            null,
//            "SubscriptionPlanName",
//            "SubscriptionPlanDescription",
//            null,
//            null,
//            null,
//            null,
//            existingFeatureDtoIds);

//        var createdModel = dto.ToModel();

//        var cmd = new CreateSubscriptionPlanCmd(dto);
//        _mockSubscriptionPlanRepo.Setup(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(createdModel);
//        _mockFeaturesRepo.Setup(repo => repo.GetRangeByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
//            .ReturnsAsync(existingFeatures);
//        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
//            .Returns(Task.CompletedTask);

//        var handler = new CreateSubscriptionPlanCmdHandler(_mockUnitOfWork.Object);


//        // Act
//        var result = await handler.Handle(cmd, CancellationToken.None);

//        // Assert
//        result.ShouldBeOfType<GenResult<SubscriptionPlanDto>>();
//        result.Value.ShouldNotBeNull();
//        foreach (var featureId in dto.FeatureFlagIds)
//        {
//            result.Value?.FeatureFlagIds.ShouldContain(featureId);
//        }
//        _mockSubscriptionPlanRepo.Verify(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()), Times.Once);
//        _mockFeaturesRepo.Verify(repo => repo.GetRangeByIdsAsync(cmd.Dto.FeatureFlagIds), Times.Once);
//        _mockSubscriptionPlanRepo.Verify(repo => repo.UpdateAsync(It.IsAny<SubscriptionPlan>()), Times.Once);
//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Handle_ShouldCreateSubscriptionPlanAndLinkFeatures_WhenDtoHasFeatures()
//    {
//        // Arrange
//        var featureId1 = Guid.NewGuid();
//        var featureId2 = Guid.NewGuid();
//        var feature1 = FeatureFlagDataFactory.Create(featureId1, "Feature_1_Name", "Feature_1_Description");
//        var feature2 = FeatureFlagDataFactory.Create(featureId2, "Feature_2_Name", "Feature_2_Description");
//        var feature1Dto = feature1.ToDto();
//        var feature2Dto = feature2.ToDto();

//        var existingFeatures = new List<FeatureFlag> { feature1, feature2 };
//        var existingFeatureDtos = new List<FeatureFlagDto> { feature1Dto, feature2Dto };
//        var existingFeatureDtoIds = existingFeatureDtos.Select(ff => ff.Id);

//        var dto = SubscriptionPlanDtoDataFactory.Create(
//            null,
//            "SubscriptionPlanName",
//            "SubscriptionPlanDescription",
//            null,
//            null,
//            null,
//            existingFeatureDtos);


//        var cmd = new CreateSubscriptionPlanCmd(dto);
//        var cmdFeatureFlagIds = cmd.Dto.FeatureFlags.Select(f => f.Id).ToList();
//        var createdModel = dto.ToModel();

//        _mockSubscriptionPlanRepo.Setup(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(createdModel);
//        _mockFeaturesRepo.Setup(repo => repo.GetRangeByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
//            .ReturnsAsync(existingFeatures);
//        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
//            .Returns(Task.CompletedTask);

//        var handler = new CreateSubscriptionPlanCmdHandler(_mockUnitOfWork.Object);


//        // Act
//        var result = await handler.Handle(cmd, CancellationToken.None);

//        // Assert
//        result.ShouldBeOfType<GenResult<SubscriptionPlanDto>>();
//        result.Value.ShouldNotBeNull();
//        result.Value?.FeatureFlagIds.Count().ShouldBe(dto.FeatureFlags.Count());

//        foreach (var feature in dto.FeatureFlags)
//        {
//            result.Value?.FeatureFlagIds.ShouldContain(feature.Id);
//        }

//        _mockSubscriptionPlanRepo.Verify(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()), Times.Once);
//        _mockFeaturesRepo.Verify(repo => repo.GetRangeByIdsAsync(cmdFeatureFlagIds), Times.Once);
//        _mockSubscriptionPlanRepo.Verify(repo => repo.UpdateAsync(It.IsAny<SubscriptionPlan>()), Times.Once);
//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Handle_ShouldReturnNotFound_WhenFeatureIdsAreNotFound()
//    {
//        // Arrange
//        var featureId1 = Guid.NewGuid();
//        var featureId2 = Guid.NewGuid();

//        var existingFeature = FeatureFlagDataFactory.Create(featureId1, "Feature_1_Name", "Feature_1_Description");
//        var existingFeature1Dto = existingFeature.ToDto();
//        var existingFeatures = new List<FeatureFlag> { existingFeature };
//        var existingFeatureDtos = new List<FeatureFlagDto> { existingFeature1Dto };
//        var existingFeatureDtoIds = existingFeatureDtos.Select(ff => ff.Id);

//        var missingFeature = FeatureFlagDataFactory.Create(featureId2, "Feature_2_Name", "Feature_2_Description");
//        var missingFeatureDto = missingFeature.ToDto();
//        var missingFeatures = new List<FeatureFlag> { missingFeature };
//        var missingFeatureDtos = new List<FeatureFlagDto> { missingFeatureDto };
//        var missingFeatureDtoIds = missingFeatureDtos.Select(ff => ff.Id);


//        List<FeatureFlagDto> allFeatureDtos = [.. missingFeatureDtos, .. existingFeatureDtos];
//        var allFeaturesDtoIds = allFeatureDtos.Select(ff => ff.Id);

//        var dto = SubscriptionPlanDtoDataFactory.Create(
//            null,
//            "SubscriptionPlanName",
//            "SubscriptionPlanDescription",
//            null,
//            null,
//            null,
//            null,
//            allFeaturesDtoIds);

//        var createdModel = dto.ToModel();
//        var cmd = new CreateSubscriptionPlanCmd(dto);
//        _mockSubscriptionPlanRepo.Setup(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(createdModel);
//        _mockFeaturesRepo.Setup(repo => repo.GetRangeByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
//            .ReturnsAsync([existingFeature]);

//        var handler = new CreateSubscriptionPlanCmdHandler(_mockUnitOfWork.Object);

//        // Act
//        var result = await handler.Handle(cmd, CancellationToken.None);

//        // Assert
//        result.ShouldBeOfType<GenResult<SubscriptionPlanDto>>();
//        result.Succeeded.ShouldBeFalse();
//        result.Info.ShouldStartWith(IDMsgs.Error.NotFound<FeatureFlag>(missingFeatureDtoIds.JoinStr(", ")));
//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Handle_ShouldReturnNotFound_WhenFeaturesAreNotFound()
//    {
//        // Arrange
//        var featureId1 = Guid.NewGuid();
//        var featureId2 = Guid.NewGuid();

//        var existingFeature = FeatureFlagDataFactory.Create(featureId1, "Feature_1_Name", "Feature_1_Description");
//        var existingFeature1Dto = existingFeature.ToDto();
//        var existingFeatures = new List<FeatureFlag> { existingFeature };
//        var existingFeatureDtos = new List<FeatureFlagDto> { existingFeature1Dto };
//        var existingFeatureDtoIds = existingFeatureDtos.Select(ff => ff.Id);

//        var missingFeature = FeatureFlagDataFactory.Create(featureId2, "Feature_2_Name", "Feature_2_Description");
//        var missingFeatureDto = missingFeature.ToDto();
//        var missingFeatures = new List<FeatureFlag> { missingFeature };
//        var missingFeatureDtos = new List<FeatureFlagDto> { missingFeatureDto };
//        var missingFeatureDtoIds = missingFeatureDtos.Select(ff => ff.Id);


//        List<FeatureFlagDto> allFeatureDtos = [.. missingFeatureDtos, .. existingFeatureDtos];
//        var allFeaturesDtoIds = allFeatureDtos.Select(ff => ff.Id);

//        var dto = SubscriptionPlanDtoDataFactory.Create(
//            null,
//            "SubscriptionPlanName",
//            "SubscriptionPlanDescription",
//            null,
//            null,
//            null,
//            allFeatureDtos);

//        var createdModel = dto.ToModel();
//        var cmd = new CreateSubscriptionPlanCmd(dto);
//        _mockSubscriptionPlanRepo.Setup(repo => repo.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(createdModel);
//        _mockFeaturesRepo.Setup(repo => repo.GetRangeByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
//            .ReturnsAsync([existingFeature]);

//        var handler = new CreateSubscriptionPlanCmdHandler(_mockUnitOfWork.Object);

//        // Act
//        var result = await handler.Handle(cmd, CancellationToken.None);

//        // Assert
//        result.ShouldBeOfType<GenResult<SubscriptionPlanDto>>();
//        result.Succeeded.ShouldBeFalse();
//        result.Info.ShouldStartWith(IDMsgs.Error.NotFound<FeatureFlag>(missingFeatureDtoIds.JoinStr(", ")));
//    }

//    //------------------------------------//

//}//Cls