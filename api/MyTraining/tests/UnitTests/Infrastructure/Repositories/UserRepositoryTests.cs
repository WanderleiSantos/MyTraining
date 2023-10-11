using Application.Shared.Extensions;
using Bogus;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FakeItEasy;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using SharedTests.Extensions;

namespace UnitTests.Infrastructure.Repositories;

public class UserRepositoryTests
{
    private readonly DefaultDbContext _dbContextMock;
    private readonly DbSet<User> _dbSetMock;
    private readonly IUserRepository _repositoryMock;
    private readonly Faker _faker;

    public UserRepositoryTests()
    {
        _dbSetMock = A.Fake<DbSet<User>>();
        
        _dbContextMock = A.Fake<DefaultDbContext>(x =>
        {
            x.WithArgumentsForConstructor(new object[] {  new DbContextOptionsBuilder<DefaultDbContext>().Options });
        });

        _repositoryMock = new UserRepository(_dbContextMock);
        
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldInsertUserSuccessfully()
    {
        // Given
        var user = CreateFakeUser();
        var cancellationToken = CancellationToken.None;
        
        // var internalEntityEntry = new InternalEntityEntry(
        //     new Mock<IStateManager>().Object,
        //     new RuntimeEntityType("User", typeof(User), false, null, null, null, ChangeTrackingStrategy.Snapshot, null, false),
        //     user);
        //
        // var entityEntry = new Mock<EntityEntry<User>>(internalEntityEntry);
        
        A.CallTo(() => _dbSetMock.AddAsync(A<User>._, A<CancellationToken>._))
            // .Returns(entityEntry.Object);    
        .Returns(new EntityEntry<User>
            (new InternalEntityEntry(
                A.Fake<IStateManager>(),
                new RuntimeEntityType("User", typeof(User), false, null, null, null, ChangeTrackingStrategy.Snapshot, null, false),
                user)
            ));
        A.CallTo(() => _dbContextMock.Set<User>()).Returns(_dbSetMock);
        
        // Act
        await _repositoryMock.AddAsync(user, cancellationToken);

        // Assert
        A.CallTo(() => _dbContextMock.Set<User>()).MustHaveHappenedOnceExactly();
        // A.CallTo(() => _dbSetMock.AddAsync(A<User>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
    
    private User CreateFakeUser() => new User(
        _faker.Random.String2(10),
        _faker.Random.String2(10),
        _faker.Internet.Email(),
        _faker.Internet.PasswordCustom(9, 32).HashPassword()
    );
}