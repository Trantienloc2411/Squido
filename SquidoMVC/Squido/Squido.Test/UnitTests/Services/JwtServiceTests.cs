using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    private readonly string _secretKey = "your-512-bit-secret-key-here-minimum-64-characters-long-to-support-hmac-sha512-algorithm";
    private readonly string _issuer = "test-issuer";
    private readonly string _audience = "test-audience";

    public JwtServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockConfiguration = new Mock<IConfiguration>();

        // Setup configuration values
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns(_secretKey);
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns(_issuer);
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns(_audience);
        _mockConfiguration.Setup(c => c["Jwt:ExpiryInMiunte"]).Returns("60");

        // Setup RefreshTokenRepository
        var mockRefreshTokenRepo = new Mock<IGenericRepository<RefreshToken>>();
        _mockUnitOfWork.SetupGet(u => u.RefreshTokenRepository).Returns(mockRefreshTokenRepo.Object);

        _jwtService = new JwtService(_mockUnitOfWork.Object, _mockConfiguration.Object);
    }

    // Existing tests (unchanged)
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
    public void GenerateToken_NullInput_ThrowsArgumentNullException()
    {
        // Arrange
        string userId = null;
        string username = "test-user";
        int roleId = 1;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _jwtService.GenerateToken(userId, username, roleId));
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
    public void GenerateRefreshToken_EmptyGuid_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => _jwtService.GenerateRefreshToken(userId));
    }

    [Fact]
    public async Task ValidateRefreshToken_ValidToken_ReturnsTrue()
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
        var result = await _jwtService.ValidateRefreshTokenAsync(token, userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateRefreshToken_ExpiredToken_ReturnsFalse()
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
        var result = await _jwtService.ValidateRefreshTokenAsync(token, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateRefreshToken_InvalidToken_ReturnsFalse()
    {
        // Arrange
        var token = "invalid-token";
        var userId = Guid.NewGuid();

        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync((RefreshToken)null);

        // Act
        var result = await _jwtService.ValidateRefreshTokenAsync(token, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateRefreshToken_EmptyToken_ReturnsFalse()
    {
        // Arrange
        var token = "";
        var userId = Guid.NewGuid();

        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync((RefreshToken)null);

        // Act
        var result = await _jwtService.ValidateRefreshTokenAsync(token, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateRefreshToken_MismatchedUserId_ReturnsFalse()
    {
        // Arrange
        var token = "valid-token";
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = differentUserId,
            Created = DateTime.Now,
            Expires = DateTime.Now.AddDays(7)
        };

        // Mock the repository to return null when UserId doesn't match
        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.Is<Expression<Func<RefreshToken, bool>>>(expr => !expr.Compile()(refreshToken))))
            .ReturnsAsync((RefreshToken)null);

        // Mock the repository to return the token when UserId matches
        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.Is<Expression<Func<RefreshToken, bool>>>(expr => expr.Compile()(refreshToken))))
            .ReturnsAsync(refreshToken);

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
    public async Task GetValidRefreshToken_NullToken_ReturnsNull()
    {
        // Arrange
        string token = null;
        var userId = Guid.NewGuid();

        // Mock RefreshTokenRepository
        var mockRefreshTokenRepo = new Mock<IGenericRepository<RefreshToken>>();
        mockRefreshTokenRepo.Setup(r => r.GetSingleWithIncludeAsync(
                It.IsAny<Expression<Func<RefreshToken, bool>>>(),
                It.IsAny<Expression<Func<RefreshToken, object>>[]>()))!
            .ReturnsAsync((RefreshToken)null)
            .Verifiable();

        // Setup IUnitOfWork to return the mocked RefreshTokenRepository
        _mockUnitOfWork.SetupGet(u => u.RefreshTokenRepository)
            .Returns(mockRefreshTokenRepo.Object);

        // Act
        var result = await _jwtService.GetValidRefreshTokenAsync(token, userId);

        // Assert
        Assert.Null(result);
        mockRefreshTokenRepo.Verify(r => r.GetSingleWithIncludeAsync(
            It.IsAny<Expression<Func<RefreshToken, bool>>>(),
            It.IsAny<Expression<Func<RefreshToken, object>>[]>()), Times.Never());
    }

    [Fact]
    public async Task GetValidRefreshToken_WrongUserId_ReturnsNull()
    {
        // Arrange
        var token = "valid-token";
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = differentUserId, // Different user ID
            Created = DateTime.Now,
            Expires = DateTime.Now.AddDays(7)
        };

        // Mock the repository to return null when the UserId doesn't match
        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.Is<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>(
                    expr => expr.Compile()(refreshToken) == false)))
            .ReturnsAsync((RefreshToken)null);

        // Mock the repository to return the token when the UserId matches (for completeness)
        _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetSingleWithIncludeAsync(
                It.Is<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>(
                    expr => expr.Compile()(refreshToken) == true)))
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
                It.IsAny<System.Linq.Expressions.Expression<Func<RefreshToken, bool>>>()))!
            .ReturnsAsync((RefreshToken)null);

        // Act
        await _jwtService.RevokeRefreshTokenAsync(token);

        // Assert
        _mockUnitOfWork.Verify(u => u.RefreshTokenRepository.DeleteAsync(It.IsAny<RefreshToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
    }

    [Fact]
    public async Task RevokeRefreshToken_EmptyToken_DoesNothing()
    {
        // Arrange
        string token = "";

        // Act
        await _jwtService.RevokeRefreshTokenAsync(token);

        // Assert
        _mockUnitOfWork.Verify(u => u.RefreshTokenRepository.DeleteAsync(It.IsAny<RefreshToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
    }

    // New and updated tests for ValidateAccessToken
    [Fact]
    public void ValidateAccessToken_ValidToken_ReturnsPrincipal()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var username = "testuser";
        var roleId = 1;
        var token = _jwtService.GenerateToken(userId, username, roleId);

        // Act
        var principal = _jwtService.ValidateAccessToken(token);

        // Assert
        Assert.NotNull(principal);
        Assert.Equal(userId, principal.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Check NameIdentifier instead of sub
        Assert.Equal(username, principal.FindFirst(JwtRegisteredClaimNames.Name)?.Value);
        Assert.Equal(roleId.ToString(), principal.FindFirst("Role")?.Value);
        Assert.NotNull(principal.FindFirst(JwtRegisteredClaimNames.Jti));
    }

    [Fact]
    public void ValidateAccessToken_NullToken_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _jwtService.ValidateAccessToken(null));
        Assert.Contains("The parameter 'token' cannot be a 'null' or an empty object.", exception.Message);
    }

    [Fact]
    public void ValidateAccessToken_EmptyToken_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _jwtService.ValidateAccessToken(""));
        Assert.Contains("The parameter 'token' cannot be a 'null' or an empty object.", exception.Message);
    }

    [Fact]
    public void ValidateAccessToken_InvalidTokenFormat_ThrowsSecurityTokenMalformedException()
    {
        // Arrange
        var invalidToken = "invalid.token.format";

        // Act & Assert
        var exception = Assert.Throws<SecurityTokenMalformedException>(() => _jwtService.ValidateAccessToken(invalidToken));
        Assert.Equal("Invalid token format", exception.Message);
    }

    [Fact]
    public void ValidateAccessToken_IncorrectAlgorithm_ThrowsSecurityTokenMalformedException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var username = "testuser";
        var roleId = 1;
        var token = GenerateJwtTokenWithCustomAlgorithm(userId, username, roleId, SecurityAlgorithms.HmacSha512);

        // Act & Assert
        var exception = Assert.Throws<SecurityTokenMalformedException>(() => _jwtService.ValidateAccessToken(token));
        Assert.Equal("Invalid token format", exception.Message);
    }

    [Fact]
    public void ValidateAccessToken_ExpiredToken_ThrowsSecurityTokenMalformedException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var username = "testuser";
        var roleId = 1;
        var token = GenerateJwtTokenWithCustomExpiration(userId, username, roleId, DateTime.UtcNow.AddMinutes(-10));

        // Act & Assert
        var exception = Assert.Throws<SecurityTokenMalformedException>(() => _jwtService.ValidateAccessToken(token));
        Assert.Equal("Invalid token format", exception.Message);
    }

    [Fact]
    public void ValidateAccessToken_InvalidIssuer_ThrowsSecurityTokenMalformedException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var username = "testuser";
        var roleId = 1;
        var token = GenerateJwtTokenWithCustomIssuer(userId, username, roleId, "wrong-issuer");

        // Act & Assert
        var exception = Assert.Throws<SecurityTokenMalformedException>(() => _jwtService.ValidateAccessToken(token));
        Assert.Equal("Invalid token format", exception.Message);
    }

    [Fact]
    public void ValidateAccessToken_InvalidAudience_ThrowsSecurityTokenMalformedException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var username = "testuser";
        var roleId = 1;
        var token = GenerateJwtTokenWithCustomAudience(userId, username, roleId, "wrong-audience");

        // Act & Assert
        var exception = Assert.Throws<SecurityTokenMalformedException>(() => _jwtService.ValidateAccessToken(token));
        Assert.Equal("Invalid token format", exception.Message);
    }

    // Helper method to generate a token with a custom algorithm
    private string GenerateJwtTokenWithCustomAlgorithm(string userId, string username, int roleId, string algorithm)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, algorithm);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("Role", roleId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Helper method to generate a token with a custom expiration
    private string GenerateJwtTokenWithCustomExpiration(string userId, string username, int roleId, DateTime expires)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("Role", roleId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Helper method to generate a token with a custom issuer
    private string GenerateJwtTokenWithCustomIssuer(string userId, string username, int roleId, string issuer)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("Role", roleId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Helper method to generate a token with a custom audience
    private string GenerateJwtTokenWithCustomAudience(string userId, string username, int roleId, string audience)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("Role", roleId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
}