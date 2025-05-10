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
using Microsoft.IdentityModel.Tokens;

namespace Squido.Test.UnitTests.Services;

public class JwtServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockConfiguration = new Mock<IConfiguration>();

        // Setup configuration values
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("your-256-bit-secret-key-here-minimum-32-characters");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
        _mockConfiguration.Setup(c => c["Jwt:ExpiryInMiunte"]).Returns("60");

        _jwtService = new JwtService(_mockUnitOfWork.Object, _mockConfiguration.Object);
    }

    [Fact]
    public void GenerateToken_ValidInput_ReturnsValidToken()
    {
        // Arrange
        var userId = "test-user-id";
        var username = "test-user";
        var roleId = 1;

        // Act
        var token = _jwtService.GenerateToken(userId, username, roleId);

        // Assert
        Assert.NotNull(token);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal(userId, jwtToken.Claims.First(c => c.Type == "sub").Value);
        Assert.Equal(username, jwtToken.Claims.First(c => c.Type == "name").Value);
        Assert.Equal(roleId.ToString(), jwtToken.Claims.First(c => c.Type == "Role").Value);
        Assert.Equal("test-issuer", jwtToken.Issuer);
        Assert.Equal("test-audience", jwtToken.Audiences.First());
    }

    [Fact]
    public void GenerateRefreshToken_ValidUserId_ReturnsValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.Insert(It.IsAny<RefreshToken>()));
        _mockUnitOfWork.Setup(u => u.Save());

        // Act
        var refreshToken = _jwtService.GenerateRefreshToken(userId);

        // Assert
        Assert.NotNull(refreshToken);
        Assert.Equal(userId, refreshToken.UserId);
        Assert.False(string.IsNullOrEmpty(refreshToken.Token));
        Assert.True(refreshToken.Created <= DateTime.Now);
        Assert.True(refreshToken.Expires > DateTime.Now);
        _mockUnitOfWork.Verify(u => u.RefreshTokenRepository.Insert(It.IsAny<RefreshToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
    }

    [Fact]
    public async Task ValidateRefreshToken_ValidToken_ReturnsTrue()
    {
        // Arrange
        var token = "valid-token";
        var userId = "test-user-id";
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = Guid.Parse(userId),
            Created = DateTime.Now,
            Expires = DateTime.Now.AddDays(7)
        };

        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync(refreshToken);

        // Act
        var result = await _jwtService.ValidateRefreshTokenAsync(token, userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateRefreshToken_ExpiredToken_ReturnsFalse()
    {
        // Arrange
        var token = "expired-token";
        var userId = "test-user-id";
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = Guid.Parse(userId),
            Created = DateTime.Now.AddDays(-8),
            Expires = DateTime.Now.AddDays(-1)
        };

        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync(refreshToken);

        // Act
        var result = await _jwtService.ValidateRefreshTokenAsync(token, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateRefreshToken_InvalidToken_ReturnsFalse()
    {
        // Arrange
        var token = "invalid-token";
        var userId = "test-user-id";

        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync((RefreshToken)null);

        // Act
        var result = await _jwtService.ValidateRefreshTokenAsync(token, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetValidRefreshToken_ValidToken_ReturnsToken()
    {
        // Arrange
        var token = "valid-token";
        var userId = Guid.NewGuid();
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            Created = DateTime.Now,
            Expires = DateTime.Now.AddDays(7)
        };

        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync(refreshToken);

        // Act
        var result = await _jwtService.GetValidRefreshTokenAsync(token, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(token, result.Token);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetValidRefreshToken_ExpiredToken_ReturnsNull()
    {
        // Arrange
        var token = "expired-token";
        var userId = Guid.NewGuid();
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            Created = DateTime.Now.AddDays(-8),
            Expires = DateTime.Now.AddDays(-1)
        };

        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync(refreshToken);

        // Act
        var result = await _jwtService.GetValidRefreshTokenAsync(token, userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RevokeRefreshToken_ValidToken_DeletesToken()
    {
        // Arrange
        var token = "valid-token";
        var refreshToken = new RefreshToken { Token = token };

        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync(refreshToken);

        // Act
        await _jwtService.RevokeRefreshTokenAsync(token);

        // Assert
        _mockUnitOfWork.Verify(u => u.RefreshTokenRepository.DeleteAsync(refreshToken), Times.Once);
        _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
    }

    [Fact]
    public async Task RevokeRefreshToken_InvalidToken_DoesNothing()
    {
        // Arrange
        var token = "invalid-token";

        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync((RefreshToken)null);

        // Act
        await _jwtService.RevokeRefreshTokenAsync(token);

        // Assert
        _mockUnitOfWork.Verify(u => u.RefreshTokenRepository.DeleteAsync(It.IsAny<RefreshToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
    }

    [Fact]
    public void ValidateAccessToken_ValidToken_ReturnsPrincipal()
    {
        // Arrange
        var userId = "test-user-id";
        var username = "test-user";
        var roleId = 1;
        var token = _jwtService.GenerateToken(userId, username, roleId);

        // Act
        var principal = _jwtService.ValidateAccessToken(token);

        // Assert
        Assert.NotNull(principal);
        Assert.Equal(userId, principal.FindFirst("sub")?.Value);
        Assert.Equal(username, principal.FindFirst("name")?.Value);
        Assert.Equal(roleId.ToString(), principal.FindFirst("Role")?.Value);
    }

    [Fact]
    public void ValidateAccessToken_InvalidToken_ThrowsException()
    {
        // Arrange
        var invalidToken = "invalid-token";

        // Act & Assert
        Assert.Throws<SecurityTokenMalformedException>(() => _jwtService.ValidateAccessToken(invalidToken));
    }
}
