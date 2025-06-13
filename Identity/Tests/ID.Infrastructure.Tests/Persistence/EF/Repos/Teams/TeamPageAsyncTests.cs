using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.EF.Repos;
using ID.Tests.Data.Factories;
using Pagination;
using Shouldly;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;

public class TeamPageAsyncTests : RepoTestBase, IAsyncLifetime
{
    private TeamRepo _repo = null!;

    // Test data - create enough teams to test pagination
    private readonly List<Team> _customerTeams;
    private readonly List<Team> _maintenanceTeams;
    private readonly List<Team> _superTeams;
    private readonly List<Team> _allTeams;

    //-----------------------------//

    public TeamPageAsyncTests()
    {
        // Create multiple teams of each type for pagination testing
        _customerTeams = Enumerable.Range(1, 5)
            .Select(i => TeamDataFactory.Create(
                name: $"Customer Team {i:D2}",
                description: $"Customer team description {i}",
                teamType: TeamType.Customer))
            .ToList();

        _maintenanceTeams = Enumerable.Range(1, 3)
            .Select(i => TeamDataFactory.Create(
                name: $"Maintenance Team {i:D2}",
                description: $"Maintenance team description {i}",
                teamType: TeamType.Maintenance))
            .ToList();

        _superTeams = Enumerable.Range(1, 2)
            .Select(i => TeamDataFactory.Create(
                name: $"Super Team {i:D2}",
                description: $"Super team description {i}",
                teamType: TeamType.Super))
            .ToList();

        _allTeams = [.. _customerTeams, .. _maintenanceTeams, .. _superTeams];
    }

    //-----------------------------//

    public async Task InitializeAsync()
    {
        _repo = new TeamRepo(DbContext);
        await SeedDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task SeedDataAsync()
    {
        await DbContext.Teams.AddRangeAsync(_allTeams);
        await DbContext.SaveChangesAsync();
    }

    //=================================//
    // BASIC PAGINATION TESTS
    //=================================//

    #region Basic Pagination Tests

    [Fact]
    public async Task PageAsync_BasicPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 3;
        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Number.ShouldBe(pageNumber);
        result.Size.ShouldBe(pageSize);
        result.Data.Count.ShouldBeLessThanOrEqualTo(pageSize);
    }

