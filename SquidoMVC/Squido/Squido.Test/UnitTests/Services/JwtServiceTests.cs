using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Services;

public class JwtServiceTests
{
    private readonly Mock<IUnitOfWork> mockUow;
    private readonly Mock<IConfiguration> mockConfig;
    private readonly JwtService jwtService;

    public JwtServiceTests()
    {
        mockUow = new Mock<IUnitOfWork>();
        mockConfig = new Mock<IConfiguration>();

        mockConfig.Setup(c => c["Jwt:Key"]).Returns("supersecretkey1234567890longenough!!");
        mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        mockConfig.Setup(c => c["Jwt:ExpiryInMiunte"]).Returns("30");

        jwtService = new JwtService(mockUow.Object, mockConfig.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        // Arrange
        string userId = "user-1";
        string username = "john";
        int roleId = 1;

        // Act
        string token = jwtService.GenerateToken(userId, username, roleId);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Equal("john", jwtToken.Payload[JwtRegisteredClaimNames.Name]);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldInsertAndReturnToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockRepo = new Mock<IGenericRepository<RefreshToken>>();
        mockUow.Setup(u => u.RefreshTokenRepository).Returns(mockRepo.Object);

        // Act
        var result = jwtService.GenerateRefreshToken(userId);

        // Assert
        mockRepo.Verify(r => r.Insert(It.IsAny<RefreshToken>()), Times.Once);
        mockUow.Verify(u => u.Save(), Times.Once);
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result?.Token));
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_ShouldReturnTrueIfValid()
    {
        // Arrange
        string token = "test-token";
        string userId = Guid.NewGuid().ToString();
        var validToken = new RefreshToken { Token = token, UserId = Guid.Parse(userId), Expires = DateTime.UtcNow.AddDays(1) };

        var mockRepo = new Mock<IGenericRepository<RefreshToken>>();
        mockRepo.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync(validToken);

        mockUow.Setup(u => u.RefreshTokenRepository).Returns(mockRepo.Object);

        // Act
        var result = await jwtService.ValidateRefreshTokenAsync(token, userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetValidRefreshTokenAsync_ShouldReturnTokenIfValid()
    {
        // Arrange
        var token = "valid-token";
        var userId = Guid.NewGuid();
        var storedToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            Expires = DateTime.UtcNow.AddDays(1)
        };

        var mockRepo = new Mock<IGenericRepository<RefreshToken>>();
        mockRepo.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync(storedToken);

        mockUow.Setup(u => u.RefreshTokenRepository).Returns(mockRepo.Object);

        // Act
        var result = await jwtService.GetValidRefreshTokenAsync(token, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(token, result?.Token);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_ShouldDeleteTokenIfExists()
    {
        // Arrange
        var token = "token-to-delete";
        var storedToken = new RefreshToken { Token = token };

        var mockRepo = new Mock<IGenericRepository<RefreshToken>>();
        mockRepo.Setup(r => r.GetSingleWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync(storedToken);

        mockUow.Setup(u => u.RefreshTokenRepository).Returns(mockRepo.Object);

        // Act
        await jwtService.RevokeRefreshTokenAsync(token);

        // Assert
        mockRepo.Verify(r => r.DeleteAsync(storedToken), Times.Once);
        mockUow.Verify(u => u.Save(), Times.Once);
    }

    [Fact]
    public void ValidateAccessToken_ShouldReturnClaimsPrincipal()
    {
        // Arrange
        var token = jwtService.GenerateToken("1", "testuser", 1);

        // Act
        var principal = jwtService.ValidateAccessToken(token);

        // Assert
        Assert.NotNull(principal);
        Assert.Equal("testuser", principal.FindFirst(JwtRegisteredClaimNames.Name)?.Value);
    }
}