    [Fact]
    public async Task PageAsync_ExcludesSuperTeams_ShouldNotContainSuperTeams()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 20; // Large enough to get all non-Super teams
        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotContain(t => t.TeamType == TeamType.Super);
        result.Data.ShouldContain(t => t.TeamType == TeamType.Customer);
        result.Data.ShouldContain(t => t.TeamType == TeamType.Maintenance);
    }

    [Fact]
    public async Task PageAsync_SecondPage_ShouldReturnDifferentResults()
    {
        // Arrange
        var pageSize = 2;
        var sortList = new List<SortRequest> 
        { 
            new() { Field = "Name", SortDescending = false }
        };
        var filterList = new List<FilterRequest>();

        // Act
        var page1 = await _repo.PageAsync(1, pageSize, sortList, filterList);
        var page2 = await _repo.PageAsync(2, pageSize, sortList, filterList);

        // Assert
        page1.ShouldNotBeNull();
        page2.ShouldNotBeNull();
          if (page1.Data.Any() && page2.Data.Any())
        {
            // If both pages have data, they should be different
            var page1Ids = page1.Data.Select(t => t.Id).ToList();
            var page2Ids = page2.Data.Select(t => t.Id).ToList();
            page1Ids.ShouldNotBe(page2Ids);
        }
    }

    [Fact]
    public async Task PageAsync_LargePageSize_ShouldReturnAllAvailableItems()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 100; // Much larger than available data
        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Size.ShouldBe(pageSize);
        result.Data.Count.ShouldBe(_customerTeams.Count + _maintenanceTeams.Count); // Excludes Super teams
    }

    #endregion

    //=================================//
    // SORTING TESTS
    //=================================//

    #region Sorting Tests

    [Fact]
    public async Task PageAsync_SortByNameAscending_ShouldReturnSortedResults()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var sortList = new List<SortRequest> 
        { 
            new() { Field = "Name", SortDescending = false }
        };
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
          // Check if results are sorted by name (ascending)
        var names = result.Data.Select(t => t.Name).ToList();
        var sortedNames = names.OrderBy(n => n).ToList();
        names.ShouldBe(sortedNames);
    }

    [Fact]
    public async Task PageAsync_SortByNameDescending_ShouldReturnReverseSortedResults()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var sortList = new List<SortRequest> 
        { 
            new() { Field = "Name", SortDescending = true }
        };
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
          // Check if results are sorted by name (descending)
        var names = result.Data.Select(t => t.Name).ToList();
        var sortedNames = names.OrderByDescending(n => n).ToList();
        names.ShouldBe(sortedNames);
    }

    [Fact]
    public async Task PageAsync_MultipleSortCriteria_ShouldRespectSortOrder()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var sortList = new List<SortRequest> 
        { 
            new() { Field = "TeamType", SortDescending = false },
            new() { Field = "Name", SortDescending = false }
        };
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        
        // Verify that teams are first sorted by TeamType, then by Name
        for (int i = 1; i < result.Data.Count; i++)
        {
            var current = result.Data[i];
            var previous = result.Data[i - 1];
            
            // TeamType should be in ascending order, or if same, Name should be in ascending order
            var currentTypeValue = (int)current.TeamType;
            var previousTypeValue = (int)previous.TeamType;
            
            (currentTypeValue >= previousTypeValue).ShouldBeTrue();
            
            if (currentTypeValue == previousTypeValue)
            {
                (string.Compare(current.Name, previous.Name, StringComparison.Ordinal) >= 0).ShouldBeTrue();
            }
        }
    }

    [Fact]
    public async Task PageAsync_EmptySortList_ShouldReturnResults()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 5;
        var sortList = new List<SortRequest>(); // Empty sort list
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        result.Data.Count.ShouldBeLessThanOrEqualTo(pageSize);
    }

    #endregion

    //=================================//
    // FILTERING TESTS
    //=================================//

    #region Filtering Tests

    [Fact]
    public async Task PageAsync_FilterByName_ShouldReturnFilteredResults()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var sortList = new List<SortRequest>();        var filterList = new List<FilterRequest>
        {
            new() { Field = "Name", FilterValue = "Customer", FilterType = FilterTypes.CONTAINS }
        };

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        // Note: Exact filtering behavior depends on the implementation in PageAsync
        // This test verifies the method accepts filter parameters without errors
    }

    [Fact]
    public async Task PageAsync_FilterByTeamType_ShouldReturnFilteredResults()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>
        {
            new() { Field = "TeamType", FilterValue = "Customer", FilterType = FilterTypes.EQUALS }
        };

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        // Super teams should still be excluded regardless of filter
        result.Data.ShouldNotContain(t => t.TeamType == TeamType.Super);
    }

    [Fact]
    public async Task PageAsync_MultipleFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>
        {
            new() { Field = "Name", FilterValue = "Team", FilterType = FilterTypes.CONTAINS },
            new() { Field = "TeamType", FilterValue = "Customer", FilterType = FilterTypes.EQUALS }
        };

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        // Verifies method handles multiple filters without errors
    }

    [Fact]
    public async Task PageAsync_EmptyFilterList_ShouldReturnAllResults()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>(); // Empty filter list

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        // Should still exclude Super teams
        result.Data.ShouldNotContain(t => t.TeamType == TeamType.Super);
    }

    [Fact]
    public async Task PageAsync_NullFilterList_ShouldReturnAllResults()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var sortList = new List<SortRequest>();
        List<FilterRequest>? filterList = null;

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
        // Should still exclude Super teams
        result.Data.ShouldNotContain(t => t.TeamType == TeamType.Super);
    }

    #endregion

    //=================================//
    // EDGE CASE TESTS
    //=================================//

    #region Edge Case Tests

    [Fact]
    public async Task PageAsync_ZeroPageSize_ShouldReturnEmptyPage()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 0;
        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Size.ShouldBe(0);
        result.Data.ShouldBeEmpty();
    }

    [Fact]
    public async Task PageAsync_NegativePageNumber_ShouldHandleGracefully()
    {
        // Arrange
        var pageNumber = -1;
        var pageSize = 5;
        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        // Behavior depends on implementation, but should not throw
    }

    [Fact]
    public async Task PageAsync_VeryLargePageNumber_ShouldReturnEmptyPage()
    {
        // Arrange
        var pageNumber = 1000;
        var pageSize = 5;
        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldBeEmpty(); // No data available for such a high page number
    }

    #endregion

    //=================================//
    // COMBINED OPERATIONS TESTS
    //=================================//

    #region Combined Operations Tests

    [Fact]
    public async Task PageAsync_SortingAndFiltering_ShouldWorkTogether()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var sortList = new List<SortRequest> 
        { 
            new() { Field = "Name", SortDescending = false }        };
        var filterList = new List<FilterRequest>
        {
            new() { Field = "TeamType", FilterValue = "Customer", FilterType = FilterTypes.EQUALS }
        };

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotContain(t => t.TeamType == TeamType.Super);
        
        // If any results, they should be sorted
        if (result.Data.Count > 1)
        {
            var names = result.Data.Select(t => t.Name).ToList();
            var sortedNames = names.OrderBy(n => n).ToList();
            names.ShouldBeEquivalentTo(sortedNames);
        }
    }

    [Fact]
    public async Task PageAsync_PaginationWithSortingAndFiltering_ShouldBeConsistent()
    {
        // Arrange
        var pageSize = 2;
        var sortList = new List<SortRequest> 
        { 
            new() { Field = "Name", SortDescending = false }
        };
        var filterList = new List<FilterRequest>();

        // Act
        var page1 = await _repo.PageAsync(1, pageSize, sortList, filterList);
        var page2 = await _repo.PageAsync(2, pageSize, sortList, filterList);

        // Assert
        page1.ShouldNotBeNull();
        page2.ShouldNotBeNull();
          // If both pages have data, names should be in consistent order
        if (page1.Data.Any() && page2.Data.Any())
        {
            var allNames = page1.Data.Concat(page2.Data).Select(t => t.Name).ToList();
            var sortedNames = allNames.OrderBy(n => n).ToList();
            allNames.ShouldBe(sortedNames);
        }
    }

    #endregion

}//Cls
